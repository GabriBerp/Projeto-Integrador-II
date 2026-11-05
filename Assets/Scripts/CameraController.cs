using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private string boundaryTag = "Boundary";
    [SerializeField] private float groundHeight = 0f; // altura (Y) do chão

    private Camera cam;
    private Plane groundPlane;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    // Agora os limites são em X e Z (o chão)
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
        bool inputDown = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        bool inputHeld = Input.GetMouseButton(0) || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary));
        bool inputUp = Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

        Vector2 currentScreenPos = Input.touchCount > 0
            ? Input.GetTouch(0).position
            : (Vector2)Input.mousePosition;

        if (inputDown)
        {
            if (TryGetGroundPoint(currentScreenPos, out dragOrigin))
                isDragging = true;
        }

        if (inputHeld && isDragging)
        {
            if (TryGetGroundPoint(currentScreenPos, out Vector3 currentWorldPos))
            {
                Vector3 difference = dragOrigin - currentWorldPos;
                difference.y = 0f; // nunca muda a altura da câmera

                Vector3 targetPosition = transform.position + difference;
                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);

                transform.position = targetPosition;
            }
        }

        if (inputUp)
        {
            isDragging = false;
        }
    }
}