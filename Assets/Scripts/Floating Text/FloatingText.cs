using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animação")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float riseDistance = 80f;
    [SerializeField] private float randomX = 30f;

    public void Show(string message, Color color)
    {
        text.text = message;
        text.color = color;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        RectTransform rt = (RectTransform)transform;
        Vector2 start = rt.anchoredPosition + new Vector2(Random.Range(-randomX, randomX), 0);
        Vector2 end = start + Vector2.up * riseDistance;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / duration;

            // sobe rápido no começo e desacelera
            float eased = 1f - Mathf.Pow(1f - p, 3f);
            rt.anchoredPosition = Vector2.Lerp(start, end, eased);

            // "pop" de escala no começo
            float scale = p < 0.15f ? Mathf.Lerp(0.5f, 1.3f, p / 0.15f)
                                    : Mathf.Lerp(1.3f, 1f, (p - 0.15f) / 0.85f);
            rt.localScale = Vector3.one * scale;

            // some na segunda metade
            canvasGroup.alpha = p < 0.5f ? 1f : 1f - (p - 0.5f) / 0.5f;

            yield return null;
        }

        Destroy(gameObject); // depois você pode trocar por object pooling
    }
}