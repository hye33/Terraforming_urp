using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ItemObject : MonoBehaviour
{
    Sign sign;
    public void ShowSign(Collider2D collider)
    {
        SignClose signClose = collider.GetComponent<SignClose>();
        if(sign == null)
        {
            sign = Managers.UI.ShowPopupUI<Sign>("Sign");
            sign.ID = int.Parse(collider.name);
            sign.Check = signClose.Check;
            sign.Init();
        }
    }

    public void CloseSign()
    {
        Managers.UI.ClosePopupUI(sign);
    }
}
