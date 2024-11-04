using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : SpineAnimHandler
{
    protected int _hp;
    protected int _maxHp;
    protected Rigidbody2D _rb;
    protected bool canAttacked = false;
    protected float _localSize;
    protected Action attackedAction;

    public bool _useUIHpBar;
    protected UI_HpBar _hpBar;
    protected HpBar_Sprite _hpBar_sprite;

    protected void BaseInit()
    {
        _localSize = transform.localScale.y;
        if (_useUIHpBar)
        {
            _hpBar = FindObjectOfType<UI_HpBar>();
        }
        else
        {
            _hpBar_sprite = FindObjectOfType<HpBar_Sprite>();
            _hpBar_sprite.gameObject.SetActive(false);
        }
    }

    public void Damaged(int minus, GameObject attacker)
    {
        if (attackedAction != null)
            attackedAction.Invoke();
        _hp = Mathf.Clamp(_hp - minus, 0, _maxHp);
        if (_useUIHpBar)
        {
            _hpBar.updateHpBar(_hp, _maxHp);
        }
        else
        {
            _hpBar_sprite.gameObject.SetActive(true);
            _hpBar_sprite.updateHpBar(_hp, _maxHp);
        }
        // if (canAttacked == false)
        //     return;
        if (_hp <= 0)
        {
            Death();
            return;
        }
        DamagedAnim();
        Debug.Log("hp: " + _hp + "/" + _maxHp);
        // KnockBack(attacker.transform.position);
    }

    abstract public void Death();

    public void KnockBack(Vector3 attackPos, float force = 5.0f)
    {
        Debug.Log("boss knockback");
        _rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - attackPos.x) * force, 0), ForceMode2D.Impulse);
    }

    protected void FlipSprite(float dir)
    {
        transform.localScale = new Vector3(-dir * _localSize, _localSize, 1);
        if (!_useUIHpBar)
            _hpBar_sprite.transform.localScale = new Vector3(-dir, 1, 1);
    }
}
