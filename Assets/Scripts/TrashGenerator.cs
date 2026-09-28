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
        GameManager.Instance.trash += trashProduction * poloLevel;
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
