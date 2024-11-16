using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForestEnemy : Enemy
{
    private GameObject _mucusPrefab;
    private GameObject _bulletPrefab;
    private Vector2[] _bulletDir = new Vector2[] {
        // new Vector2(-1, 5),
        // new Vector2(-2, 6),
        // new Vector2(-3, 7),
        // new Vector2(1, 5),
        // new Vector2(2, 6),
        // new Vector2(3, 7),
        new Vector2(-3, 4),
        new Vector2(-1, 5),
        new Vector2(1, 5),
    };
    private Vector2[] _bulletDirFlip = new Vector2[] {
        new Vector2(3, 4),
        new Vector2(1, 5),
        new Vector2(-1, 5),
    };

    private float _time;
    private float _explodeTime = 2.0f;
    private WaitForSecondsRealtime waitExplodeTick;
    private float _explodeTick = 0.1f;

    private float _knockBackPower = 6.0f;

    public void Init()
    {
        BaseInit();
        SkillSetting();
        ResetStateInChild();
    }

    override public void ResetStateInChild()
    {
        _time = 0.0f;
    }

    protected void SkillSetting()
    {
        switch (_type)
        {
            case Define.ForestEnemyType.Forest01:
                _mucusPrefab = Managers.Resource.Load<GameObject>("Prefabs/Sprites/Mucus");
                // StartCoroutine(coDropMucus());
                break;
            case Define.ForestEnemyType.Forest02:
                _bulletPrefab = Managers.Resource.Load<GameObject>("Prefabs/Sprites/EnemyBomb");
                StartCoroutine(coTrackPlayer());
                break; 
            case Define.ForestEnemyType.Forest03:
                break;
            case Define.ForestEnemyType.Forest04:
                waitExplodeTick = new WaitForSecondsRealtime(_explodeTick);
                _time = 0.0f;
                break;
        }
    }

    override protected void Attack()
    {
        if (isAttacking)
            return;
        isAttacking = true;
        switch (_type)
        {
            case Define.ForestEnemyType.Forest01:
                _currentAnimCo = StartCoroutine(nameof(coCommonAttack));
                break;
            case Define.ForestEnemyType.Forest02:
                _currentAnimCo = StartCoroutine(nameof(coShootAttack));
                break;
            case Define.ForestEnemyType.Forest03:
                // if (Util.checkProbability(50))
                _currentAnimCo = StartCoroutine(nameof(coJumpAttack));
                // else
                //StartCoroutine(nameof(coNormalAttack));
                break;
            case Define.ForestEnemyType.Forest04:
                _currentAnimCo = StartCoroutine(nameof(CheckExplode));
                break;
        }
    }

    private bool CheckStandGround()
    {
        RaycastHit2D hits = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, 0.1f, LayerMask.GetMask("Floor") | LayerMask.GetMask("Platform"));
        if (hits.transform != null)
            return true;
        return false;
    }

    private IEnumerator coCommonAttack()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        if (State == Define.EnemyState.Death)
            yield break;
        _currentAnimState = AnimState.Hit;
        PlayOneShotAnim((int)EnemyAnimEnum.Hit);

        Collider2D hit = Physics2D.OverlapBox(
            transform.position + Vector3.forward * transform.localScale.x,
            Vector2.one * 1.5f, 0, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            hit.GetComponent<PlayerController>().Damaged(Damage, gameObject);
            Player.GetComponent<Rigidbody2D>().AddForce(
                new Vector2(Mathf.Sign(Player.transform.position.x - transform.position.x) * _knockBackPower, 0), ForceMode2D.Impulse);
            if (Util.checkProbability(50))
                Player.GetComponent<PlayerController>().Slow(3.0f, 0.7f);
        }
        yield return new WaitForSecondsRealtime(1.2f);

        if (State != Define.EnemyState.Death)
            State = Define.EnemyState.Idle;
        isAttacking = false;
    }

    // position 움직임 없이 방향만 바라보도록
    private IEnumerator coTrackPlayer()
    {
        while (true)
        {
            FlipSprite(Mathf.Sign(Player.transform.position.x - transform.position.x));
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator coShootAttack()
    {
        _currentAnimState = AnimState.Hit;
        PlayOneShotAnim((int)EnemyAnimEnum.Hit);
        yield return new WaitForSeconds(0.55f);
        ShootAttack();
        yield return new WaitForSeconds(5.0f);
        if (State != Define.EnemyState.Death)
            State = Define.EnemyState.Idle;
        isAttacking = false;
    }

    private void ShootAttack()
    {
        // 반전되어 있지 않은 상태면 그대로 발사
        if (transform.localScale.x > 0)
        {
            for (int i = 0; i < _bulletDir.Length; i++)
            {
                GameObject go = Instantiate(_bulletPrefab, transform.position + Vector3.up, Quaternion.identity);
                go.GetComponent<DroppableEnemyObject>().ShootObject(_bulletDir[i]);
            }
        }
        // 반전되어 있는 경우 반전된 좌표로 발사
        else
        {
            for (int i = 0; i < _bulletDir.Length; i++)
            {
                GameObject go = Instantiate(_bulletPrefab, transform.position + Vector3.up, Quaternion.identity);
                go.GetComponent<DroppableEnemyObject>().ShootObject(_bulletDirFlip[i]);
            }
        }
    }

    // Forest03
    // Extra1: Jump Start
    // Extra2: Jump End
    private IEnumerator coJumpAttack()
    {
        ChangeSkin(0);
        FlipSprite(Mathf.Sign(Player.transform.position.x - transform.position.x));
        yield return new WaitForSecondsRealtime(0.5f);
        float distX = Player.transform.position.x - transform.position.x;
        FlipSprite(Mathf.Sign(distX));
        PlayOneShotAnim((int)EnemyAnimEnum.Extra1);
        yield return new WaitForSecondsRealtime(0.2f);
        rb.AddForce(new Vector2(distX * 0.5f, 10.0f), ForceMode2D.Impulse);
        yield return new WaitWhile(() => CheckStandGround());
        yield return new WaitUntil(() => CheckStandGround());

        Collider2D hits = Physics2D.OverlapBox(
            transform.position, new Vector2(4.0f, 3.0f), 0, LayerMask.GetMask("Player"));
        if (hits != null)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            hits.GetComponent<PlayerController>().Damaged(Damage, gameObject);
            Player.GetComponent<Rigidbody2D>().AddForce(
                new Vector2(Mathf.Sign(Player.transform.position.x - transform.position.x) * _knockBackPower, 0), ForceMode2D.Impulse);
        }
        rb.velocity = Vector2.zero;
        PlayOneShotAnim((int)EnemyAnimEnum.Extra2);
        yield return new WaitForSeconds(2.5f);
        State = Define.EnemyState.Idle;
        PlayLoopAnim((int)EnemyAnimEnum.Idle);
        isAttacking = false;
    }

    // 개구리 혓바닥 공격
    private IEnumerator coNormalAttack()
    {
        ChangeSkin(1);
        FlipSprite(Mathf.Sign(Player.transform.position.x - transform.position.x));
        yield return new WaitForSecondsRealtime(0.2f);
        if (State == Define.EnemyState.Death)
            yield break;
        _currentAnimState = AnimState.Hit;
        PlayOneShotAnim((int)EnemyAnimEnum.Hit);

        Collider2D hit = Physics2D.OverlapBox(
            transform.position + Vector3.forward * transform.localScale.x,
            Vector2.one * 2.0f, 0, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            hit.GetComponent<PlayerController>().Damaged(Damage, gameObject);
            Player.GetComponent<Rigidbody2D>().AddForce(
                new Vector2(Mathf.Sign(Player.transform.position.x - transform.position.x) * _knockBackPower, 0), ForceMode2D.Impulse);
        }

        yield return new WaitForSecondsRealtime(1.2f);

        if (State != Define.EnemyState.Death)
            State = Define.EnemyState.Idle;
        isAttacking = false;
        ChangeSkin(0);
    }

    private IEnumerator CheckExplode()
    {
        while (State != Define.EnemyState.Death)
        {
            if (_time >= _explodeTime)
            {
                StartCoroutine(nameof(coExplodeAttack));
                State = Define.EnemyState.Death;
                yield break;
            }

            float dist = (Player.transform.position - transform.position).sqrMagnitude;
            if (dist > AttackRange * AttackRange)
            {
                _time = 0.0f;
                State = Define.EnemyState.Idle;
                isAttacking = false;
                yield break;
            }

            yield return waitExplodeTick;
            _time += _explodeTick;
        }
    }

    // 1번 몬스터 점액 분비(삭제)
    // private IEnumerator coDropMucus()
    // {
    //     while (true)
    //     {
    //         if (State == Define.EnemyState.Return || State == Define.EnemyState.Track)
    //         {
    //             Instantiate(_mucusPrefab, gameObject.transform.position - new Vector3 (0, 0.45f, 0), Quaternion.identity);
    //         }
    //         yield return new WaitForSeconds(0.4f);
    //     }
    // }

    private IEnumerator coExplodeAttack()
    {
        PlayOneShotAnim((int)EnemyAnimEnum.Hit, false, true);
        isAttacking = false;
        yield return new WaitForSeconds(1.0f); // 애니메이션 실행 후 자폭 + 데미지 입히기까지 걸리는 시간
        Exploding();
        //Damaged(_maxHp);
        StartCoroutine(coDeath());
        _time = 0.0f;
    }

    public void Exploding()
    {
        Collider2D hits = Physics2D.OverlapCircle(transform.position, 3.0f, LayerMask.GetMask("Player"));
        if (hits == null)
            return;
        hits.GetComponent<PlayerController>().Damaged(Damage, gameObject);
    }
}
