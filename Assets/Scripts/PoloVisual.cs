using System.Collections.Generic;
using UnityEngine;

public class PoloVisual : MonoBehaviour
{
    [Header("Bloqueado")]
    [SerializeField, Range(0f, 1f)] private float lockedBrightness = 0.35f;

    [Header("Seleção")]
    [SerializeField] private Color outlineColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private float outlineWidth = 0.08f;
    [SerializeField] private float outlinePadding = 0.15f;
    [Tooltip("Opcional. Se vazio, usa Sprites/Default (atribua um material para garantir que entre na build).")]
    [SerializeField] private Material outlineMaterial;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP
    private static readonly int ColorId = Shader.PropertyToID("_Color");         // Built-in

    private struct Slot
    {
        public Material material;
        public int colorId;
        public Color original;
    }

    private PoloBase polo;
    private readonly List<Slot> slots = new List<Slot>();
    private LineRenderer outline;
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
                int id = m.HasProperty(BaseColorId) ? BaseColorId : m.HasProperty(ColorId) ? ColorId : -1;
                if (id == -1) continue;
                slots.Add(new Slot { material = m, colorId = id, original = m.GetColor(id) });
            }
        }
    }

    void Start()
    {
        CreateOutline();
        Refresh();
    }

    void OnDestroy()
    {
        foreach (var s in slots)
            if (s.material != null) Destroy(s.material);
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
    }

    public void SetSelected(bool selected)
    {
        if (outline != null) outline.enabled = selected;
    }

    private void CreateOutline()
    {
        var go = new GameObject("SelectionOutline");
        go.transform.SetParent(transform, false);

        outline = go.AddComponent<LineRenderer>();
        outline.useWorldSpace = false;
        outline.loop = true;
        outline.positionCount = 4;
        outline.startWidth = outline.endWidth = outlineWidth;
        outline.startColor = outline.endColor = outlineColor;
        outline.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        outline.receiveShadows = false;
        outline.material = outlineMaterial != null ? outlineMaterial : new Material(Shader.Find("Sprites/Default"));

        // Retângulo no chão, um pouco acima para não z-fightar com o terreno
        float y = bounds.min.y + 0.02f;
        float x0 = bounds.min.x - outlinePadding, x1 = bounds.max.x + outlinePadding;
        float z0 = bounds.min.z - outlinePadding, z1 = bounds.max.z + outlinePadding;

        outline.SetPosition(0, transform.InverseTransformPoint(new Vector3(x0, y, z0)));
        outline.SetPosition(1, transform.InverseTransformPoint(new Vector3(x0, y, z1)));
        outline.SetPosition(2, transform.InverseTransformPoint(new Vector3(x1, y, z1)));
        outline.SetPosition(3, transform.InverseTransformPoint(new Vector3(x1, y, z0)));

        outline.enabled = false;
    }
}