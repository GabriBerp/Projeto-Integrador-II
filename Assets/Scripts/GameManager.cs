using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Valores")]
    public float money = 0;
    public float handwork = 0;
    public float trash = 0;
    public float lifeQuality = 0;
    public float maxLifeQuality = 100f;
    public float baseMoneyCap = 500f;
    public float baseHandWorkCap = 500f;
    public float baseTrashCap = 500f;

    [Header("UI - ResourcePanel")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI handworkText;
    [SerializeField] private TextMeshProUGUI trashText;
    [SerializeField] private Slider lifeQualitySlider;

    [Header ("GameObjects")]
    [SerializeField] private GameObject infoPanel;
    public PoloBase selectedPolo;
    [SerializeField] private GameObject blockedOverlay;

    [Header("UI - InfoPanel")]
    [SerializeField] private TextMeshProUGUI poloNameText;
    [SerializeField] private TextMeshProUGUI poloLevelText;
    [SerializeField] private TextMeshProUGUI productionText;
    [SerializeField] private TextMeshProUGUI consumeText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI unlockCostText;
    [SerializeField] private Image productionImage1;
    [SerializeField] private Image productionImage2;
    [SerializeField] private Image consumeImage;

    public Sprite moneyIcon;
    public Sprite handWorkIcon;
    public Sprite trashIcon;

    private InfoPanelScript infoPanelScript;

    void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (infoPanel != null)
            infoPanelScript = infoPanel.GetComponent<InfoPanelScript>();

        if (lifeQualitySlider != null)
        {
            lifeQualitySlider.minValue = 0f;
            lifeQualitySlider.maxValue = maxLifeQuality;
            lifeQualitySlider.interactable = false;   // jogador não mexe no slider
        }

        UpdateInfoPanel();
    }

    void Update() {
        UpdateUI(); 
        money = Mathf.Clamp(money, 0, baseMoneyCap);
        handwork = Mathf.Clamp(handwork, 0, baseHandWorkCap);
        trash = Mathf.Clamp(trash, 0, baseTrashCap);
    }

    void UpdateUI()
    {
        moneyText.text = money.ToString("0.00");
        handworkText.text = handwork.ToString("0");
        trashText.text = trash.ToString("0");

        lifeQuality = Mathf.Clamp(lifeQuality, 0f, maxLifeQuality);

        if (lifeQualitySlider != null)
            lifeQualitySlider.value = lifeQuality;
    }

    void UpdateInfoPanel()
    {
        if (selectedPolo == null || selectedPolo.IsLocked) return;

        poloNameText.text      = selectedPolo.poloName;
        poloLevelText.text     = "Nível " + selectedPolo.Level;
        productionText.text    = selectedPolo.GetProductionText();
        consumeText.text       = selectedPolo.GetConsumptionText();
        upgradeCostText.text   = "$ " + selectedPolo.CurrentUpgradeCost.ToString("0.00");

        // productionImage1 = moneyIcon; => se for PoloComercial ou PoloReciclagem
        if (selectedPolo as PoloReciclagem != null || selectedPolo as PoloComercial != null)
        {
            productionImage1.sprite = moneyIcon;
        }
        // productionImagem1 = handWorkIcon => se for PoloResidencial
        if (selectedPolo as PoloResidencial != null)
        {
            productionImage1.sprite = moneyIcon;
        }

        if (selectedPolo.trashGenerator != null)
        {
            productionImage2.sprite = trashIcon;
        }
        else
        {
           productionImage2.sprite = null; 
        }

        if (consumeText.text != "   .")
        {
            // consumeImage.sprite = trashIcon; => se for PoloReciclagem
            if (selectedPolo as PoloReciclagem != null)
            {
                consumeImage.sprite = trashIcon;
            }
            // consumeImage.sprite = handWorkIcon; => se for PoloComercial
            if (selectedPolo as PoloComercial != null)
            {
                consumeImage.sprite = handWorkIcon;
            }
        }
        else
        {
            consumeImage.sprite = null;
        }
    }

    public void SelectPolo(PoloBase polo)
    {
        selectedPolo = polo;
        RefreshInfoPanel();
    }

    public void OpenInfoPanel()
    {
        if (infoPanelScript != null) infoPanelScript.Show();
    }

    void RefreshInfoPanel()
    {
        var polo = selectedPolo;   // o nome do seu campo pode ser outro
        if (polo == null) return;

        bool locked = polo.IsLocked;
        blockedOverlay.SetActive(locked);

        if (locked)
        {
            poloNameText.text = "";
            poloLevelText.text = "";
            productionText.text = "";
            consumeText.text = "";
            upgradeCostText.text = "";
            unlockCostText.text = "$ " + polo.UnlockCost.ToString("0.00");

            productionImage1.sprite = null;
            productionImage2.sprite = null;
            consumeImage.sprite = null;
            return;
        }

        poloNameText.text      = polo.poloName;
        poloLevelText.text     = "Nível " + polo.Level;
        productionText.text    = polo.GetProductionText();
        consumeText.text       = polo.GetConsumptionText();
        upgradeCostText.text   = "$ " + polo.CurrentUpgradeCost.ToString("0.00");

        // productionImage1 = moneyIcon; => se for PoloComercial ou PoloReciclagem
        if (selectedPolo as PoloReciclagem != null || selectedPolo as PoloComercial != null)
        {
            productionImage1.sprite = moneyIcon;
        }
        // productionImagem1 = handWorkIcon => se for PoloResidencial
        if (selectedPolo as PoloResidencial != null)
        {
            productionImage1.sprite = moneyIcon;
        }

        if (selectedPolo.trashGenerator != null)
        {
            productionImage2.sprite = trashIcon;
        }
        else
        {
           productionImage2.sprite = null; 
        }

        if (consumeText.text != "   .")
        {
            // consumeImage.sprite = trashIcon; => se for PoloReciclagem
            if (selectedPolo as PoloReciclagem != null)
            {
                consumeImage.sprite = trashIcon;
            }
            // consumeImage.sprite = handWorkIcon; => se for PoloComercial
            if (selectedPolo as PoloComercial != null)
            {
                consumeImage.sprite = handWorkIcon;
            }
        }
        else
        {
            consumeImage.sprite = null;
        }
    }

    public void UnlockSelectedPolo()
    {
        if (selectedPolo != null && selectedPolo.TryUnlock())
            RefreshInfoPanel();
    }
    public void UpdateButtonClick()
    {
        if (selectedPolo != null && selectedPolo.TryUpgrade())
        {
            RefreshInfoPanel();
        }
    }
}
