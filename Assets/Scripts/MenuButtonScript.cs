using UnityEngine;

public class MenuButtonScript : MonoBehaviour
{
    public void MenuShow(GameObject menu){
        menu.SetActive(!menu.activeSelf);
    }
}
