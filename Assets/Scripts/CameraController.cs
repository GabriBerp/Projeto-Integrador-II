using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private string boundaryTag = "Boundary";
    [SerializeField] private float groundHeight = 0f;

    [Header("Diagnóstico")]
    [Tooltip("Loga no Console o nome do objeto de UI que bloqueou o arrasto.")]
    [SerializeField] private bool logUIBlocking = false;

    private Camera cam;
    private Plane groundPlane;
    private Vector3 dragOrigin;
    private bool isDragging;
    // Fica true até soltar todos os dedos/botão: impede que um gesto iniciado
    // sobre UI, ou o dedo que sobrou depois de dois dedos na tela, comece a arrastar a câmera
    private bool dragBlocked;

    private float minX, maxX, minZ, maxZ;

    private readonly List<TouchControl> activeTouches = new List<TouchControl>(4);
    private readonly List<RaycastResult> uiHits = new List<RaycastResult>(8);
    private PointerEventData uiPointer;

    void Start()
    {
        cam = Camera.main;
        groundPlane = new Plane(Vector3.up, new Vector3(0f, groundHeight, 0f));
        CalculateBoundaries();
    }

    void OnDisable() => ResetDrag();

    // Ao perder o foco (ex: clicar no Editor) o release do mouse pode nunca chegar
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) ResetDrag();
    }

    void Update()
    {
        CollectActiveTouches();

        if (activeTouches.Count >= 2)
        {
            isDragging = false;
            dragBlocked = true;
            return;
        }

        if (activeTouches.Count == 1)
        {
            UpdateDrag(true, activeTouches[0].position.ReadValue());
            return;
        }

        var mouse = Mouse.current;
        if (mouse == null)
        {
            UpdateDrag(false, Vector2.zero);
            return;
        }

        UpdateDrag(mouse.leftButton.isPressed, mouse.position.ReadValue());
    }

    private void ResetDrag()
    {
        isDragging = false;
        dragBlocked = false;
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

    private void CollectActiveTouches()
    {
        activeTouches.Clear();
        var touchscreen = Touchscreen.current;
        if (touchscreen == null) return;

        // Os índices de touches[] são slots, não a ordem dos dedos,
        // por isso filtramos pelos toques realmente ativos
        foreach (var t in touchscreen.touches)
            if (t.isInProgress) activeTouches.Add(t);
    }

    // Raycast direto na UI: não depende do ID do ponteiro nem do tipo de input module
    private bool IsOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        if (uiPointer == null || uiPointer.currentInputModule != EventSystem.current.currentInputModule)
            uiPointer = new PointerEventData(EventSystem.current);

        uiPointer.position = screenPos;
        uiHits.Clear();
        EventSystem.current.RaycastAll(uiPointer, uiHits);

        if (uiHits.Count == 0) return false;

        if (logUIBlocking)
            Debug.Log($"Input bloqueado pela UI: {uiHits[0].gameObject.name}", uiHits[0].gameObject);
        return true;
    }

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

    private void UpdateDrag(bool pressed, Vector2 screenPos)
    {
        if (!pressed)
        {
            ResetDrag();
            return;
        }

        if (dragBlocked) return;

        if (!isDragging)
        {
            if (IsOverUI(screenPos) || !TryGetGroundPoint(screenPos, out dragOrigin))
            {
                dragBlocked = true;
                return;
            }
            isDragging = true;
            return;
        }

        if (!TryGetGroundPoint(screenPos, out Vector3 currentWorldPos)) return;

        Vector3 difference = dragOrigin - currentWorldPos;
        difference.y = 0f;

        Vector3 targetPosition = transform.position + difference;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);

        transform.position = targetPosition;
    }
}