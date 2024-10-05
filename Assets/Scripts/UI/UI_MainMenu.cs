using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : UI_Scene
{
    private GameObject[] buttons;
    private GameObject[] Imgs;
    private TextMeshProUGUI[] TMPs;
    private int currentButtonIndex = -1;
    //private Color highlightColor = new Color32(249, 222, 110,255);
    //private Color nomalColor = new Color32(205, 197, 173,255);

    //private AudioClip _buttonAudio;
    enum GameObjects
    {
        StartBtn,
        SettingBtn,
        EndBtn,
        StartImg,
        SettingImg,
        EndImg
    }

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        //_buttonAudio = Resources.Load<AudioClip>("Sounds/Effect/cursor6");
        Time.timeScale = 1;
        //UI 카메라 붙이기 
        Camera uiCamera = GameObject.Find("UICamera")?.GetComponent<Camera>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;  // RenderMode를 ScreenSpaceCamera로 설정
        canvas.worldCamera = uiCamera;  // Render Camera에 uiCamera를 할당

        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.StartBtn).BindEvent(StartGame);
        GetObject((int)GameObjects.SettingBtn).BindEvent(Setting);
        GetObject((int)GameObjects.EndBtn).BindEvent(EndGame);
        Managers.Clear();
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;

        buttons = new GameObject[]
        {
            GetObject((int)GameObjects.StartBtn),
            GetObject((int)GameObjects.SettingBtn),
            GetObject((int)GameObjects.EndBtn)
        };
        Imgs = new GameObject[]
        {
            GetObject((int)GameObjects.StartImg),
            GetObject((int)GameObjects.SettingImg),
            GetObject((int)GameObjects.EndImg)
        };
        TMPs = new TextMeshProUGUI[]
        {
                GetObject((int)GameObjects.StartBtn).transform.GetComponentInChildren<TextMeshProUGUI>(),
                GetObject((int)GameObjects.SettingBtn).transform.GetComponentInChildren<TextMeshProUGUI>(),
                GetObject((int) GameObjects.EndBtn).transform.GetComponentInChildren < TextMeshProUGUI >()
        };
        for (int i = 0; i < Imgs.Length; i++)
        {
            Imgs[i].SetActive(false);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            var hoverHandler = buttons[i].AddComponent<UI_MouseHoverHandler>();
            int index = i;
            hoverHandler.OnHoverHandler = (eventData) =>
            {
                if (currentButtonIndex != index)
                {
                    MoveFocus(index - currentButtonIndex, ref currentButtonIndex, buttons, Imgs, TMPs, true);
                }
            };
        }
    }

    public void OnMouseHover(int index)
    {
        if (currentButtonIndex != index)
        {

            MoveFocus(index - currentButtonIndex, ref currentButtonIndex, buttons, Imgs, TMPs, true);
        }
    }
    private void StartGame(PointerEventData data)
    {
        Managers.Input.UIKeyAction -= InputKey;
        UI_Continue gameContinue = Managers.UI.ShowPopupUI<UI_Continue>("ContinueUI");
        gameContinue.UI_MainMenu = this;
        Managers.UI.SetCanvas(gameContinue.gameObject, true, CanvasScaler.ScreenMatchMode.Shrink);
    }
    private void Setting(PointerEventData data)
    {
        Managers.Input.UIKeyAction -= InputKey;
        UI_Setting setting = Managers.UI.ShowPopupUI<UI_Setting>("SettingUI");
        setting.UI_MainMenu = this;
        Managers.UI.SetCanvas(setting.gameObject, true, CanvasScaler.ScreenMatchMode.Shrink);
    }
    private void EndGame(PointerEventData data)
    {
        Managers.Input.UIKeyAction -= InputKey;
        UI_Exit exit = Managers.UI.ShowPopupUI<UI_Exit>("ExitUI");
        exit.UI_MainMenu = this;
        Managers.UI.SetCanvas(exit.gameObject, true, CanvasScaler.ScreenMatchMode.Shrink);
    }
    /*
    private void MoveFocus(int dir)
    {
        if (currentButtonIndex <= 0)
        {
            Imgs[0].SetActive(false);
            TMPs[0].color = nomalColor;
        }
        else if (currentButtonIndex >= (buttons.Length - 1))
        {
            Imgs[(buttons.Length - 1)].SetActive(false);
            TMPs[(buttons.Length - 1)].color = nomalColor;
        }
        else
        {
            Imgs[currentButtonIndex].SetActive(false);
            TMPs[currentButtonIndex].color = nomalColor;
        }
        if (currentButtonIndex < 1) currentButtonIndex += buttons.Length;
        currentButtonIndex = (currentButtonIndex + dir) % buttons.Length;

        EventSystem.current.SetSelectedGameObject(buttons[currentButtonIndex]);
        Imgs[currentButtonIndex].SetActive(true);
        TMPs[currentButtonIndex].color = highlightColor;
        Managers.Sound.Play(_buttonAudio, Define.Sound.Effect);
    }*/

    private void OnClickBtn()
    {
        Debug.Log("ONClickBtn");
        if (currentButtonIndex == -1) return;
        GameObject currentButtonObject = buttons[currentButtonIndex];
        if (currentButtonObject == null) return;
        UI_ClickEventHandler currentButton = currentButtonObject.GetComponent<UI_ClickEventHandler>();
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.button = PointerEventData.InputButton.Left;
        currentButton.OnPointerClick(data);
    }

    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.W:
                if (currentButtonIndex == -1) currentButtonIndex = 0;

                MoveFocus(-1, ref currentButtonIndex, buttons, Imgs, TMPs, true);
                break;

            case Define.KeyEvent.Space:
                OnClickBtn();
                break;

            case Define.KeyEvent.S:

                MoveFocus(1, ref currentButtonIndex, buttons, Imgs, TMPs, true);
                break;
        }
    }

    public void RegisterInputKeyAction()
    {
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;
    }

    public void OnDestroy()
    {
        Debug.Log("MainMenu On Destroy");
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.Clear();
    }
}
