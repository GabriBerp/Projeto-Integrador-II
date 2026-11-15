using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private string boundaryTag = "Boundary";
    [SerializeField] private float groundHeight = 0f; // altura (Y) do chão

    [Header("Configurações de Zoom")]
    // Perspectiva: altura da câmera acima do chão. Ortográfica: orthographicSize.
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;
    [SerializeField] private float scrollStep = 0.1f;       // 10% por "clique" da roda
    [SerializeField] private float pinchSensitivity = 1f;   // >1 deixa a pinça mais agressiva

    private Camera cam;
    private Plane groundPlane;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    private float minX, maxX, minZ, maxZ;

    void Start()
    {
        cam = Camera.main;
        groundPlane = new Plane(Vector3.up, new Vector3(0f, groundHeight, 0f));
        CalculateBoundaries();
    }

    void Update()
    {
        // Durante a pinça o arrasto é cancelado para a câmera não "pular" ao soltar um dedo
        if (HandlePinch())
        {
            isDragging = false;
            return;
        }

        HandleScroll();
        HandleDrag();
    }

    private void CalculateBoundaries()
    {
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag(boundaryTag);

        if (boundaries.Length == 0)
        {
            Debug.LogWarning($"Nenhum objeto com a tag '{boundaryTag}' foi encontrado. A câmera está sem limites!");
            minX = minZ = float.MinValue;
            maxX = maxZ = float.MaxValue;
            return;
        }

        minX = minZ = float.MaxValue;
        maxX = maxZ = float.MinValue;

        foreach (var boundary in boundaries)
        {
            Vector3 pos = boundary.transform.position;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.z < minZ) minZ = pos.z;
            if (pos.z > maxZ) maxZ = pos.z;
        }
    }

    private Vector3 ClampToBounds(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        return pos;
    }

    // Ponto do chão que está sob o dedo/mouse
    private bool TryGetGroundPoint(Vector2 screenPos, out Vector3 point)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (groundPlane.Raycast(ray, out float distance))
        {
            point = ray.GetPoint(distance);
            return true;
        }
        point = Vector3.zero;
        return false;
    }

    // factor < 1 aproxima, factor > 1 afasta
    private void ApplyZoom(Vector2 screenPos, float factor)
    {
        if (!TryGetGroundPoint(screenPos, out Vector3 anchor)) return;

        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize * factor, minZoom, maxZoom);

            // O size mudou, então o ponto sob o cursor deslizou; compensa movendo a câmera
            if (TryGetGroundPoint(screenPos, out Vector3 after))
                transform.position = ClampToBounds(transform.position + (anchor - after));
            return;
        }

        float height = transform.position.y - groundHeight;
        if (height <= 0.01f) return;

        float newHeight = Mathf.Clamp(height * factor, minZoom, maxZoom);
        float scale = newHeight / height;

        // Escalar o vetor âncora->câmera mantém o ponto sob o cursor fixo na tela
        transform.position = ClampToBounds(anchor + (transform.position - anchor) * scale);
    }

    private void HandleScroll()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Approximately(scroll, 0f)) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // Só o sinal importa: o valor bruto da roda varia entre plataformas (1, 120...)
        ApplyZoom(mouse.position.ReadValue(), 1f - Mathf.Sign(scroll) * scrollStep);
    }

    // Retorna true enquanto houver dois dedos na tela
    private bool HandlePinch()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null) return false;

        TouchControl a = null, b = null;
        foreach (var t in touchscreen.touches)
        {
            if (!t.isInProgress) continue;
            if (a == null) a = t;
            else { b = t; break; }
        }
        if (b == null) return false;

        Vector2 posA = a.position.ReadValue();
        Vector2 posB = b.position.ReadValue();
        Vector2 prevA = posA - a.delta.ReadValue();
        Vector2 prevB = posB - b.delta.ReadValue();

        float prevDist = Vector2.Distance(prevA, prevB);
        float currDist = Vector2.Distance(posA, posB);

        if (prevDist > 0.01f && currDist > 0.01f)
        {
            // Dedos se afastando (currDist > prevDist) => factor < 1 => aproxima
            float factor = Mathf.Pow(prevDist / currDist, pinchSensitivity);
            ApplyZoom((posA + posB) * 0.5f, factor);
        }

        return true;
    }

    private void HandleDrag()
    {
        var pointer = Pointer.current; // mouse, toque ou caneta
        if (pointer == null)
        {
            isDragging = false;
            return;
        }

        Vector2 screenPos = pointer.position.ReadValue();

        // 1) Começou a tocar/clicar
        if (pointer.press.wasPressedThisFrame)
        {
            // Não arrasta a câmera se o toque começou em cima de UI
            bool overUI = EventSystem.current != null &&
                          EventSystem.current.IsPointerOverGameObject();

            if (!overUI && TryGetGroundPoint(screenPos, out dragOrigin))
                isDragging = true;
        }

        // 2) Soltou (ou perdeu o ponteiro por qualquer motivo)
        if (!pointer.press.isPressed)
        {
            isDragging = false;
            return;
        }

        // 3) Arrastando
        if (isDragging && TryGetGroundPoint(screenPos, out Vector3 currentWorldPos))
        {
            Vector3 difference = dragOrigin - currentWorldPos;
            difference.y = 0f; // nunca muda a altura da câmera

            transform.position = ClampToBounds(transform.position + difference);
        }
    }
}
