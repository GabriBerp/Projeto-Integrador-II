using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private string boundaryTag = "Boundary";
    [SerializeField] private float groundHeight = 0f; // altura (Y) do chão

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
        HandleInput();
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

    private void HandleInput()
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

            Vector3 targetPosition = transform.position + difference;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);

            transform.position = targetPosition;
        }
    }
}