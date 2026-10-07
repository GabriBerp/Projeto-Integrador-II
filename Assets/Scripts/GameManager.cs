using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Valores")]
    public float money = 0;
    public float handwork = 0;
    public float trash = 0;
    public float lifeQuality = 0;

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI handworkText;
    public TextMeshProUGUI trashText;
    public TextMeshProUGUI productionText;
    public TextMeshProUGUI consumeText;
    public TextMeshProUGUI upgradeCostText;

    [Header ("GameObjects")]
    public GameObject infoPanel;
    public PoloBase selectedPolo;

    [Header("UI - InfoPanel")]
    public TextMeshProUGUI poloNameText;
    public TextMeshProUGUI poloLevelText;
    // productionText, consumeText e upgradeCostText já existem

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
    }

    void Update() {
        UpdateUI();  
        UpdateInfoPanel();  
    }

    void UpdateUI()
    {
        moneyText.text = money.ToString("0.00");
        handworkText.text = handwork.ToString("0");
        trashText.text = trash.ToString("0");
    }

    void UpdateInfoPanel()
    {
        if (selectedPolo == null) return;

        poloNameText.text      = selectedPolo.poloName;
        poloLevelText.text     = "Nível " + selectedPolo.Level;
        productionText.text    = selectedPolo.GetProductionText();
        consumeText.text       = selectedPolo.GetConsumptionText();
        upgradeCostText.text   = "$ " + selectedPolo.CurrentUpgradeCost.ToString("0.00");
    }

    public void SelectPolo(PoloBase polo)
    {
        selectedPolo = polo;
    }

    public void OpenInfoPanel()
    {
        if (infoPanelScript != null) infoPanelScript.Show();
    }

    public void UpdateButtonClick()
    {
        if (selectedPolo == null) return;
        bool updated = selectedPolo.TryUpgrade();
        if (updated)
        {
            Debug.Log("Debug: " + selectedPolo.gameObject.name + " updated");
        }
        else
        {
            Debug.Log("Debug: " + selectedPolo.gameObject.name + " cannot be updated");
        }
    }

}
