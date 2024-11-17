using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sign : UI_Popup
{
    public int ID;
    public bool Check;
    TextMeshProUGUI infoText;
    enum GameObjects
    {
        Record,
        MonsterGuide,
        ExitR,
        ExitM,
        Title,
        Image,
        Message,
        MText
    }

    public override void Init()
    {
        BindObject(typeof(GameObjects));
        infoText = GetObject((int)GameObjects.Title).GetComponent<TextMeshProUGUI>();
        var objectData = Managers.Data.ObjectDict[ID];

        string information = "";
        if(ID > 0)
        {
            if (!Managers.Game.SaveData.getRecord.ContainsKey(ID))
            {
                Managers.Game.SaveData.getRecord.Add(ID, false);
            }
            information = objectData.Title;
        }
        if (ID >= 2000)
        {
            Managers.Input.UIKeyAction = null;
            Managers.Input.UIKeyAction -= InputKey;
            Managers.Input.UIKeyAction += InputKey;
            Check = false;
        }

        infoText.text = information;
        GetObject((int)GameObjects.Image).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/RecordObjects/" + objectData.Image);

    }

    private void InputKey(Define.KeyEvent key)
    {
        Debug.Log(key);
        switch (key)
        {
            case Define.KeyEvent.W:
                if (Check)
                    SceneManager.LoadScene("ForestBoss"); // 보스 입장
                else 
                    Check = true;
                break;
        }
    }


    public void ShowMessage(string msg)
    {
        infoText.text = msg;
        StartCoroutine("closeMessage");
    }

    IEnumerator closeMessage()
    {
        yield return new WaitForSeconds(1.0f);
        Managers.UI.ClosePopupUI(this);
    }

    private void OnDestroy()
    {
        Managers.Input.UIKeyAction -= InputKey;
        Managers.UI.ClosePopupUI(this);
    }

}
