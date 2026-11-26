using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PoloInputHandler : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask raycastMask = ~0;
    [SerializeField] private float maxDistance = 200f;
    [SerializeField] private float holdTime = 0.5f;
    [SerializeField] private float moveTolerancePixels = 20f; // se arrastar mais que isso, cancela

    private PoloBase pressedPolo;
    private bool pressedOnEmpty;   // toque começou fora de UI e fora de qualquer polo
    private bool pressActive;      // existe um toque válido em andamento
    private bool dragged;          // moveu além da tolerância
    private Vector2 pressStartPos;
    private float timer;
    private bool holdTriggered;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        var pointer = Pointer.current; // mouse, touch ou pen (o último usado)
        if (pointer == null) return;

        Vector2 pos = pointer.position.ReadValue();

        // 1) Começou a pressionar
        if (pointer.press.wasPressedThisFrame)
        {
            BeginPress(pos);
        }

        // 3) Soltou
        if (pointer.press.wasReleasedThisFrame)
        {
            EndPress(pos);
        }
    }

    private void BeginPress(Vector2 screenPos)
    {
        CancelPress();

        // Ignora toques em cima de UI (inclui o próprio InfoPanel)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        pressActive = true;
        pressedPolo = RaycastPolo(screenPos);
        pressedOnEmpty = pressedPolo == null;
        pressStartPos = screenPos;
        timer = 0f;
        holdTriggered = false;
        dragged = false;
    }

    private void EndPress(Vector2 screenPos)
    {
        if (pressActive && !dragged)
        {
            if (pressedPolo != null)
            {
                // Clique normal: não virou hold e soltou ainda em cima do mesmo polo
                if (!holdTriggered && RaycastPolo(screenPos) == pressedPolo)
                    pressedPolo.OnClick();
                    GameManager.Instance.SelectPolo(pressedPolo);
                    GameManager.Instance.OpenInfoPanel();
                    
            }
            else if (pressedOnEmpty)
            {
                // Toque em área vazia: fecha o InfoPanel se estiver aberto
                if (GameManager.Instance != null && GameManager.Instance.IsInfoPanelOpen)
                    GameManager.Instance.CloseInfoPanel();
            }
        }

        CancelPress();
    }

    private void CancelPress()
    {
        pressedPolo = null;
        pressedOnEmpty = false;
        pressActive = false;
        dragged = false;
        timer = 0f;
        holdTriggered = false;
    }

    private PoloBase RaycastPolo(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, raycastMask))
            return hit.collider.GetComponentInParent<PoloBase>();
        return null;
    }
}