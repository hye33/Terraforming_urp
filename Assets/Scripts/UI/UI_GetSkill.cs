using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GetSkill : UI_Popup
{
    private GameObject[] buttons;
    private int currentButtonIndex = 0;
    // Start is called before the first frame update
    enum GameObjects
    {
        SkillA,
        SkillB
    }

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        Time.timeScale = 0;
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;

        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.SkillA).BindEvent(SelectSkill);
        GetObject((int)GameObjects.SkillB).BindEvent(SelectSkill);
        buttons = new GameObject[]
        {
            GetObject((int)GameObjects.SkillA),
            GetObject((int)GameObjects.SkillB)
        };
    }

    private void SelectSkill(PointerEventData data)
    {
        if (currentButtonIndex == 0)
        {
            Debug.Log("get Skill A");
            Managers.Game.SaveData.Enhancement[0] += 1;
        }
        else Debug.Log("get Skill B");
        ClosePopup();
    }

    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.A:
                Focus(0);
                break;

            case Define.KeyEvent.D:
                Focus(1);
                break;

            case Define.KeyEvent.Space:
                OnClickBtn();
                break;
        }
    }
    private void Focus(int dir)
    {
        buttons[currentButtonIndex].transform.localScale = Vector3.one;
        buttons[dir].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        EventSystem.current.SetSelectedGameObject(buttons[dir]);
        currentButtonIndex = dir;
    }

    private void OnClickBtn()
    {
        GameObject currentButtonObject = buttons[currentButtonIndex];
        UI_ClickEventHandler currentButton = currentButtonObject.GetComponent<UI_ClickEventHandler>();
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.button = PointerEventData.InputButton.Left;
        currentButton.OnPointerClick(data);
    }
    private void ClosePopup()
    {
        Time.timeScale = 1;
        Managers.UI.ClosePopupUI(this);
        Managers.Input.UIKeyAction -= InputKey;
    }

}
