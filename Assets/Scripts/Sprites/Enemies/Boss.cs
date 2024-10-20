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

    protected void BaseInit()
    {
        _localSize = transform.localScale.y;
    }

    public void Damaged(int minus, GameObject attacker)
    {
        if (attackedAction != null)
            attackedAction.Invoke();
        // if (canAttacked == false)
        //     return;
        if (_hp <= 0)
        {
            Death();
            return;
        }
        DamagedAnim();
        _hp = Mathf.Clamp(_hp - minus, 0, _maxHp);
        Debug.Log("hp: " + _hp);
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
    }
}
