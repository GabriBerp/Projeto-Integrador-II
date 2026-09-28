using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Valores")]
    public float money;
    public int handwork;
    public int trash;
    public float lifeQuality;

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
        moneyText.text = money.ToString("#.00");
        handworkText.text = handwork.ToString("#");
        trashText.text = trash.ToString("#");
    }

}
