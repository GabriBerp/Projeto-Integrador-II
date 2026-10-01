using UnityEngine;

public class PoloReciclagem : PoloBase
{
    [Header("Reciclagem")]
    [SerializeField] private int baseCapacity = 5;     // QbR
    [SerializeField] private float trashPrice = 1f;    // V_l
 
    public int Capacity => baseCapacity * Level;       // QR = QbR * N
 
    protected override void OnCycleComplete()
    {
        int recycled = Mathf.Min(GameManager.Instance.trash, Capacity); // LR = min(L, QR)
        if (recycled <= 0) return;
 
        GameManager.Instance.trash -= recycled;
        GameManager.Instance.money += recycled * trashPrice;            // D = LR * V_l
    }

    protected override void OnLevelChanged()
    {
        
    }
}
