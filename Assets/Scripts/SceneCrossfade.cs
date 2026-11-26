using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCrossfade : MonoBehaviour
{
    public static void Load(string sceneName, float duration)
    {
        var go = new GameObject("SceneCrossfade");
        // O MenuManager morre junto com a cena; a corrotina precisa viver num objeto que sobreviva
        DontDestroyOnLoad(go);
        var crossfade = go.AddComponent<SceneCrossfade>();
        crossfade.StartCoroutine(crossfade.Run(sceneName, Mathf.Max(duration, 0.01f)));
    }

    private IEnumerator Run(string sceneName, float duration)
    {
        // Só no fim do frame o backbuffer tem a imagem completa, incluindo UI em Screen Space Overlay
        yield return new WaitForEndOfFrame();
        Texture2D snapshot = ScreenCapture.CaptureScreenshotAsTexture();
        CanvasGroup group = BuildOverlay(snapshot);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;

        // Frame extra: garante que o Start() da nova cena rodou e a câmera já renderizou
        yield return null;

        for (float t = 0f; t < 1f; )
        {
            // O primeiro frame após o load costuma ter um pico de deltaTime que pularia o fade inteiro
            t += Mathf.Min(Time.unscaledDeltaTime, 0.05f) / duration;
            group.alpha = 1f - Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }

        Destroy(snapshot);
        Destroy(gameObject);
    }

    private CanvasGroup BuildOverlay(Texture2D snapshot)
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        // Bloqueia cliques na cena nova enquanto a transição acontece
        gameObject.AddComponent<GraphicRaycaster>();
        var group = gameObject.AddComponent<CanvasGroup>();

        var imageObject = new GameObject("Snapshot", typeof(RectTransform), typeof(RawImage));
        imageObject.transform.SetParent(transform, false);

        var rect = (RectTransform)imageObject.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;

        imageObject.GetComponent<RawImage>().texture = snapshot;
        return group;
    }
}