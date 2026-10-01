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

    void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        UpdateUI();    
    }

    void UpdateUI()
    {
        moneyText.text = money.ToString("0.00");
        handworkText.text = handwork.ToString("0");
        trashText.text = trash.ToString("0");
    }

    public void UpdateButtonClick()
    {
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
