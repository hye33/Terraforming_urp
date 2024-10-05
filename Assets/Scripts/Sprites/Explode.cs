using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    private bool isPlayer;
    private int damage;
    private float range;
    private ParticleSystem particle;

    public void Init(bool isPlayer, int damage, float range)
    {
        particle = GetComponent<ParticleSystem>();

        this.isPlayer = isPlayer;
        this.damage = damage;  
        this.range = range;
    }

    public void Exploding()
    {
        particle.Play();
        Collider2D[] hits;
        if (isPlayer)
            hits = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask("Enemy"));
        else 
            hits = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask("Player"));
        if (hits.Length == 0)
            return;
        for (int i = hits.Length - 1; i >= 0; i--)
        {
            if (isPlayer)
                hits[i].GetComponent<Enemy>().Damaged(damage);
            else
                hits[i].GetComponent<PlayerController>().Damaged(damage);
        }
    }
}