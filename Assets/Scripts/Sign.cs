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
        signCanvas,
        Information
    }

    public override void Init()
    {
        BindObject(typeof(GameObjects));
        infoText = GetObject((int)GameObjects.Information).GetComponent<TextMeshProUGUI>();

        string information = "";
        if(ID > 0)
        {
            if (!Managers.Game.SaveData.getRecord.ContainsKey(ID))
            {
                Managers.Game.SaveData.getRecord.Add(ID, false);
            }
            information = Managers.Data.ObjectDict[ID].Information;
        }
        if (ID >= 2000)
        {
            Managers.Input.UIKeyAction = null;
            Managers.Input.UIKeyAction -= InputKey;
            Managers.Input.UIKeyAction += InputKey;
            Check = false;
        }
        else if (ID >= 1100 && !Check)
        {
            if (!Managers.Game.SaveData.getRelic.ContainsKey(ID))
            {
                Managers.Game.SaveData.getRelic.Add(ID, false);
            }
            information += "\n무엇인가 빛나는 것을 얻었다.";
        }
        infoText.text = information;
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
