using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignClose : MonoBehaviour
{
    private bool _Check = false;
    public bool Check { get { return _Check; } }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log(collision.tag);
        if (collision.CompareTag("Player"))
        {
            _Check = true;
            GetComponentInParent<ItemObject>().CloseSign();
        }
    }
}
