using UnityEngine;

public class PoloResidencial : PoloBase
{
    [Header("Residencial")]
    [SerializeField] private int baseHandworkProduction = 3; // QbM
 
    public int HandworkProduction => baseHandworkProduction * Level; // QM = QbM * N

    protected override string GetProductionLine() => FormatHandwork(HandworkProduction, cycleMaxTime);
 
    protected override void OnCycleComplete()
    {
        GameManager.Instance.handwork += HandworkProduction;
        
        FloatingTextSpawner.Instance.SpawnAtUI(HandworkProduction, (RectTransform)transform);
    }

    protected override void OnLevelChanged()
    {
        
    }

    protected override void OnClickEffect()
    {
        float val = 0.5f * Level;
        GameManager.Instance.handwork += val;

        FloatingTextSpawner.Instance.SpawnAtScreenPoint(val, Input.mousePosition);
    }
}
