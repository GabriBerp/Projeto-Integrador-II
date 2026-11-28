using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PoloVisual : MonoBehaviour
{
    [Header("Bloqueado")]
    [SerializeField, Range(0f, 1f)] private float lockedBrightness = 0.35f;
    [Tooltip("Opcional. Se vazio, o cadeado é montado com primitivas.")]
    [SerializeField] private GameObject lockPrefab;
    [SerializeField] private Color lockColor = new Color(0.95f, 0.95f, 0.95f);

    [Header("Upgrade")]
    [Tooltip("Opcional. Se vazio, a seta é montada com primitivas.")]
    [SerializeField] private GameObject arrowPrefab;

    [Header("Indicadores")]
    [Tooltip("Cor da seta de upgrade e do cadeado quando dá para desbloquear.")]
    [SerializeField] private Color affordableColor = new Color(0.25f, 0.9f, 0.3f);
    [FormerlySerializedAs("lockSize")]
    [SerializeField] private float indicatorSize = 1f;
    [FormerlySerializedAs("lockHeight")]
    [Tooltip("Distância entre o topo do modelo e a base do indicador.")]
    [SerializeField] private float indicatorHeight = 0.3f;
    [Tooltip("Material base das peças procedurais (cadeado/seta). Precisa ter _BaseColor ou _Color. Ex.: URP/Unlit ou URP/Lit.")]
    [SerializeField] private Material indicatorMaterial;

    [Header("Seleção")]
    [SerializeField] private Color outlineColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private float outlineWidth = 0.03f;
    [Tooltip("Material com o shader Custom/SelectionOutline.")]
    [SerializeField] private Material outlineMaterial;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP
    private static readonly int ColorId = Shader.PropertyToID("_Color");         // Built-in

    private enum IndicatorState { None, Locked, Unlockable, Upgradable }

    private struct Slot
    {
        public Material material;
        public int colorId;
        public Color original;
        public bool tintable;
    }

    private PoloBase polo;
    private readonly List<Slot> slots = new List<Slot>();
    private readonly List<Slot> lockSlots = new List<Slot>();
    private readonly List<Slot> arrowSlots = new List<Slot>();
    private readonly List<MeshRenderer> outlines = new List<MeshRenderer>();
    private Material outlineInstance;
    private Transform lockRoot;
    private Transform arrowRoot;
    private IndicatorState? state;
    private Bounds bounds;

    void Awake()
    {
        polo = GetComponent<PoloBase>();

        bool hasBounds = false;
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            if (r is LineRenderer || r is ParticleSystemRenderer || r is TrailRenderer) continue;

            if (!hasBounds) { bounds = r.bounds; hasBounds = true; }
            else bounds.Encapsulate(r.bounds);

            // .materials instancia o material; assim escurecer um polo não afeta os outros
            foreach (var m in r.materials)
            {
                int id = GetColorId(m);
                if (id == -1) continue;
                slots.Add(new Slot { material = m, colorId = id, original = m.GetColor(id) });
            }
        }
    }

    void Start()
    {
        CreateOutlines();
        lockRoot = CreateLock();
        arrowRoot = CreateArrow();
        Refresh();
    }

    void Update() => UpdateIndicators();

    void LateUpdate()
    {
        var cam = Camera.main;
        if (cam == null) return;

        // Só gira no eixo Y: o indicador encara a câmera sem tombar quando ela olha de cima
        Vector3 toCam = cam.transform.position - transform.position;
        toCam.y = 0f;
        if (toCam.sqrMagnitude < 0.0001f) return;

        Quaternion facing = Quaternion.LookRotation(toCam);
        if (lockRoot != null) lockRoot.rotation = facing;
        if (arrowRoot != null) arrowRoot.rotation = facing;
    }

    void OnDestroy()
    {
        DestroyMaterials(slots);
        DestroyMaterials(lockSlots);
        DestroyMaterials(arrowSlots);
        if (outlineInstance != null) Destroy(outlineInstance);
    }

    // Chamado no início e quando o polo é desbloqueado
    public void Refresh()
    {
        float k = polo != null && polo.IsLocked ? lockedBrightness : 1f;
        foreach (var s in slots)
        {
            Color c = s.original;
            s.material.SetColor(s.colorId, new Color(c.r * k, c.g * k, c.b * k, c.a));
        }
        UpdateIndicators();
    }

    public void SetSelected(bool selected)
    {
        foreach (var o in outlines)
            if (o != null) o.enabled = selected;
    }

    // O dinheiro muda a todo frame, então consulta aqui; só mexe nos objetos quando o estado troca
    private void UpdateIndicators()
    {
        if (polo == null || GameManager.Instance == null) return;

        IndicatorState next = polo.IsLocked
            ? (polo.CanUnlock() ? IndicatorState.Unlockable : IndicatorState.Locked)
            : (polo.CanUpgrade() ? IndicatorState.Upgradable : IndicatorState.None);
        if (next == state) return;
        state = next;

        bool showLock = next == IndicatorState.Locked || next == IndicatorState.Unlockable;
        if (lockRoot != null) lockRoot.gameObject.SetActive(showLock);
        if (arrowRoot != null) arrowRoot.gameObject.SetActive(next == IndicatorState.Upgradable);
        TintLock(next == IndicatorState.Unlockable);
    }

    private void TintLock(bool affordable)
    {
        foreach (var s in lockSlots)
        {
            if (!s.tintable || s.colorId == -1) continue;
            s.material.SetColor(s.colorId, affordable ? s.original * affordableColor : s.original);
        }
    }

    private static int GetColorId(Material m) =>
        m.HasProperty(BaseColorId) ? BaseColorId : m.HasProperty(ColorId) ? ColorId : -1;

    private static void DestroyMaterials(List<Slot> list)
    {
        foreach (var s in list)
            if (s.material != null) Destroy(s.material);
    }

    private Transform CreateLock()
    {
        GameObject root;
        if (lockPrefab != null)
        {
            root = Instantiate(lockPrefab);
            foreach (var r in root.GetComponentsInChildren<Renderer>())
                foreach (var m in r.materials)
                {
                    int id = GetColorId(m);
                    lockSlots.Add(new Slot { material = m, colorId = id, original = id == -1 ? default : m.GetColor(id), tintable = true });
                }
        }
        else root = BuildProceduralLock();

        return PlaceIndicator(root);
    }

    private Transform CreateArrow()
    {
        GameObject root = arrowPrefab != null ? Instantiate(arrowPrefab) : BuildProceduralArrow();
        return PlaceIndicator(root);
    }

    private Transform PlaceIndicator(GameObject root)
    {
        Vector3 baseScale = root.transform.localScale;

        // Colliders atrapalhariam o raycast de seleção dos polos
        foreach (var c in root.GetComponentsInChildren<Collider>()) Destroy(c);

        var t = root.transform;
        t.SetParent(transform, false);

        // Compensa a escala do polo para o indicador ter tamanho fixo no mundo
        Vector3 ls = transform.lossyScale;
        t.localScale = new Vector3(
            baseScale.x * indicatorSize / ls.x,
            baseScale.y * indicatorSize / ls.y,
            baseScale.z * indicatorSize / ls.z);

        t.position = new Vector3(bounds.center.x, bounds.max.y + indicatorHeight, bounds.center.z);
        root.SetActive(false);
        return t;
    }

    // Origem na base do corpo; o buraco da fechadura fica em +Z (face que encara a câmera)
    private GameObject BuildProceduralLock()
    {
        var root = new GameObject("Lock");
        Color shackle = lockColor * 0.8f;
        lockSlots.Add(AddPart(root.transform, new Vector3(0f, 0.2f, 0f), new Vector3(0.5f, 0.4f, 0.2f), Quaternion.identity, lockColor, true));
        lockSlots.Add(AddPart(root.transform, new Vector3(-0.14f, 0.55f, 0f), new Vector3(0.07f, 0.3f, 0.07f), Quaternion.identity, shackle, true));
        lockSlots.Add(AddPart(root.transform, new Vector3(0.14f, 0.55f, 0f), new Vector3(0.07f, 0.3f, 0.07f), Quaternion.identity, shackle, true));
        lockSlots.Add(AddPart(root.transform, new Vector3(0f, 0.685f, 0f), new Vector3(0.35f, 0.07f, 0.07f), Quaternion.identity, shackle, true));
        lockSlots.Add(AddPart(root.transform, new Vector3(0f, 0.22f, 0.1f), new Vector3(0.08f, 0.12f, 0.04f), Quaternion.identity, Color.black, false));
        return root;
    }

    // Haste vertical + duas barras a 45° formando a ponta; origem na base da haste
    private GameObject BuildProceduralArrow()
    {
        var root = new GameObject("UpgradeArrow");
        Vector3 shaft = new Vector3(0.14f, 0.62f, 0.14f);
        Vector3 bar = new Vector3(0.4f, 0.14f, 0.14f);
        arrowSlots.Add(AddPart(root.transform, new Vector3(0f, 0.31f, 0f), shaft, Quaternion.identity, affordableColor, false));
        arrowSlots.Add(AddPart(root.transform, new Vector3(-0.14f, 0.61f, 0f), bar, Quaternion.Euler(0f, 0f, 45f), affordableColor, false));
        arrowSlots.Add(AddPart(root.transform, new Vector3(0.14f, 0.61f, 0f), bar, Quaternion.Euler(0f, 0f, -45f), affordableColor, false));
        return root;
    }

    private Slot AddPart(Transform parent, Vector3 pos, Vector3 scale, Quaternion rot, Color color, bool tintable)
    {
        var part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.transform.SetParent(parent, false);
        part.transform.localPosition = pos;
        part.transform.localRotation = rot;
        part.transform.localScale = scale;

        var r = part.GetComponent<MeshRenderer>();
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;

        Material m;
        if (indicatorMaterial != null)
        {
            m = new Material(indicatorMaterial);   // instância própria, para tingir sem afetar os outros
            r.sharedMaterial = m;
        }
        else
        {
            Debug.LogWarning($"{name}: indicatorMaterial não atribuído; as peças podem ficar roxas na build.", this);
            m = r.material;
        }

        int id = GetColorId(m);
        if (id != -1) m.SetColor(id, color);
        return new Slot { material = m, colorId = id, original = color, tintable = tintable };
    }

    private void CreateOutlines()
    {
        if (outlineMaterial == null)
        {
            Debug.LogError($"{name}: outlineMaterial não atribuído.", this);
            return;
        }

        outlineInstance = new Material(outlineMaterial);
        outlineInstance.SetColor("_OutlineColor", outlineColor);
        outlineInstance.SetFloat("_OutlineWidth", outlineWidth);

        foreach (var src in GetComponentsInChildren<MeshRenderer>())
        {
            var filter = src.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) continue;

            var go = new GameObject("SelectionOutline");
            go.transform.SetParent(src.transform, false);
            go.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;

            var mr = go.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;

            var mats = new Material[filter.sharedMesh.subMeshCount];
            for (int i = 0; i < mats.Length; i++) mats[i] = outlineInstance;
            mr.sharedMaterials = mats;

            mr.enabled = false;
            outlines.Add(mr);
        }
    }
}