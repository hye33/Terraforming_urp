using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBossBomb : MonoBehaviour
{
    private int _damage = 20;

    void Start()
    {
        StartCoroutine(BombCount(3.0f));
    }

    private IEnumerator BombCount(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        GetComponent<Animator>().SetTrigger("Bomb");
        yield return new WaitForSeconds(1.0f); // 폭발 애니메이션 출력 시간
        Bomb();
    }

    private void Bomb()
    {
        Collider2D hits = Physics2D.OverlapCircle(transform.position, 6.0f, LayerMask.GetMask("Player"));
        if (hits != null)
            hits.GetComponent<PlayerController>().Damaged(_damage);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Floor") || other.CompareTag("Platform"))
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
}
