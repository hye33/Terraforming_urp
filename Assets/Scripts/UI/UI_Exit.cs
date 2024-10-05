using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Exit : UI_Popup
{
    private GameObject[] buttons;
    private GameObject[] Imgs;
    private int currentButtonIndex = -1;
    private UI_MainMenu _mainMenu;
    public UI_MainMenu UI_MainMenu { set { _mainMenu = value; } }
    private UI_Pause _pause = null;
    public UI_Pause UI_Pause { set { _pause = value; } }
    enum GameObjects
    {
        Yes,
        No,
        YesImg,
        NoImg,
        ExitBtn
    }

    public override void Init()
    {
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;

        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.Yes).BindEvent(ExitGame);
        GetObject((int)GameObjects.No).BindEvent(Close);

        buttons = new GameObject[]
{
            GetObject((int)GameObjects.Yes),
            GetObject((int)GameObjects.No),
};
        Imgs = new GameObject[]
        {
            GetObject((int)GameObjects.YesImg),
            GetObject((int)GameObjects.NoImg),
        };
        for (int i = 0; i < Imgs.Length; i++)
        {
            Imgs[i].SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void ExitGame(PointerEventData data)
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
    private void Close(PointerEventData data)
    {
        CloseUI();
    }

    private void CloseUI()
    {
        if (_mainMenu != null)
        {
            _mainMenu.RegisterInputKeyAction();
        }

        Managers.Input.UIKeyAction -= InputKey;
        Managers.UI.ClosePopupUI(this);
        if (_pause != null)
        {
            _pause.SetActiveTrue();
        }
    }


    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.A:
                if (currentButtonIndex == -1) currentButtonIndex = 0;
                MoveFocus(-1, ref currentButtonIndex, buttons, Imgs);
                break;

            case Define.KeyEvent.Space:
                OnClickBtn();
                break;

            case Define.KeyEvent.D:
                MoveFocus(1, ref currentButtonIndex, buttons, Imgs);
                break;

            case Define.KeyEvent.Esc:
                CloseUI();
                break;
        }
    }
    /*
    private void MoveFocus(int dir)
    {
        if (currentButtonIndex <= 0) Imgs[0].SetActive(false);
        else if (currentButtonIndex >= (buttons.Length - 1)) Imgs[(buttons.Length - 1)].SetActive(false);
        else Imgs[currentButtonIndex].SetActive(false);
        if (currentButtonIndex < 1) currentButtonIndex += buttons.Length;
        currentButtonIndex = (currentButtonIndex + dir) % buttons.Length;
        EventSystem.current.SetSelectedGameObject(buttons[currentButtonIndex]);
        Imgs[currentButtonIndex].SetActive(true);
    }*/
    private void OnClickBtn()
    {
        if (currentButtonIndex == -1) return;
        GameObject currentButtonObject = buttons[currentButtonIndex];
        UI_ClickEventHandler currentButton = currentButtonObject.GetComponent<UI_ClickEventHandler>();
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.button = PointerEventData.InputButton.Left;
        currentButton.OnPointerClick(data);
    }
    private void OnDestroy()
    {
        Managers.Input.UIKeyAction -= InputKey;
    }

}
