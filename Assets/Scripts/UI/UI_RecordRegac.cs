using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RecordRegacy : UI_Popup
{
    //private GameObject buttonPrefab;
    //int[] getRelics;
    Dictionary<int, bool> getRecords;

    //TextMeshProUGUI time;
    //TextMeshProUGUI location;
    //TextMeshProUGUI type;
    //TextMeshProUGUI used;
    TextMeshProUGUI explain;
    Image relicImg;

    /*
    enum GameObjects
    {
        Container,
        Stage1Btn,
        Stage2Btn,
        Stage3Btn,
        Time,
        Location,
        Type,
        Used,
        Explain,
        RelicImg
    }*/
    enum GameObjects
    {
        Content,
        RecordImg,
        NameTMP,
        ExplainTMP,
        ExitBtn,
        RecordBtn,
        MapBtn
    }

    public override void Init()
    {
        Time.timeScale = 0;
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;

        BindObject(typeof(GameObjects));
        //getRelics = Managers.Game.GetRelic;
        getRecords = Managers.Game.SaveData.getRecord;
        /*
        GameObject container = GetObject((int)GameObjects.Container);
        time = GetObject((int)GameObjects.Time).GetComponent<TextMeshProUGUI>();
        location = GetObject((int)GameObjects.Location).GetComponent<TextMeshProUGUI>();
        type = GetObject((int)GameObjects.Type).GetComponent<TextMeshProUGUI>();
        used = GetObject((int)GameObjects.Used).GetComponent<TextMeshProUGUI>();
        explain = GetObject((int)GameObjects.Explain).GetComponent<TextMeshProUGUI>();
        relicImg = GetObject((int)GameObjects.RelicImg).GetComponent<Image>();
        */
        GameObject container = GetObject((int)GameObjects.Content);

        //if (getRelics.Count == 0) return;

        int i = 0;
        //for(int i = 0; i < getRelics.Count; i++) // ���� �����ϰ� �ִ� Relics�� 
        foreach(KeyValuePair<int,bool> record in getRecords)
        {
            int ID = record.Key;
            GameObject current = container.transform.GetChild(i).gameObject;
            
            //GameObject current = GetObject((int)GameObjects.Container).transform.Find(i.ToString()).gameObject;
            current.SetActive(true);
            current.GetComponentInChildren<TextMeshProUGUI>().text = Managers.Data.ObjectDict[ID].Title;
            current.name = ID.ToString();

            if (record.Value)
            {
                current.transform.GetChild(1).gameObject.SetActive(true);
            }
            i++;
        }

        for (int k = 0; k < 6; k++) // container ���� �׸��
        {
            GameObject current = container.transform.GetChild(k).gameObject;
            current.BindEvent((data) => onClick(data, current));
            if(k >= i)
            {
                Debug.Log(k);
                current.SetActive(false);
            }
        }

        UI_ClickEventHandler currentButton = container.transform.GetChild(0).gameObject.GetComponent<UI_ClickEventHandler>();
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.button = PointerEventData.InputButton.Left;
        currentButton.OnPointerClick(data);
        EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
        
    }

    private void onClick(PointerEventData data, GameObject currentObject)
    {
        Debug.Log(currentObject.name);
        //int index = int.Parse(currentObject.name);
        int ID = int.Parse(currentObject.name);
        ObjectData objectData = Managers.Data.ObjectDict[ID];
        /*
        time.text = Time.deltaTime.ToString(); // �߰� �ð����� ����
        location.text = objectData.Location;
        type.text = objectData.Type;
        used.text = objectData.Used;
        explain.text = objectData.Explain;
        */
        string spritePath = "UI/Relic/" + (int.Parse(currentObject.name) - 1000) / 100;
        Sprite imageSprite = Resources.Load<Sprite>(spritePath);
        relicImg.sprite = imageSprite;

        currentObject.transform.GetChild(1).gameObject.SetActive(true);
        Managers.Game.SaveData.getRelic[ID] = true;
    }
    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.Esc:
                CloseUI();
                break;
        }
    }

    private void CloseUI()
    {
        Time.timeScale = 1;
        Managers.Input.UIKeyAction -= InputKey;
        Managers.UI.ClosePopupUI(this);
    }

    private void Start()
    {
        Init();
    }

}
