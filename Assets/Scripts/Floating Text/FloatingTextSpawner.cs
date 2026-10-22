using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance;

    [SerializeField] private FloatingText prefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private Canvas canvas;

    private Camera UICamera =>
        canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

    private void Awake() => Instance = this;

    // Posição em pixels da tela (ex: Input.mousePosition)
    public void SpawnAtScreenPoint(double amount, Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            container, screenPoint, UICamera, out Vector2 localPos);

        FloatingText ft = Instantiate(prefab, container);
        ((RectTransform)ft.transform).anchoredPosition = localPos;
        ft.Show("+$" + NumberFormatter.Format(amount), Color.yellow);
    }

    // Em cima de um objeto de UI (botão, prédio feito com RectTransform)
    public void SpawnAtUI(double amount, RectTransform target)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(UICamera, target.position);
        SpawnAtScreenPoint(amount, screenPoint);
    }

    // Em cima de um objeto do mundo (SpriteRenderer, 3D)
    public void SpawnAtWorld(double amount, Vector3 worldPosition)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
        SpawnAtScreenPoint(amount, screenPoint);
    }
}