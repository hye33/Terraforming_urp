using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRange : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Damaged(damage, gameObject);
        }
    }
}