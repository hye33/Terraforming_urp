using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mucus : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(coDestroy());
    }

    private IEnumerator coDestroy()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        Color32 original = sr.color;
        for (int i = 255; i > 0; i -= 25)
        {
            sr.color = new Color32(original.r, original.g, original.b, (byte)i);
            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger!!!");
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Damaged(10);
        }
    }
}
