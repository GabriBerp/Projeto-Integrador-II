using UnityEngine;
using TMPro;

public class ResourcePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float moveSpeed = 12f;

    private RectTransform rect;
    private Vector2 target;
    private bool leaving;

    public ResourceType Type { get; private set; }
    public float Height => rect.rect.height;
    public float Width => rect.rect.width;

    void Awake()
    {
        rect = (RectTransform)transform;
    }

    public void Setup(ResourceType type, string message)
    {
        Type = type;
        messageText.text = message;
    }

    public void SnapTo(Vector2 pos)
    {
        rect.anchoredPosition = pos;
        target = pos;
    }

    public void SetTarget(Vector2 pos)
    {
        if (!leaving) target = pos;
    }

    // Desliza de volta pro lado de onde veio, mantendo a altura atual
    public void Dismiss(float hiddenX)
    {
        leaving = true;
        target = new Vector2(hiddenX, rect.anchoredPosition.y);
    }

    void Update()
    {
        // unscaledDeltaTime: funciona mesmo se você pausar o jogo (timeScale = 0)
        float t = 1f - Mathf.Exp(-moveSpeed * Time.unscaledDeltaTime);
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, target, t);

        if (leaving && Vector2.Distance(rect.anchoredPosition, target) < 1f)
            Destroy(gameObject);
    }
}