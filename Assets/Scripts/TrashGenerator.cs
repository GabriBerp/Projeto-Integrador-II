using UnityEngine;

public class TrashGenerator : MonoBehaviour
{
    [Header("Trash Generator Info")]
    private int trashProduction;
    [SerializeField] private int _baseTrashProduction;
    public PoloBase polo;
    [SerializeField] private float cycleMaxTime;
    public float _cycletime = 0f;
    [SerializeField] private float timeReductionPerClick;

    void Awake() {
        trashProduction = _baseTrashProduction;
    }
}
