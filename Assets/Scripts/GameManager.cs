using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Valores")]
    public float money = 0;
    public int handwork = 0;
    public int trash = 0;
    public float lifeQuality = 0;

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI handworkText;
    public TextMeshProUGUI trashText;

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

}
