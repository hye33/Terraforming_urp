using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningSpace : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(disappear());
    }

    private IEnumerator disappear()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
