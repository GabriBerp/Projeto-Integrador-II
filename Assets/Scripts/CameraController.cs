using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float dragSpeed = 0.5f;
    [SerializeField] private string boundaryTag = "Boundary";

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    // Valores calculados dinamicamente com base nas tags
    private float minX, maxX, minY, maxY;

    void Start()
    {
        cam = Camera.main;
        CalculateBoundaries();
    }

    void Update()
    {
        HandleInput();
    }

    // Busca os objetos na cena com a Tag e define os limites de movimento da câmera
    private void CalculateBoundaries()
    {
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag(boundaryTag);

        if (boundaries.Length == 0)
        {
            Debug.LogWarning($"Nenhum objeto com a tag '{boundaryTag}' foi encontrado. A câmera está sem limites!");
            minX = minY = float.MinValue;
            maxX = maxY = float.MaxValue;
            return;
        }

        // Inicializa com os valores extremos
        minX = minY = float.MaxValue;
        maxX = maxY = float.MinValue;

        foreach (var boundary in boundaries)
        {
            Vector3 pos = boundary.transform.position;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }
    }

    private void HandleInput()
    {
        // Detecção Unificada: Funciona para Toque (Celular) e Clique (Mouse/Editor)
        bool inputDown = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        bool inputHeld = Input.GetMouseButton(0) || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary));
        bool inputUp = Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

        Vector3 currentScreenPos = Vector3.zero;

        // Captura a posição atual do ponteiro
        if (Input.touchCount > 0)
        {
            currentScreenPos = Input.GetTouch(0).position;
        }
        else
        {
            currentScreenPos = Input.mousePosition;
        }

        // Início do Arraste
        if (inputDown)
        {
            dragOrigin = cam.ScreenToWorldPoint(currentScreenPos);
            isDragging = true;
        }

        // Movimento durante o Arraste
        if (inputHeld && isDragging)
        {
            Vector3 currentWorldPos = cam.ScreenToWorldPoint(currentScreenPos);
            Vector3 difference = dragOrigin - currentWorldPos;

            // Calcula a nova posição desejada da câmera
            Vector3 targetPosition = transform.position + difference;

            // Aplica os limites físicos (Clamp) baseados nas posições dos objetos Boundary
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            // Mantém a posição Z original da câmera intacta
            targetPosition.z = transform.position.z;

            transform.position = targetPosition;
        }

        // Fim do Arraste
        if (inputUp)
        {
            isDragging = false;
        }
    }
}
