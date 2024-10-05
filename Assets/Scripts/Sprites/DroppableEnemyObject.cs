using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppableEnemyObject : MonoBehaviour
{
    private float moveSpeed = 6.0f;
    private float shootPower = 1.5f;
    [SerializeField] private int _damage = 20;
    private int _onGroundDamage = 5;

    private Rigidbody2D rb;

    private bool _aboveGround = true;
    public float _lifeTime = 1.5f; // 땅에 닿은 후

    private void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        // _damage = Managers.Data.EnemyDict[1002].damage
    }

    // 특정 방향으로 발사할 경우
    public void ShootObject(Vector2 dir)
    {
        Init();
        rb.AddForce(dir * shootPower, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.Damaged(_aboveGround ? _damage : _onGroundDamage);
            Destroy(gameObject);
        }
        if (other.CompareTag("Floor") || other.CompareTag("Platform"))
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            StartCoroutine(coDestroy());
        }
    }

    private IEnumerator coDestroy()
    {
        yield return new WaitForSecondsRealtime(_lifeTime);
        Destroy(gameObject);
    } 
}
