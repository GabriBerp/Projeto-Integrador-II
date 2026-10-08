using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public abstract class PoloBase : MonoBehaviour
{
    [Header("Polo Info")]
    public string poloName;
    public TrashGenerator trashGenerator;
    public float CycleMaxTime => cycleMaxTime;

    // ---------- Textos para o InfoPanel ----------
    protected static string FormatMoney(float v, float t)    => $"$ {v:0.00} / {t:0.##} s";
    protected static string FormatHandwork(float v, float t) => $"🛠️ {v:0.##} / {t:0.##} s";
    protected static string FormatTrash(float v, float t)    => $"🗑️ {v:0.##} / {t:0.##} s";

    // Cada polo descreve a própria produção
    protected abstract string GetProductionLine();

    // Por padrão não consome nada
    public virtual string GetConsumptionText() => "-";

    // Produção do polo + lixo do TrashGenerator (se houver)
    public string GetProductionText()
    {
        if (isLocked) return "";
        
        string text = GetProductionLine();
        if (trashGenerator != null)
            text += "\n" + trashGenerator.GetProductionLine();
        return text;
    }
 
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

    [Header("Bloqueio")]
    [SerializeField] protected bool isLocked = false;
    [SerializeField] protected float unlockCost = 50f;
    [SerializeField] protected Image lockTarget;                       // Image do botão do polo
    [SerializeField] protected Color lockedTint = new Color(0.45f, 0.45f, 0.45f, 1f);

    private Color originalColor = Color.white;

    public bool IsLocked => isLocked;
    public float UnlockCost => unlockCost;
    public bool CanUnlock() => isLocked && GameManager.Instance.money >= unlockCost;

    [Header("Debug Mode")]
    [SerializeField] protected bool showTimer = false;
    [SerializeField] protected TextMeshProUGUI timerText;
 
    protected virtual void Awake()
    {
        Level = 1;

        if (lockTarget != null) originalColor = lockTarget.color;
        ApplyLockVisual();
    }

    public bool TryUnlock()
    {
        if (!CanUnlock()) return false;

        GameManager.Instance.money -= unlockCost;
        isLocked = false;
        ApplyLockVisual();
        return true;
    }

    protected void ApplyLockVisual()
    {
        if (lockTarget != null)
            lockTarget.color = isLocked ? lockedTint : originalColor;
    }
 
    protected virtual void Update()
    {
        if (isLocked) return; 

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
        GameManager.Instance.SelectPolo(this);
        if (isLocked)
        {
            // Só abre o painel; o overlay e os textos vazios são tratados pelo GameManager
            GameManager.Instance.OpenInfoPanel();
            return;
        }

        trashGenerator?.OnClick();
        OnClickEffect();
        AdvanceTime(baseTimeReductionPerClick);
    }
 
    // Efeito extra do clique (ex.: dinheiro por clique no comercial).
    protected virtual void OnClickEffect() { }
 
    public bool CanUpgrade() => !isLocked && GameManager.Instance.money >= CurrentUpgradeCost;
 
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
 
        float available = GameManager.Instance.handwork;
        float consumed = Mathf.Min(available, needed);
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