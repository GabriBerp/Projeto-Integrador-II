using UnityEngine;
using TMPro;
 
public abstract class PoloBase : MonoBehaviour
{
    [Header("Polo Info")]
    public string poloName;
    public TrashGenerator trashGenerator;
 
    [Header("Ciclo")]
    [SerializeField] protected float cycleMaxTime = 5f;               // TR
    [SerializeField] protected float baseTimeReductionPerClick = 0.5f; // RL
    public float CycleTime { get; protected set; }                    // tempo decorrido no ciclo
 
    [Header("Upgrade")]
    [SerializeField] protected float baseUpgradeCost = 10f;
    [SerializeField] protected float upgradeCostGrowth = 1.15f;
    [SerializeField] protected float baseQualityOfLifePerUpgrade = 1f; // QbVM
 
    public int Level { get; protected set; } = 1; // N
 
    // Custo = base * 1.15^(N-1)
    public float CurrentUpgradeCost => baseUpgradeCost * Mathf.Pow(upgradeCostGrowth, Level - 1);
 
    // Para UI: quanto falta pro fim do ciclo (TR_novo = TR - Δt - RL*K)
    public float TimeRemaining => Mathf.Max(0f, cycleMaxTime - CycleTime);
 
    // Mão de obra que este polo consome por ciclo (usado p/ CM_total).
    public virtual int HandworkConsumption => 0;

    [Header("Debug Mode")]
    [SerializeField] protected bool showTimer = false;
    [SerializeField] protected TextMeshProUGUI timerText;
 
    protected virtual void Awake()
    {
        Level = 1;
    }
 
    protected virtual void Update()
    {
        AdvanceTime(Time.deltaTime);

        DebugMode();
    }
 
    // Soma tempo (automático ou por clique) e dispara quantos ciclos completaram.
    protected void AdvanceTime(float amount)
    {
        if (cycleMaxTime <= 0f) return;
 
        CycleTime += amount;
        while (CycleTime >= cycleMaxTime)
        {
            CycleTime -= cycleMaxTime; // preserva o excedente
            OnCycleComplete();
        }
    }

    // Cada polo define o que produz no fim do ciclo.
    protected abstract void OnCycleComplete();
 
    public virtual void OnClick()
    {
        trashGenerator?.OnClick();
        OnClickEffect();
        AdvanceTime(baseTimeReductionPerClick); // RL * K (um clique)

        if (GameManager.Instance.selectedPolo != this)
        {
            GameManager.Instance.selectedPolo = this;
        }
    }
 
    // Efeito extra do clique (ex.: dinheiro por clique no comercial).
    protected virtual void OnClickEffect() { }
 
    public bool CanUpgrade() => GameManager.Instance.money >= CurrentUpgradeCost;
 
    public virtual bool TryUpgrade()
    {
        if (!CanUpgrade()) return false;
 
        GameManager.Instance.money -= CurrentUpgradeCost; // custo do nível atual
        Level++;
 
        // QVM = QbVM * N_novo
        GameManager.Instance.lifeQuality += baseQualityOfLifePerUpgrade * Level;
 
        OnLevelChanged();
        return true;
    }
 
    protected virtual void OnLevelChanged() { }
 
    // Fração (0..1) do consumo de mão de obra que pôde ser atendida; consome o que atendeu.
    protected float ConsumeHandwork(int needed)
    {
        if (needed <= 0) return 1f;
 
        int available = GameManager.Instance.handwork;
        int consumed = Mathf.Min(available, needed);
        GameManager.Instance.handwork -= consumed;
        return (float)consumed / needed;
    }

    protected void DebugMode()
    {
        if (timerText != null && showTimer)
        {
            timerText.text = CycleTime.ToString("0.00s");
        }
    }
}