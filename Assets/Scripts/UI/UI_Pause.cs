using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Pause : UI_Popup
{/*
    private bool isPause = false;
    private GameObject[] buttons;
    private GameObject[] Imgs;
    private int currentButtonIndex = -1;
    private Slider swordGuage;
    private Slider gunGuage;

    // Start is called before the first frame update
    enum GameObjects
    {
        ContinueBtn,
        SettingBtn,
        HelpBtn,
        ExitBtn,
        ContinueImg,
        SettingImg,
        HelpImg,
        ExitImg,
        SwordGuage,
        GunGuage,
        GetObjectTMP,
    }

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;

        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.ContinueBtn).BindEvent(Continue);
        GetObject((int)GameObjects.SettingBtn).BindEvent(Setting);
        GetObject((int)GameObjects.HelpBtn).BindEvent(Help);
        GetObject((int)GameObjects.ExitBtn).BindEvent(Exit);

        buttons = new GameObject[]
        {
            GetObject((int)GameObjects.ContinueBtn),
            GetObject((int)GameObjects.HelpBtn),
            GetObject((int)GameObjects.SettingBtn),
            GetObject((int)GameObjects.ExitBtn)
        };
        Imgs = new GameObject[]
        {
            GetObject((int)GameObjects.ContinueImg),
            GetObject((int)GameObjects.HelpImg),
            GetObject((int)GameObjects.SettingImg),
            GetObject((int)GameObjects.ExitImg)
        };
        for (int i = 0; i < Imgs.Length; i++)
        {
            Imgs[i].SetActive(false);
        }
        swordGuage = GetObject((int)GameObjects.SwordGuage).GetComponent<Slider>();
        gunGuage = GetObject((int)GameObjects.GunGuage).GetComponent<Slider>();

        swordGuage.value = (Managers.Game.SaveData.Enhancement[0] / 9.0f);
        gunGuage.value = (Managers.Game.SaveData.Enhancement[1] / 9.0f);

        GetObject((int)GameObjects.GetObjectTMP).GetComponent<TextMeshProUGUI>().text = Managers.Game.SaveData.getRelic.Count.ToString();
        //EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    private void Continue(PointerEventData data)
    {
        ClosePopup();
    }

    private void Setting(PointerEventData data)
    {
        UI_Setting setting = Managers.UI.ShowPopupUI<UI_Setting>("SettingUI");
        Managers.UI.SetCanvas(setting.gameObject, true, CanvasScaler.ScreenMatchMode.Shrink);
    }

    private void Help(PointerEventData data)
    {

    }

    private void Exit(PointerEventData data)
    {
        //StartCoroutine("MainMenu");
        Managers.Resource.Instantiate("UI/BlackScreen");
        ClosePopup();
        SceneManager.LoadScene("Main");

    }

    private void MoveFocus(int dir)
    {
        if (currentButtonIndex <= 0) Imgs[0].SetActive(false);
        else if (currentButtonIndex >= (buttons.Length - 1)) Imgs[(buttons.Length - 1)].SetActive(false);
        else Imgs[currentButtonIndex].SetActive(false);
        if (currentButtonIndex < 1) currentButtonIndex += buttons.Length;
        currentButtonIndex = (currentButtonIndex + dir) % buttons.Length;
        EventSystem.current.SetSelectedGameObject(buttons[currentButtonIndex]); 
        Imgs[currentButtonIndex].SetActive(true);
    }

    private void OnClickBtn()
    {
        if (currentButtonIndex == -1) return;
        GameObject currentButtonObject = buttons[currentButtonIndex];
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
                MoveFocus(-1);
                break;

            case Define.KeyEvent.Space:
                OnClickBtn();
                break;

            case Define.KeyEvent.S:
                MoveFocus(1);
                break;
        }
    }
    private void OnDestroy()
    {
        Managers.Input.UIKeyAction -= InputKey;
        Time.timeScale = 1;
    }
    private void ClosePopup()
    {
        Managers.UI.ClosePopupUI(this);
    }*/
    private GameObject[] buttons;
    private GameObject[] Imgs;
    private TextMeshProUGUI[] TMPs;
    private int currentButtonIndex = -1;
    //private AudioClip _buttonAudio;

    //private Color highlightColor = new Color32(249, 222, 110, 255);
    //private Color nomalColor = new Color32(205, 197, 173, 255);
    enum GameObjects
    {
        StartBtn,
        SettingBtn,
        EndBtn,
        StartImg,
        SettingImg,
        EndImg,
    }
    private void Awake()
    {
        Init();
    }
    public override void Init()
    {
        //_buttonAudio = Resources.Load<AudioClip>("Sounds/Effect/cursor6");
        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.StartBtn).BindEvent(StartGame);
        GetObject((int)GameObjects.SettingBtn).BindEvent(Setting);
        GetObject((int)GameObjects.EndBtn).BindEvent(EndGame);

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
        }; for (int i = 0; i < Imgs.Length; i++)
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
                    //MoveFocus();
                    MoveFocus(index - currentButtonIndex, ref currentButtonIndex, buttons, Imgs, TMPs, true);
                }
            };
        }
    }
    public void OnMouseHover(int index)
    {
        if (currentButtonIndex != index)
        {
            //MoveFocus(index - currentButtonIndex);
            MoveFocus(index - currentButtonIndex, ref currentButtonIndex, buttons, Imgs, TMPs, true);
        }
    }
    private void StartGame(PointerEventData data)
    {
        ClosePopup();

    }
    private void Setting(PointerEventData data)
    {
        Managers.Input.UIKeyAction -= InputKey;
        this.gameObject.SetActive(false);
        UI_Setting setting = Managers.UI.ShowPopupUI<UI_Setting>("SettingUI");
        setting.UI_Pause = this;
        Managers.UI.SetCanvas(setting.gameObject, true, CanvasScaler.ScreenMatchMode.Shrink);
    }
    private void EndGame(PointerEventData data)
    {/*
        Managers.Input.UIKeyAction -= InputKey;

        this.gameObject.SetActive(false);
        UI_Exit exit = Managers.UI.ShowPopupUI<UI_Exit>("ExitUI");
        exit.UI_Pause = this;
        Managers.UI.SetCanvas(exit.gameObject, true, CanvasScaler.ScreenMatchMode.Shrink);*/
        //StartCoroutine("MainMenu");
        Managers.Resource.Instantiate("UI/BlackScreen");
        ClosePopup();
        SceneManager.LoadScene("Main");

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
        if (currentButtonIndex == -1) return;
        GameObject currentButtonObject = buttons[currentButtonIndex];
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
                //MoveFocus(-1);
                MoveFocus(-1, ref currentButtonIndex, buttons, Imgs, TMPs, true);
                break;

            case Define.KeyEvent.Space:
                OnClickBtn();
                break;

            case Define.KeyEvent.S:
                //MoveFocus(1);
                MoveFocus(1, ref currentButtonIndex, buttons, Imgs, TMPs, true);
                break;
        }
    }
    private void OnDestroy()
    {
        Managers.Input.UIKeyAction -= InputKey;
        Time.timeScale = 1;
    }
    private void ClosePopup()
    {
        Managers.UI.ClosePopupUI(this); 
        Managers.Input.UIKeyAction -= InputKey;
        Time.timeScale = 1;
    }

    public void SetActiveTrue()
    {
        gameObject.SetActive(true);
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;
    }
}
