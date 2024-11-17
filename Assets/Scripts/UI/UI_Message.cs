using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Message : UI_Popup
{
    public bool Check;
    enum GameObjects
    {
        MText
    }

    public void ShowMessage(string message)
    {

        if (message == "2000")
        {
            message = "�ٽ� WŰ(��ȣ�ۿ� Ű)�� ���� ���η� �����մϴ�.";
            Managers.Input.UIKeyAction = null;
            Managers.Input.UIKeyAction -= InputKey;
            Managers.Input.UIKeyAction += InputKey;
            Check = false;
        }

        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.MText).GetComponent<TextMeshProUGUI>().text = message;

        StartCoroutine("CloseMessage");
    }

    IEnumerator CloseMessage()
    {
        yield return new WaitForSeconds(1.0f);
        Managers.UI.ClosePopupUI(this);
    }

    private void InputKey(Define.KeyEvent key)
    {
        Debug.Log(key);
        switch (key)
        {
            case Define.KeyEvent.W:
                if (Check)
                    SceneManager.LoadScene("ForestBoss"); // ���� ����
                else
                    Check = true;
                break;
        }
    }

    private void OnDestroy()
    {
        Managers.Input.UIKeyAction -= InputKey;
        Managers.UI.ClosePopupUI(this);
    }
}
