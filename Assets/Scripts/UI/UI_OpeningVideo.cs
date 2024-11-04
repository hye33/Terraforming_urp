using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_OpeningVideo : UI_Popup
{
    public GameObject skipBtn;

    private void Start()
    {
        skipBtn.BindEvent(GameStart);
        Invoke("GameStart", 25.0f);
    }

    public void GameStart(PointerEventData data = null)
    {
        SceneManager.LoadScene("Forest");
    }
}
