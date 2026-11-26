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
    public float baseTrashCap = 50f;
    public float TrashCap
    {
        get
        {
            int level = Reciclagem != null ? Reciclagem.Level : 1;

            // Tier 1 vale até o nível 5; cada tier seguinte dobra o teto (10, 20, 40...)
            int tier = 1;
            int limit = 5;
            while (level > limit)
            {
                limit *= 2;
                tier++;
            }
            return baseTrashCap * tier;
        }
    }

    [Header("UI - ResourcePanel")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI handworkText;
    [SerializeField] private TextMeshProUGUI trashText;
    [SerializeField] private Slider lifeQualitySlider;

    [Header ("GameObjects")]
    [SerializeField] private GameObject infoPanel;
    public PoloBase selectedPolo;
    [SerializeField] private GameObject blockedOverlay;
    public PoloReciclagem Reciclagem {get; private set;}

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
    [SerializeField] private Image productionFillImage;

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

        if (selectedPolo != null)
        {
            selectedPolo.SetSelected(true);
            Reciclagem = selectedPolo as PoloReciclagem;
        }

        UpdateInfoPanel();
    }

    void Update() 
    {
        UpdateUI();
        UpdateProductionFill();
        trash = Mathf.Clamp(trash, 0, TrashCap);
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

    void UpdateProductionFill()
    {
        if (productionFillImage == null) return;

        if (selectedPolo == null || selectedPolo.IsLocked)
        {
            productionFillImage.fillAmount = 0f;
            return;
        }

        productionFillImage.fillAmount = selectedPolo.TimeRemaining;
    }

    void UpdateInfoPanel()
    {
        if (selectedPolo == null || selectedPolo.IsLocked) return;

        // poloNameText.text      = selectedPolo.poloName;
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
        if (selectedPolo != null) selectedPolo.SetSelected(false);
        selectedPolo = polo;
        if (selectedPolo != null) selectedPolo.SetSelected(true);
        RefreshInfoPanel();
    }

    public bool IsInfoPanelOpen => infoPanelScript != null && infoPanelScript.state;

    public void CloseInfoPanel()
    {
        SelectPolo(null); // tira o contorno ao fechar o painel
        if (infoPanelScript != null) infoPanelScript.Hide();
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
            // poloNameText.text = "";
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

        // poloNameText.text      = polo.poloName;
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
