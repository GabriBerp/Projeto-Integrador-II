using UnityEngine;

public class PoloComercial : PoloBase
{
    [Header("Comercial")]
    [SerializeField] private float baseValue = 2f;              // Vb_c
    [SerializeField] private float baseClickMoney = 1f;         // o "1" de C_c = 1 + N_c
    [SerializeField] private int baseHandworkConsumption = 1;   // CbM_c
 
    public float AutoProduction => baseValue * Level;           // P_c = Vb_c * N_c
    public float MoneyPerClick => baseClickMoney + Level;       // C_c = 1 + N_c
    public override int HandworkConsumption => baseHandworkConsumption * Level; // CM_c

    protected override void OnCycleComplete()
    {
        // Se faltar mão de obra, a produção cai proporcionalmente.
        float satisfaction = ConsumeHandwork(HandworkConsumption);
        GameManager.Instance.money += AutoProduction * satisfaction;
    }
 
    protected override void OnClickEffect()
    {
        if (GameManager.Instance.handwork > 1 * Level)
        {
            GameManager.Instance.handwork -= 1 * Level;
            GameManager.Instance.money += MoneyPerClick;
        }
    }

    protected override void OnLevelChanged()
    {
        
    }
}