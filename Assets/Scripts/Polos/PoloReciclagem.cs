using UnityEngine;

public class PoloReciclagem : PoloBase
{
    [Header("Reciclagem")]
    [SerializeField] private int baseCapacity = 5;     // QbR
    [SerializeField] private float trashPrice = 1f;    // V_l
    [SerializeField] private float baseClickMoney = 1f; 
 
    public int Capacity => baseCapacity * Level;       // QR = QbR * N
    public float MoneyPerClick => baseClickMoney + Level;

    protected override string GetProductionLine() => FormatMoney(Capacity * trashPrice, cycleMaxTime);
    public override string GetConsumptionText()   => FormatTrash(Capacity, cycleMaxTime);
 
    protected override void OnCycleComplete()
    {
        float recycled = Mathf.Min(GameManager.Instance.trash, Capacity); // LR = min(L, QR)
        if (recycled <= 0) return;
 
        GameManager.Instance.trash -= recycled;
        GameManager.Instance.money += recycled * trashPrice;            // D = LR * V_l
    }

    protected override void OnLevelChanged()
    {
        
    }

    protected override void OnClickEffect()
    {
        float val = 0.5f * Level;
        if (GameManager.Instance.trash > val)
        {
            GameManager.Instance.trash -= val;
            GameManager.Instance.money += MoneyPerClick;
        }
    }
}
