using UnityEngine;

public class PoloBase : MonoBehaviour
{
    [Header("Polo Variables")]
    [SerializeField] protected float moneyPerTime;
    [SerializeField] protected float moneyPerClick;
    private int Level { get; set; }
    [Header("Polo Info")]
    public string poloName;
    [Header("Upgrade Variable")]
    public float upgradePrice;


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

    public virtual void ShowPoloInfo()
    {
        
    }

    public virtual void Upgrade()
    {
        Level++;
        GameManager.Instance.money -= upgradePrice;
        
        // O resto é os proprios polos que adicionam, seloko mo preguiça.
    }
}
