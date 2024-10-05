using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DeleteFile : UI_Popup
{
    private UI_Continue _continue;
    private GameObject[] buttons;
    private GameObject[] Imgs;
    private int currentButtonIndex = -1;
    private UI_Continue _uiContinue;
    public UI_Continue UIContinue { set { _uiContinue = value; } }

    private string text = "파일이 제거되었습니다.";
    private int _fileNumber;
    public int FileNumber { set { _fileNumber = value; } }
    private LoadFile _loadFileInstance;
    public LoadFile LoadFileInstance { set {_loadFileInstance = value;}}
    enum GameObjects{
        TMP,
        NoBtn,
        YesBtn,
        YesImg,
        NoImg
    }

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        _continue = FindObjectOfType<UI_Continue>();
        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.NoBtn).BindEvent(NoEvent);
        GetObject((int)GameObjects.YesBtn).BindEvent(YesEvent);

        buttons = new GameObject[]
{
            GetObject((int)GameObjects.YesBtn),
            GetObject((int)GameObjects.NoBtn),
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
        Managers.Input.Clear();
        Managers.Input.UIKeyAction += InputKey;
    }

    private void NoEvent(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
    }

    private void YesEvent(PointerEventData data)
    {
        Managers.Game.DeleteGameData(_fileNumber);
        _loadFileInstance.RefreshUI();
        GetObject((int)GameObjects.TMP).GetComponent<TextMeshProUGUI>().text = text;
        GetObject((int)GameObjects.NoBtn).SetActive(false);
        GetObject((int)GameObjects.YesBtn).SetActive(false);
        StartCoroutine("CloseLate");
    }

    IEnumerator CloseLate()
    {
        float elapsedTime = 0f;
        float waitTime = 0.8f;

        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Managers.UI.ClosePopupUI(this);
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
                NoEvent(null);
                break;
        }
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

    private void OnDestroy()
    {
        Managers.Input.UIKeyAction -= InputKey;
        if (_continue != null)
        {
            _continue.RegisterInputKeyAction();
        }
    }

}
