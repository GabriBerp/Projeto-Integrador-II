using UnityEngine;

public class MenuButtonScript : MonoBehaviour
{
    bool openUpMotherFucker = false;
    bool isConfigShowing = false;

    public void MenuShow(GameObject menu){
        openUpMotherFucker = !openUpMotherFucker;
        menu.SetActive(openUpMotherFucker);
    }

    public void ConfigShow(GameObject config)
    {
        isConfigShowing = !isConfigShowing;
        config.SetActive(isConfigShowing);
    }

    public void ForceCloseConfig(GameObject config)
    {
        isConfigShowing = false;
        config.SetActive(isConfigShowing);
    }
}
