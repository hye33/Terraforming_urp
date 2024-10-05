using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public bool isSwing = false;

    private int _power;
    private float _scale;
    private float ATTACK_TERM = 0.5f;
    private Rigidbody2D rb;

    // audio 볼륨 조절 추가해야 함
    private AudioSource _audio;

    private GameObject _effectPrefab;
    private Vector3 _effectOffset = new Vector3(0.4f, -0.5f, 0);

    private void StatSetting()
    {
        _power = Managers.Data.Player.closeDamage;
        _scale = Stat.PLAYER_SWORD_RANGE;
    }

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
        _effectPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Player/PlayerSwingEffect");
        StatSetting();
    }

    public void Swing()
    {
        isSwing = true;
        _audio.Play();
        GameObject go = Instantiate(_effectPrefab, transform);
        go.transform.position += _effectOffset;
        StartCoroutine(coDestory(go, 0.8f));
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            transform.position,
            new Vector3(_scale, _scale, 1),
            0,
            LayerMask.GetMask("Enemy"));
        if (colliders.Length == 0)
        {
            colliders = Physics2D.OverlapBoxAll(
                transform.position,
                new Vector3(_scale, _scale, 1),
                0,
                LayerMask.GetMask("Boss"));
            StartCoroutine(coSwingToBoss(colliders));
        }
        else
            StartCoroutine(coSwingToEnemy(colliders));
    }

    private IEnumerator coDestory(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(go);
    }

    private IEnumerator coSwingToEnemy(Collider2D[] target)
    {
        if (target != null && target.Length > 0)
        {
            for (int i = target.Length - 1; i >= 0; i--)
            {
                Enemy enemy = target[i].GetComponent<Enemy>();
                yield return new WaitForSeconds(0.3f);
                enemy.Damaged(_power, gameObject);
            }
        }
        yield return new WaitForSeconds(ATTACK_TERM);
        isSwing = false;
    }


    private IEnumerator coSwingToBoss(Collider2D[] target)
    {
        if (target != null && target.Length > 0)
        {
            for (int i = target.Length - 1; i >= 0; i--)
            {
                Boss boss = target[i].GetComponent<Boss>();
                yield return new WaitForSeconds(0.3f);
                boss.Damaged(_power, gameObject);
            }
        }
        yield return new WaitForSeconds(ATTACK_TERM);
        isSwing = false;
    }
}
