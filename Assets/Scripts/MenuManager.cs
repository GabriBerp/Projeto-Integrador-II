using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private float transitionDuration = 1f;

    private bool loading;

    public void Play()
    {
        if (loading) return;
        loading = true;
        SceneCrossfade.Load(gameSceneName, transitionDuration);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}