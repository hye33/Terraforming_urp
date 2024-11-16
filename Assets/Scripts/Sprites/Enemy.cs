using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : SpineAnimHandler
{
    protected bool isAttacking = false;
    [SerializeField]
    protected Vector3 _spawnPos;
    protected float _localSize;

    protected enum EnemyAnimEnum { Idle, Walk, Hit, Die, Extra1, Extra2 }

    [SerializeField]
    protected Define.ForestEnemyType _type;
    public Define.ForestEnemyType Type { get => _type; }
    [SerializeField]
    protected Define.EnemyState _state;
    public Define.EnemyState State { get => _state; set {_state = value;}}
    [SerializeField]
    protected int idx;

    protected enum AnimState { Idle, Walk, Hit, Die }
    protected AnimState _currentAnimState;

    private GameObject _player;
    protected GameObject Player { get => _player; set => _player = value; }
    protected Vector3 _playerPos;

    private int _touchDamage;

    private float _trackRange;
    private float _attackRange;
    private float _attackRangeOffset = 0.5f; // 공격 범위보다 좀 더 가까이(offset) 붙은 상태로 공격 기능 시작하도록
    protected float TrackRange { get => _trackRange; }
    protected float AttackRange { get => _attackRange; }

    protected int _maxHp;
    private int _hp;
    public int Hp { get { return _hp; } private set { _hp = value; UpdateHp(); } }
    protected int Damage { get; private set; }
    private float _moveSpeed;
    private int _skillGauge;
    [SerializeField] private int _terraformingGauge = 0;

    private GameObject _terraformingObjectPrefab;
    private HpBar_Sprite _hpBar;

    protected Rigidbody2D rb;
    // private TMP_Text hpText;

    public Action isDead;
    private AudioClip _damagedAudio;

    protected Coroutine _currentAnimCo;

    protected void BaseInit()
    {
        SettingStat();
        rb = GetComponent<Rigidbody2D>();
        // hpText = GetComponentInChildren<TMP_Text>();
        Player = FindObjectOfType<PlayerController>().gameObject;

        Hp = _maxHp;
        State = Define.EnemyState.Idle;
        _spawnPos = transform.position;

        _localSize = transform.localScale.x;

        _currentAnimState = AnimState.Idle;
        PlayLoopAnim((int)EnemyAnimEnum.Idle);
        _damagedAudio = Resources.Load<AudioClip>("Sounds/Effect/blow1-3");

        _terraformingObjectPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Effect/TerraformingObject");
        _hpBar = GetComponentInChildren<HpBar_Sprite>();
        _hpBar.gameObject.SetActive(false);
    }

    private void SettingStat()
    {
        idx = ((int)Managers.Game.CurrentStage + 1) * 1000
            + (int)_type + 1;
        _maxHp = Managers.Data.EnemyDict[idx].hp;
        Damage = Managers.Data.EnemyDict[idx].damage;
        _touchDamage = Managers.Data.EnemyDict[idx].touchDamage;
        _moveSpeed = Managers.Data.EnemyDict[idx].moveSpeed;
        _skillGauge = Managers.Data.EnemyDict[idx].skillGauge;
        _trackRange = Managers.Data.EnemyDict[idx].trackRange;
        _attackRange = Managers.Data.EnemyDict[idx].attackRange;
    }

    public void ResetState()
    {
        Hp = _maxHp;
        rb.bodyType = RigidbodyType2D.Dynamic;
        State = Define.EnemyState.Idle;
        isAttacking = false;
        _currentAnimState = AnimState.Idle;
        PlayLoopAnim((int)EnemyAnimEnum.Idle);
        ResetStateInChild();
    }

    abstract public void ResetStateInChild();

    private void FixedUpdate()
    {
        switch (State)
        {
            case Define.EnemyState.Idle:
                if (_currentAnimState != AnimState.Idle)
                {
                    _currentAnimState = AnimState.Idle;
                    PlayLoopAnim((int)EnemyAnimEnum.Idle);
                }
                UpdateIdle();
                break;
            case Define.EnemyState.Track:
                if (_currentAnimState != AnimState.Walk)
                {
                    _currentAnimState = AnimState.Walk;
                    PlayLoopAnim((int)EnemyAnimEnum.Walk);
                }
                UpdateTracking();
                break;
            case Define.EnemyState.Return:
                if (_currentAnimState != AnimState.Walk)
                {
                    _currentAnimState = AnimState.Walk;
                    PlayLoopAnim((int)EnemyAnimEnum.Walk);
                }
                UpdateReturning();
                break;
            case Define.EnemyState.Skill:
                break;
            case Define.EnemyState.Death:
                break;
        }
    }

    private void UpdateIdle()
    {
        rb.velocity = Vector2.zero;

        if (Player == null)
            return;

        float dist = (Player.transform.position - transform.position).sqrMagnitude;
        if (dist < TrackRange * TrackRange)
        {
            State = Define.EnemyState.Track;
        }
        if (dist < (AttackRange - _attackRangeOffset) * (AttackRange - _attackRangeOffset))
        {
            rb.velocity = Vector2.zero;
            Attack();
            State = Define.EnemyState.Skill;
        }
    }

    private void UpdateTracking()
    {
        _playerPos = Player.transform.position;
        Vector3 distance = _playerPos - transform.position;
        // 거리가 공격범위 - offset 이내면 멈추고 공격
        if (distance.sqrMagnitude < (AttackRange - _attackRangeOffset) * (AttackRange - _attackRangeOffset)) 
        {
            rb.velocity = Vector2.zero;
            Attack();
            State = Define.EnemyState.Skill;
            return;
        }
        // 추적 범위 밖으로 넘어갔다면 되돌아가기
        if (distance.sqrMagnitude > TrackRange * TrackRange)
        {
            State = Define.EnemyState.Return;
            return;
        }
        rb.velocity = new Vector2(distance.normalized.x * _moveSpeed, 0);
        FlipSprite(Mathf.Sign(distance.x));
    }

    private void UpdateReturning()
    {
        float distance = _spawnPos.x - transform.position.x;
        if ((Player.transform.position - transform.position).sqrMagnitude <= TrackRange * TrackRange)
        {
            State = Define.EnemyState.Track;
            return;
        }
        if (MathF.Abs(distance) <= 0.01f)
        {
            State = Define.EnemyState.Idle;
            return;
        }
        rb.velocity = new Vector2(Mathf.Sign(distance) * _moveSpeed, 0);
        FlipSprite(Mathf.Sign(distance));
    }

    protected void FlipSprite(float dir)
    {
        transform.localScale = new Vector3(-dir * _localSize, _localSize, 1);
        _hpBar.transform.localScale = new Vector3(-dir, 1, 1);
    }

    abstract protected void Attack();

    // 몬스터 개체 피격
    public void Damaged(int minus, GameObject attacker = null)
    {
        _hpBar.gameObject.SetActive(true);
        Managers.Sound.Play(_damagedAudio, Define.Sound.Effect);
        if (State == Define.EnemyState.Death)
            return;
        Hp = Mathf.Clamp(_hp - minus, 0, _maxHp);
        _hpBar.updateHpBar(Hp, _maxHp);
        if (Hp <= 0)
        {
            DamagedAnim();
            StartCoroutine(coDeath());
            return;
        }
        if (attacker != null)
            KnockBack(attacker.transform.position); // 공격자 좌표 전달
        DamagedAnim();
        UpdateHp();
    }

    public void KnockBack(Vector3 attackPos, float force = 3.0f)
    {
        rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - attackPos.x) * force, 0), ForceMode2D.Impulse);
    }

    // 몬스터 개체 사망
    public IEnumerator coDeath()
    {
        State = Define.EnemyState.Death;
        if (isDead != null)
            isDead.Invoke();
        // rb.bodyType = RigidbodyType2D.Static;
        if(_currentAnimCo != null) StopCoroutine(_currentAnimCo);
        _currentAnimState = AnimState.Die;
        PlayOneShotAnim((int)EnemyAnimEnum.Die, false);
        yield return new WaitForSeconds(1.8f);
        gameObject.SetActive(false);
        _hpBar.updateHpBar(_maxHp, _maxHp);
        _hpBar.gameObject.SetActive(true);

        for (int i = 0; i < _terraformingGauge; i++)
        {
            DropObject(_terraformingObjectPrefab, transform.position + new Vector3(i * 0.2f, 0.0f, 0.0f));
        }
    }

    private void DropObject(GameObject prefab, Vector3 position)
    {
        GameObject go = Instantiate(prefab);
        go.transform.position = position;
    }

    // 삭제 예정
    private void UpdateHp()
    {
        // hpText.text = Hp.ToString() + "/" + _maxHp.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_touchDamage == 0)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Damaged(_touchDamage);
        }
    }
}
