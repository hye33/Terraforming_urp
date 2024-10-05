using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _speed;
    private int _power;
    private bool _isPlayer;
    private int _damage;
    private bool _destroyAtGround;
    private bool _hasTerm;
    private float _term;
    private Vector3 _dir;

    private Rigidbody2D rb;

    private void StatSetting()
    {
        _power = Managers.Data.Player.longDamage;
    }

    private void Init(bool isPlayer, int damage, float speed, bool destroyAtGround, Vector3 dir)
    {
        StatSetting();
        rb = GetComponent<Rigidbody2D>();
        _isPlayer = isPlayer;
        _damage = isPlayer ? Managers.Data.Player.longDamage : damage;
        _speed = isPlayer ? Stat.PLAYER_BULLET_SPEED : speed;
        _destroyAtGround = destroyAtGround;
        _dir = dir;
    }

    // 일반 총알
    public void SetBullet(
        Vector3 dir,
        Vector3 pos,
        bool isPlayer,
        int damage = 1,
        float speed = 0,
        bool destroyAtGround = false)
    {
        Init(isPlayer, damage, speed, destroyAtGround, dir);
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        transform.position = pos;
        rb.velocity = dir.normalized * _speed;
        Invoke("DestroyBullet", 0.6f);
    }

    // 멈췄다가 나가는 총알
    public void SetTermBullet(
        Vector3 dir,
        Vector3 pos,
        bool isPlayer,
        int damage = 1,
        float speed = 0,
        bool destroyAtGround = false,
        float term = 0.0f,
        float pauseTerm = 0.0f)
    {
        Init(isPlayer, damage, speed, destroyAtGround, dir);
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        transform.position = pos;
        rb.velocity = dir.normalized * _speed;
        StartCoroutine(ShootTermBullet(term, pauseTerm));
        Invoke("DestroyBullet", 3.0f);
    }

    private IEnumerator ShootTermBullet(float term, float pauseTerm)
    {
        yield return new WaitForSeconds(term);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(pauseTerm);
        rb.velocity = _dir.normalized * _speed;
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isPlayer && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.Type == Define.ForestEnemyType.Forest04)
            {
                enemy.Damaged(_power / 2);
                return;
            }
            other.GetComponent<Enemy>().Damaged(_damage);
            Destroy(gameObject);
        }
        if (!_isPlayer && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.Damaged(_damage);
            Destroy(gameObject);
        }
        if (_destroyAtGround && other.CompareTag("Floor"))
            Destroy(gameObject);
        if (_destroyAtGround && other.CompareTag("Ground"))
            Destroy(gameObject);
    }
}
