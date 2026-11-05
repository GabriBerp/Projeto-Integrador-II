using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Money, Handwork, Trash }

public class ResourcePopupManager : MonoBehaviour
{
    [SerializeField] private ResourcePopup popupPrefab;
    [SerializeField] private RectTransform container;   // ancorado no canto superior direito
    [SerializeField] private float margin = 20f;
    [SerializeField] private float spacing = 10f;

    [Header("Histerese")]
    [Tooltip("O pop-up só some quando o recurso cair abaixo dessa fração do limite")]
    [Range(0.5f, 1f)] [SerializeField] private float hideBelow = 0.9f;

    private readonly List<ResourcePopup> active = new List<ResourcePopup>();

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        Check(ResourceType.Money,    gm.money,    gm.baseMoneyCap,    "Dinheiro no limite! Gaste antes de desperdiçar.");
        Check(ResourceType.Handwork, gm.handwork, gm.baseHandWorkCap, "Mão de obra no limite! Coloque ela pra trabalhar.");
        Check(ResourceType.Trash,    gm.trash,    gm.baseTrashCap,    "Lixo no limite! Hora de dar um destino a ele.");
    }

    void Check(ResourceType type, float value, float cap, string message)
    {
        ResourcePopup existing = Find(type);
        bool isFull = value >= cap - 0.01f;

        if (isFull && existing == null)
            Show(type, message);
        else if (existing != null && value < cap * hideBelow)
            Hide(existing);
    }

    ResourcePopup Find(ResourceType type)
    {
        foreach (var p in active)
            if (p.Type == type) return p;
        return null;
    }

    void Show(ResourceType type, string message)
    {
        var popup = Instantiate(popupPrefab, container);
        popup.Setup(type, message);

        // Nasce fora da tela (à direita), já na altura do slot dele
        float hiddenX = popup.Width;
        popup.SnapTo(new Vector2(hiddenX, -SlotY(active.Count)));

        active.Add(popup);
        Relayout();
    }

    void Hide(ResourcePopup popup)
    {
        active.Remove(popup);
        popup.Dismiss(popup.Width);   // sai deslizando pro lado
        Relayout();                   // os de baixo sobem
    }

    void Relayout()
    {
        for (int i = 0; i < active.Count; i++)
            active[i].SetTarget(new Vector2(-margin, -SlotY(i)));
    }

    float SlotY(int index)
    {
        float y = margin;
        for (int i = 0; i < index; i++)
            y += active[i].Height + spacing;
        return y;
    }
}