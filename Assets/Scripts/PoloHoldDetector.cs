using UnityEngine;
using UnityEngine.EventSystems;

public class PoloHoldDetector : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private PoloBase polo;
    [SerializeField] private float holdTime = 0.5f;

    private bool pressing;
    private bool holdTriggered;
    private float timer;

    void Awake()
    {
        if (polo == null) polo = GetComponent<PoloBase>();
    }

    void Update()
    {
        if (!pressing || holdTriggered) return;

        timer += Time.unscaledDeltaTime;
        if (timer >= holdTime)
        {
            holdTriggered = true;
            GameManager.Instance.SelectPolo(polo);
            GameManager.Instance.OpenInfoPanel();
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        pressing = true;
        holdTriggered = false;
        timer = 0f;
    }

    public void OnPointerUp(PointerEventData e)
    {
        // Soltou antes do tempo = clique normal
        if (pressing && !holdTriggered) polo.OnClick();
        pressing = false;
    }

    public void OnPointerExit(PointerEventData e)
    {
        pressing = false; // saiu de cima: cancela
    }
}