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

    protected override string GetProductionLine() => FormatMoney(AutoProduction, cycleMaxTime);
    public override string GetConsumptionText()   => FormatHandwork(HandworkConsumption, cycleMaxTime);
    protected override void OnCycleComplete()
    {
        // Se faltar mão de obra, a produção cai proporcionalmente.
        float satisfaction = ConsumeHandwork(HandworkConsumption);
        GameManager.Instance.money += AutoProduction * satisfaction;
    }
 
    protected override void OnClickEffect()
    {
        float val = 0.5f * Level;
        if (GameManager.Instance.handwork > val)
        {
            GameManager.Instance.handwork -= val;
            GameManager.Instance.money += MoneyPerClick;
        }
    }

    protected override void OnLevelChanged()
    {
        
    }
}