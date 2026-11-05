using System.Collections;
using UnityEngine;

public class InfoPanelScript : MonoBehaviour
{
    Vector2 startPos;
    Vector2 endPos;
    RectTransform rect;
    Coroutine animCoroutine;

    public float offsetY;
    public float duration = 0.15f; // duração da animação em segundos
    public bool state = false; // false - Escondido | true - Revelado

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        endPos = startPos;
        endPos.y += offsetY;
    }

    public void Panel_Click()
    {
        if (state) Hide();
        else Show();
    }

    public void Show()
    {
        state = true;
        AnimateTo(endPos);
    }

    public void Hide()
    {
        state = false;
        AnimateTo(startPos);
    }

    void AnimateTo(Vector2 target)
    {
        // Cancela a animação anterior se o jogador clicar rápido
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(MoveRoutine(target));
    }

    IEnumerator MoveRoutine(Vector2 target)
    {
        Vector2 from = rect.anchoredPosition; // parte da posição atual, não da inicial
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float eased = Mathf.SmoothStep(0f, 1f, t); // suaviza início e fim
            rect.anchoredPosition = Vector2.Lerp(from, target, eased);
            yield return null;
        }

        rect.anchoredPosition = target; // garante a posição final exata
        animCoroutine = null;
    }
}