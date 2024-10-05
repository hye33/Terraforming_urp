using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTrigger : MonoBehaviour
{
    [SerializeField] GameObject info;
    private GameObject _player;
    private GameObject infoObject;
    Vector3 _infoPos = new Vector3(0, 4.0f, 0);

    private bool isShowing = false;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>().gameObject;
    }

    private void ShowInfoIcon()
    {
        if (isShowing)
            return;
        isShowing = true;
        infoObject = Instantiate(info);
        infoObject.transform.position += _infoPos;
        StartCoroutine(coShowingInfoIcon());
    }

    private IEnumerator coShowingInfoIcon()
    {
        while (isShowing)
        {
            infoObject.transform.position = _player.transform.position + _infoPos;
            yield return null;
        }
    }

    private void CloseInfoIcon()
    {
        if(!isShowing)
            return;
        isShowing = false;
        if (infoObject == null)
            return;
        Destroy(infoObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        ShowInfoIcon();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        CloseInfoIcon();
    }
}
