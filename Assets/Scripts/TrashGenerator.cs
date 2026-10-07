using UnityEngine;

public class TrashGenerator : MonoBehaviour
{
    [Header("Trash Generator Info")]
    private int trashProduction;
    [SerializeField] private int _baseTrashProduction;
    public PoloBase polo;
    private int poloLevel;
    [SerializeField] private float cycleMaxTime;
    public float _cycletime = 0f;
    private float timeReductionPerClick;
   [SerializeField] private float _baseTimeReductionPerClick;

    public int ProductionPerCycle => _baseTrashProduction * (polo != null ? polo.Level : 1);

    public string GetProductionLine() => $"🗑️ {ProductionPerCycle} / {cycleMaxTime:0.##} s";


    void Awake() {
        trashProduction = _baseTrashProduction;
        timeReductionPerClick = _baseTimeReductionPerClick;
    }

    void Start() {
        if (polo != null)
        {
            poloLevel = polo.Level;
        }    
    }

    void FixedUpdate() {
        if (_cycletime < cycleMaxTime)
        {
            _cycletime += Time.deltaTime;
        }else
        {
            GenerateTrash();
            _cycletime = 0f;
        } 
    }

    public void GenerateTrash()
    {
        GameManager.Instance.trash += ProductionPerCycle;
    }

    public void OnClick()
    {
        if (_cycletime < cycleMaxTime)
        {
            _cycletime += timeReductionPerClick;
        }else
        {
            GenerateTrash();
            _cycletime = 0f;
        } 
        // Debug.Log(gameObject.name + ": apertou");
    }
}
