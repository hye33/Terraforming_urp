using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Record : UI_Popup
{
    Dictionary<int, bool> getRecords;
    GameObject prevSelected;
    enum GameObjects
    {
        SelectedTMP,
        InformationTMP,
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

            current.SetActive(true);
            var objectData = Managers.Data.ObjectDict[ID];

            string path = "UI/RecordObjects/" + objectData.Image;
            current.transform.Find("Img").GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
            current.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = objectData.Title;
            current.transform.Find("SelectedFrame").gameObject.SetActive(false);
            current.transform.Find("UnSelectedFrame").gameObject.SetActive(true);

            current.BindEvent((PointerEventData data) => ObjectSelect(data, objectData, current));
            Debug.Log(current.gameObject.name);
            i++;
        }
        GetObject((int)GameObjects.Exit).BindEvent(Close);
    }

    private void ObjectSelect(PointerEventData data, ObjectData objectData, GameObject current)
    {
        Debug.Log(objectData.Title);
        GetObject((int)GameObjects.SelectedTMP).GetComponent<TextMeshProUGUI>().text = objectData.Title;
        GetObject((int)GameObjects.InformationTMP).GetComponent<TextMeshProUGUI>().text = objectData.Explain;
        GetObject((int)GameObjects.SelectedImg).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/RecordObjects/" + objectData.Image);

        current.transform.Find("SelectedFrame").gameObject.SetActive(true);
        current.transform.Find("UnSelectedFrame").gameObject.SetActive(false);

        if(prevSelected != null && prevSelected != current)
        {
            prevSelected.transform.Find("SelectedFrame").gameObject.SetActive(false);
            prevSelected.transform.Find("UnSelectedFrame").gameObject.SetActive(true);
        }

        prevSelected = current;
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
        Debug.Log("Close Called in Ui_Record");
        Managers.UI.ClosePopupUI(this);
        Managers.Input.UIKeyAction -= InputKey;
        Time.timeScale = 1;
    }

}
