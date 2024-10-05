using UnityEngine;

public class MainScene : MonoBehaviour
{
    private void Start()
    {
        UI_MainMenu mainMenuUI = Managers.UI.ShowSceneUI<UI_MainMenu>("MainMenuUI");
        //Managers.UI.SetCanvas(mainMenuUI.gameObject);
    }
}
