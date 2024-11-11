using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Record : UI_Popup
{
    Dictionary<int, bool> getRecords;
    enum GameObjects
    {
        SelectedTMP,
        InforationTMP,
        SelectedImg,
        Content,
        Exit,
        Map,
        MonsterGuide
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
        getRecords = Managers.Game.SaveData.getRecord;

        int i = 0;
        foreach (KeyValuePair<int, bool> record in getRecords)
        {
            int ID = record.Key;
            GameObject current = GetObject((int)GameObjects.Content).transform.GetChild(i).gameObject;
            i++;

            var objectData = Managers.Data.ObjectDict[ID];
            Debug.Log(objectData);

            string path = "UI/RecordObjects/" + objectData.Image;
            current.transform.Find("Img").GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
            current.transform.Find("Text").GetComponent<Text>().text = objectData.Title;

            current.BindEvent((PointerEventData data) => ObjectSelect(data, objectData));
            i++;
        }
    }

    private void ObjectSelect(PointerEventData data, ObjectData objectData)
    {
        GetObject((int)GameObjects.SelectedTMP).GetComponent<TextMeshProUGUI>().text = objectData.Title;
        GetObject((int)GameObjects.InforationTMP).GetComponent<TextMeshProUGUI>().text = objectData.Explain;
        GetObject((int)GameObjects.SelectedImg).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/RecordObjects/" + objectData.Image);
    }

    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.Q:
                break;
            case Define.KeyEvent.E:
                break;
            case Define.KeyEvent.Esc:
                Close(null);
                break;
        }

    }
    private void Close(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
    }

}
