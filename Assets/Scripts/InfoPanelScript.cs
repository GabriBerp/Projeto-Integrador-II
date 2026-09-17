using UnityEngine;

public class InfoPanelScript : MonoBehaviour
{
    Vector2 startPos;
    Vector2 endPos;
    public float offsetY;
    public bool state = false; // false - Escondido | true - Revelado

    private void Start() {
        startPos = GetComponent<RectTransform>().anchoredPosition;
        endPos = startPos;
        endPos.y += offsetY;
    }

    public void Panel_Click()
    {
        if (state)
        {
            GetComponent<RectTransform>().anchoredPosition = startPos;
            state = false;
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = endPos;
            state = true;
        }
    }
}
