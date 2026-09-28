using UnityEngine;

public class PoloBase : MonoBehaviour
{
    [Header("Polo Variables")]
    [SerializeField] protected float moneyPerTime;
    [SerializeField] protected float moneyPerClick;
    public TrashGenerator trashGenerator;
    public int Level { get; set; }
    [SerializeField] protected float cycleMaxTime;
    public float _cycletime = 0f;
    protected float timeReductionPerClick;
   [SerializeField] protected float _baseTimeReductionPerClick;
    [Header("Polo Info")]
    public string poloName;
    [Header("Upgrade Variable")]
    public float upgradePrice;


    public virtual void Awake() {
        Level = 1;    
        timeReductionPerClick = _baseTimeReductionPerClick;
    }

    public virtual void ProduceMoneyPerClick()
    {
        GameManager.Instance.money += moneyPerClick;
    }

    public virtual void ProduceMoneyPerCycle()
    {
        GameManager.Instance.money += moneyPerTime;
    }

    public virtual void ProduceHandWorkPerCycle()
    {
        GameManager.Instance.handwork += (int)moneyPerTime;
    }

    public virtual void ShowPoloInfo()
    {
        
    }

    public virtual void Upgrade()
    {
        Level++;
        GameManager.Instance.money -= upgradePrice;

        // O resto é os proprios polos que adicionam, seloko mo preguiça.
    }

    public virtual void OnClick()
    {
        trashGenerator?.OnClick();
    }
}
