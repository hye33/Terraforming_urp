using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ItemObject : MonoBehaviour
{
    Dictionary<int, bool> getRecords;
    Sign sign;
    UI_Message _message;
    private bool _check;

    private void Awake()
    {
        getRecords = Managers.Game.SaveData.getRecord;
    }
    public void ShowSign(Collider2D collider)
    {
        int key;

        Debug.Log("" + collider.name +" - " + _check);
        if(collider.name == "2000")
        {
            if(_message == null)
            {
                _message = Managers.UI.ShowPopupUI<UI_Message>("MessageUI");
                _message.ShowMessage("2000");
            }
        }
        else if (int.TryParse(collider.name, out key) && getRecords.ContainsKey(key))
        { return; }

        else if (sign == null)
        {
            Time.timeScale = 0;
            sign = Managers.UI.ShowPopupUI<Sign>("Sign");
            sign.ID = key;
            Managers.UI.SetCanvas(sign.gameObject);
            sign.Init();
            //sign.Check = signClose.Check;
            _check = true;
            if (key < 1999) sign.SetUI("Record");
            else if (key > 3000) sign.SetUI("MonsterGuide");
        }
    }

    public void CloseSign()
    {
        Managers.UI.ClosePopupUI(sign);
    }
}
