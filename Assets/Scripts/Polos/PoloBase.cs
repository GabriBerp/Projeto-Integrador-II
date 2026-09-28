using UnityEngine;

public class PoloBase : MonoBehaviour
{
    [Header("Polo Info")]
    [SerializeField] protected float moneyPerTime;
    [SerializeField] protected float moneyPerClick;
    private int Level { get; set; }

    public virtual void Awake() {
        Level = 1;    
    }

    public virtual void ProduceMoneyPerTime()
    {
        GameManager.Instance.money += moneyPerTime;
    }

    public virtual void ProduceMoneyPerClick()
    {
        GameManager.Instance.money += moneyPerClick;
    }

    public virtual void ProduceMoneyPerCycle()
    {
        GameManager.Instance.money += moneyPerTime;
    }
}
