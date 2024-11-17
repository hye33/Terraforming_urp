using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Spine;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    GameObject[] _infoPrefabs;
    private enum infoEnum{ interactKey, mouseKey, moveKey, saveKey }
    Vector3 _infoPos = new Vector3(0, 3.0f, 0);

    // size
    private float defaultScale;

    private bool _stopMove;

    private enum PlayerState { None, Run, Idle, Die, Shoot, Temp, Jump, Sword, Damaged }
    [SerializeField] private PlayerState _currentState;

    private Animator animator;

    // Spine Anim Enum
    public enum PlayerAnimEnum { Idle, Walk, Jump, Land, Hit, Die, Attacked }
    public enum PlayerSkinEnum { Sword, Gun }

    public PlayerAnimEnum _pastAnim;

    public Action<float> updateHp;
    public Action decreaseLife;
    public Action gameOver;
    public Action<Define.PlayerWeapon> weaponChange;

    // Invoke when player change weapon
    public Action<Define.PlayerWeapon, int> changeWeapon;

    Define.EnemyState State;

    private MapScene _forestScene;

    private AudioClip _walkAudio;
    private AudioClip _landingAudio;
    private AudioClip _deathAudio;

    public int _jumpCount;
    private bool _settingSavePoint;
    private bool _canHit;
    private bool _isTakeDowning;
    private bool _isExploding;
    private bool _canDash;
    private int _maxBullet = 2;
    private int _shootCount;
    private float _coolTime = 5.0f;
    private int _bulletChargeCount;

    public bool _canMakeSavepoint;

    private bool _standOnPlatform;

    private bool _getKeyS;

    [SerializeField]
    private bool _unlockDoubleJump = false;
    [SerializeField]
    private bool _unlockTakeDown = false;
    [SerializeField]
    private bool _unlockSelfExplode = false;

    private int _life = 4;
    public int Life { get => _life; }
    private int _hp;
    public int Hp { get => _hp; private set { _hp = value; UpdateHp(); } }
    private int _maxHp;

    private float _moveSpeed = Stat.PLAYER_MOVE_SPEED;
    private float _jumpPower = Stat.PLAYER_JUMP_POWER;
    private float _takeDownPower = Stat.PLAYER_TAKEDOWN_POWER;

    private Rigidbody2D rb;
    //private CapsuleCollider2D capsuleCollider;
    private CapsuleCollider2D playerCollider;

    private Define.PlayerWeapon _weaponType;
    private PlayerSword _sword;
    private PlayerGun _gun;
    private GameObject explodePrefab;
    private GameObject _explode;
    [SerializeField] private GameObject _dashSmokePrefab;
    [SerializeField] private GameObject _shadow;

    //position offset
    private Vector3 _feetOffset = new Vector3(0, -1.2f, 0); // 발 위치

    private Puzzle _puzzle; // 퍼즐 UI 생성되었는지 확인 용 
    private UI_Record _record;

    private Vector3[] _autoSavePos;
    private void Init()
    {
        _forestScene = GetComponentInParent<MapScene>();

        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        //capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        _sword = GetComponentInChildren<PlayerSword>();
        _gun = GetComponentInChildren<PlayerGun>();
        explodePrefab = Resources.Load<GameObject>("Prefabs/Sprites/Explode");
        _dashSmokePrefab = Resources.Load<GameObject>("Prefabs/Sprites/Player/PlayerSmoke");

        _walkAudio = Resources.Load<AudioClip>("Sounds/Effect/PlayerLanding");
        _landingAudio = Resources.Load<AudioClip>("Sounds/Effect/PlayerLanding");
        _deathAudio = Resources.Load<AudioClip>("Sounds/Effect/PlayerDeath");

        _infoPrefabs = Resources.LoadAll<GameObject>("Prefabs/Sprites/InfoUI");

        _autoSavePos = Util.GetAllChildPositions(FindObjectOfType<AutoSavePoint>().gameObject);

        defaultScale = transform.localScale.y;

        Managers.Input.KeyAction = null;
        Managers.Input.KeyAction += InputKey;
        Managers.Input.MouseAction = null;
        Managers.Input.MouseAction += InputMouse;
    }

    private void ResetState()
    {
        _maxHp = (int)Managers.Data.Player.hp;
        Hp = _maxHp;
        State = Define.EnemyState.Idle;
        _weaponType = Define.PlayerWeapon.Sword;
        _shootCount = _maxBullet;

        _jumpCount = 0;
        _settingSavePoint = false;
        _canHit = true;
        _isTakeDowning = false;
        _isExploding = false;
        _canDash = true;
        _getKeyS = false;
        _standOnPlatform = false;
        _stopMove = false; // Set True when Shoot or Death state

        _canMakeSavepoint = true;

        _currentState = PlayerState.Idle;

        _gun._aim.gameObject.SetActive(_weaponType == Define.PlayerWeapon.Gun);

        _gun.Reset();
        _bulletChargeCount = 0;
        _sword.isSwing = false;

        // // spine reset
        // ChangeSkin((int)PlayerSkinEnum.Sword);
        // PlayLoopAnim((int)PlayerAnimEnum.Idle * 2 + (int)_weaponType, 0);

        ChangeAnimationState(PlayerSkinEnum.Sword);

        if (changeWeapon != null)
            changeWeapon.Invoke(_weaponType, _shootCount);
    }

    private void SaveGame()
    {
        Managers.Game.SaveData.playerPosition = transform.position;
        Managers.Game.SaveData.playerLife = _life;
        Managers.Game.SaveData.playerHp = _hp;
        Managers.Game.SaveGameData(Managers.Game.SaveData.slotNum);
    }

    public void AutoSaveGame(int idx)
    {
        Managers.Game.SaveData.playerPosition = _autoSavePos[idx];
        Managers.Game.AutoSaveNum = idx;
    }

    private void Start()
    {
        Init();
        ResetState();
        // ShowInfoIcon(infoEnum.moveKey);

        Managers.Game.SaveData.playerPosition = _autoSavePos[0];
        Managers.Game.AutoSaveNum = 0;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position + _feetOffset, (Mathf.Sign(transform.localScale.x) < 0 ? Vector3.left : Vector3.right) * 6.0f, Color.red);
        if (_currentState == PlayerState.Die)
            return;
        // velocity check
        if (Mathf.Abs(rb.velocity.x) < 0.2f)
        {
            if (_currentState < PlayerState.Idle && !_stopMove)
            {
                Managers.Sound.Stop(Define.Sound.LoopEffect);
                _currentState = PlayerState.Idle;
                PlayAnimation(PlayerAnimEnum.Idle);
                // PlayLoopAnim((int)PlayerAnimEnum.Idle * 2 + (int)_weaponType, 0);
            }
        }
    }

    // private void ShowInfoIcon(infoEnum info)
    // {
    //     GameObject go = Instantiate(_infoPrefabs[(int)info], transform);
    //     go.transform.position += _infoPos;
    // }

    #region Animation
    public void PlayAnimation(PlayerAnimEnum anim)
    {
        switch (anim)
        {
            case PlayerAnimEnum.Idle:
                animator.SetTrigger("ReturnIdle");
                break;
            case PlayerAnimEnum.Walk:
                animator.SetTrigger("Walk");
                break;
            case PlayerAnimEnum.Attacked:
                animator.SetTrigger("Attacked");
                break;
            case PlayerAnimEnum.Die:
                animator.SetTrigger("Die");
                break;
            case PlayerAnimEnum.Hit:
                // 총 칼 분기 분리하기
                animator.SetTrigger("Hit");
                break;
            case PlayerAnimEnum.Jump:
                animator.SetTrigger("Jump");
                //animator.SetBool("IsJumping", true);
                break;
            case PlayerAnimEnum.Land:
                animator.SetTrigger("Land");
                break;
        }
        _pastAnim = anim;
    }

    private void ChangeAnimationState(PlayerSkinEnum state)
    {
        animator.SetInteger("Weapon", (int)state);
    }

    // 나머지 애들도 함수 사용하도록 수정해야 함
    private void SetStateIdle()
    {
        _currentState = PlayerState.Idle;
        PlayAnimation(PlayerAnimEnum.Idle);
        _stopMove = false;
    }
    #endregion

    #region Input Key
    private void InputKey(Define.KeyEvent key)
    {
        if (_stopMove || _currentState == PlayerState.Die)
            return;

        switch (key)
        {
            case Define.KeyEvent.A:
                Move(-1);
                break;
            case Define.KeyEvent.D:
                Move(1);
                break;
            case Define.KeyEvent.W:
                InteractW();
                break;
            case Define.KeyEvent.Space:
                Jump();
                break;
            case Define.KeyEvent.R:
                SettingSavePoint();
                break;
            case Define.KeyEvent.S:
                _getKeyS = true;
                break;
            case Define.KeyEvent.SUp:
                _getKeyS = false;
                break;
            case Define.KeyEvent.LeftShift:
                Dash();
                break;
        }
    }

    public void InputMouse(Define.MouseEvent type)
    {
        if (_stopMove || _currentState == PlayerState.Die)
            return;

        if (type == Define.MouseEvent.LeftClick)
        {
            if (_getKeyS)
            {
                if (_isExploding || _unlockSelfExplode == false)
                    return;
                _isExploding = true;
                StartCoroutine(coSelfExplode());
            }
            else
                Attack();
        }
        if (type == Define.MouseEvent.RightClick && _settingSavePoint == false)
        {
            WeaponChange();
        }
    }
    #endregion

    #region Hp
    public void Healed(int heal)
    {
        Hp = Mathf.Clamp(_hp + heal, 0, _maxHp);
    }

    public void Slow(float duration, float percentage)
    {
        StartCoroutine(coSlow(duration, percentage, _moveSpeed));
    }

    private IEnumerator coSlow(float duration, float percentage, float defaultMoveSpeed)
    {
        _moveSpeed = defaultMoveSpeed * percentage;
        yield return new WaitForSeconds(duration);
        _moveSpeed = defaultMoveSpeed;
    }

    // 피격시 넉백 효과
    public void KnockBack(Vector3 attackPos, float force = 1.5f)
    {
        rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - attackPos.x) * force, 0), ForceMode2D.Impulse);
    }

    // TODO: 피격 후 무적 시간
    public void Damaged(int damage, GameObject attacker = null)
    {
        if (damage == 0)
            return;
        if (_canHit == false)
            return;
        _canHit = false;
        Hp = Mathf.Clamp(_hp - damage, 0, _maxHp);
        if (Hp <= 0)
        {
            StartCoroutine(coDeath());
            return;
        }
        if (attacker != null)
            KnockBack(attacker.transform.position);
        StartCoroutine(coAfterDamaged());
    }

    private IEnumerator coAfterDamaged()
    {
        _currentState = PlayerState.Damaged;
        //PlayOneShotAnim((int)PlayerAnimEnum.Attacked * 2 + (int)_weaponType, true, true);
        //DamagedAnim();
        PlayAnimation(PlayerAnimEnum.Attacked);
        yield return new WaitForSeconds(0.35f);
        _currentState = PlayerState.None;
        _canHit = true;
    }

    private void UpdateHp()
    {
        if (updateHp != null)
            updateHp.Invoke(_hp);
    }

    // 지속데미지
    public IEnumerator coContinousDamage(int damage, float term, int count)
    {
        WaitForSecondsRealtime delta = new WaitForSecondsRealtime(term); // delta 텀으로 damage가 들어옴
        for (int i = 0; i < count; i++)
        {
            Damaged(damage);
            yield return delta;
        }
    }

    private IEnumerator coDeath()
    {
        _stopMove = true;
        _currentState = PlayerState.Die;
        rb.velocity = Vector2.zero;
        //PlayOneShotAnim((int)PlayerAnimEnum.Die * 2 + (int)_weaponType, false, true, true, 0);
        PlayAnimation(PlayerAnimEnum.Die);
        _canHit = false;
        yield return new WaitForSecondsRealtime(0.8f);
        _life--;
        if (_life <= 0)
        {
            Invoke("GameOver", 1.0f);
            yield break;
        }
        Invoke("Resurrent", 1.0f);
    }

    private void Resurrent()
    {
        PlayAnimation(PlayerAnimEnum.Idle);
        if (decreaseLife != null)
            decreaseLife.Invoke();
        ResetState();
        gameObject.SetActive(true);
        transform.position = Managers.Game.SaveData.playerPosition;
    }

    private void GameOver()
    {
        PlayAnimation(PlayerAnimEnum.Idle);
        _life = 4;
        if (decreaseLife != null)
            decreaseLife.Invoke();
        ResetState();
        gameObject.SetActive(true);
        Managers.Game.SetGameDataToSaved();
        transform.position = Managers.Game.SaveData.playerPosition;
    }
    #endregion

    #region Move and Jump

    private void Move(int dir)
    {
        if (_currentState != PlayerState.Run && _currentState < PlayerState.Jump)
        {
            PlayAnimation(PlayerAnimEnum.Walk);
            Managers.Sound.Play(_walkAudio, Define.Sound.LoopEffect);
            _currentState = PlayerState.Run;
            //PlayLoopAnim((int)PlayerAnimEnum.Walk * 2 + (int)_weaponType, 0);
        }
        rb.velocity = new Vector2(dir * _moveSpeed, rb.velocity.y);
        FlipSprite(rb.velocity.x);
    }

    private void FlipSprite(float dir)
    {
        transform.localScale = new Vector2(Mathf.Sign(dir) * defaultScale, defaultScale);
    }

    private void StopPlayer()
    {
        _stopMove = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void Jump()
    {
        Managers.Sound.Stop(Define.Sound.LoopEffect);
        if (_getKeyS)
        {
            if (_standOnPlatform)
                StartCoroutine(nameof(coDownJump));
            return;
        }
        if (_jumpCount >= 2)
            return;
        if (_jumpCount == 1 && _unlockDoubleJump == false)
            return;
        _canDash = false;
        _shadow.SetActive(false);
        PlayAnimation(PlayerAnimEnum.Jump);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        _jumpCount++;

        _gun.ChangeState(canShoot: false);

        _currentState = PlayerState.Jump;
        //PlayOneShotAnim((int)PlayerAnimEnum.Jump * 2 + (int)_weaponType, false);
    }

    private IEnumerator coDownJump()
    {
        float y = transform.position.y;
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        playerCollider.isTrigger = true;
        while (transform.position.y > y - 0.5f && transform.position.y <= y)
        {
            yield return wait;
        }
        playerCollider.isTrigger = false;
    }

    private void Dash()
    {
        if (_canDash == false)
            return;
        _canDash = false;

        Vector3 posOffset = new Vector3(0.5f, 0.5f, 0);
        GameObject go = Instantiate(_dashSmokePrefab);
        go.transform.position = transform.position + posOffset;
        go.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x) > 0 ? go.transform.localScale.x : -go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
        StartCoroutine(coSetDestoryTimer(go, 1.0f));

        // 얘 나중에 위쪽으로 빼두기
        float distance = 6.0f;
        bool isFlip = Mathf.Sign(transform.localScale.x) < 0;
        rb.AddForce((Mathf.Sign(transform.localScale.x) > 0 ? Vector3.right : Vector3.left) * 21.0f, ForceMode2D.Impulse);

        
        // float distanceOffset = 0.5f;
        // bool isFlip = Mathf.Sign(transform.localScale.x) < 0;
        // RaycastHit2D hit = Physics2D.Raycast(
        //     transform.position + _feetOffset,
        //     isFlip ? Vector3.left : Vector3.right,
        //     distance + distanceOffset,
        //     LayerMask.GetMask("Floor") | LayerMask.GetMask("Platform"));
        // Vector3 targetPosition = transform.position + (isFlip ? Vector3.left : Vector3.right) * distance;
        // if (hit.collider == null)
        // {
        //     Debug.Log("Dash: not hit");
        //     // 충돌이 없을 때만 이동
        //     transform.position = targetPosition;
        // }
        // else
        // {
        //     Debug.Log("Dash: hit");
        //     // 충돌 지점까지만 이동
        //     Vector3 offset = new Vector3(isFlip ? distanceOffset : -distanceOffset, 0, 0);
        //     transform.position = (Vector3)hit.point - _feetOffset + offset;
        // }
    }

    private IEnumerator coSetDestoryTimer(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(go);
        _canDash = true;
    }

    private IEnumerator coSetActiveTimer(GameObject go, bool active, float t)
    {
        yield return new WaitForSeconds(t);
        go.SetActive(active);
    }

    #endregion

    #region UI Event
    private void InteractW()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(
            transform.position, 1.0f, LayerMask.GetMask("ItemObject") | LayerMask.GetMask("Puzzle"));
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("ItemObject"))
                collider.GetComponentInParent<ItemObject>().ShowSign(collider);
            else
            {
                if (_puzzle == null && !Managers.Game.SaveData.puzzleSolved)
                {
                    _puzzle = Managers.UI.ShowPopupUI<Puzzle>("Puzzle");
                    Managers.UI.SetCanvas(_puzzle.gameObject);
                }
            }
        }
    }
    #endregion

    #region Attack and Skills
    private void Attack()
    {
        if (_weaponType == Define.PlayerWeapon.Sword)
        {
            if (_sword.isSwing)
                return;
            if (_jumpCount > 0 && _unlockTakeDown)
            {
                rb.AddForce(Vector2.down * _takeDownPower, ForceMode2D.Impulse);
                _isTakeDowning = true;
                return;
            }
            //PlayOneShotAnim((int)PlayerAnimEnum.Hit * 2 + (int)_weaponType, true, true);
            StopPlayer();
            _currentState = PlayerState.Sword;
            PlayAnimation(PlayerAnimEnum.Hit);
            _sword.Swing();
            Invoke("SetStateIdle", 0.8f);
        }
        else if (_weaponType == Define.PlayerWeapon.Gun)
        {
            if (_jumpCount > 0)
                return;
            if (_shootCount == 0)
                return;

            PlayAnimation(PlayerAnimEnum.Hit);
            StartCoroutine(coShooting());
        }
    }

    public void AddChargeBulletCount()
    {
        if (_shootCount > 1) // 총알 2개인 상태면 카운트 X
            return;
        _bulletChargeCount++;
        if (_bulletChargeCount == 10) // 카운트 10개 = 총알 1개로 장전
        {
            _bulletChargeCount -= 10;
            _shootCount++;
            // 장전 코드 써야함
        }
    }

    private IEnumerator coShooting()
    {
        _gun.Shoot(transform.position);
        _currentState = PlayerState.Shoot;
        FlipSprite(_gun.GetComponent<PlayerGun>()._aim.position.x - transform.position.x);
        //PlayOneShotAnim((int)PlayerAnimEnum.Hit * 2 + (int)_weaponType, true, true);
        _stopMove = true;
        rb.velocity = new Vector2(0, 0);
        _shootCount--;
        if (changeWeapon != null)
            changeWeapon.Invoke(_weaponType, _shootCount); // 캐릭터 UI의 총알 개수 변경
        if (_shootCount == 0)
        {
            _gun.ChangeState(canShoot: false);
            // StartCoroutine(coCharging());
        }

        yield return new WaitForSeconds(0.6f);
        _currentState = PlayerState.None;
        _stopMove = false;
    }

    private void WeaponChange()
    {
        if (weaponChange != null)
            weaponChange.Invoke(_weaponType);

        if (_weaponType == Define.PlayerWeapon.Sword)
        {
            //ChangeSkin((int)PlayerSkinEnum.Gun);
            _weaponType = Define.PlayerWeapon.Gun;
        }
        else if (_weaponType == Define.PlayerWeapon.Gun)
        {
            //ChangeSkin((int)PlayerSkinEnum.Sword);
            _weaponType = Define.PlayerWeapon.Sword;
        }
        _gun._aim.gameObject.SetActive(_weaponType == Define.PlayerWeapon.Gun);
        animator.SetInteger("Weapon", (int)_weaponType);

        if (_currentState == PlayerState.Idle)
        {
            animator.SetTrigger("ReturnIdle");
        }

        if (changeWeapon != null)
            changeWeapon.Invoke(_weaponType, _shootCount);
    }

    // 바꾼 로직 적용 잘 되는 거 확인하고 지울 것
    private IEnumerator coCharging()
    {
        changeWeapon?.Invoke(_weaponType, _shootCount);
        yield return new WaitForSecondsRealtime(_coolTime);
        _shootCount = _maxBullet;
        _gun.ChangeState(canShoot: true);
        changeWeapon?.Invoke(_weaponType, _shootCount);
    }

    private IEnumerator coSelfExplode()
    {
        if (_explode == null)
        {
            _explode = Instantiate(explodePrefab, transform);
            _explode.GetComponent<Explode>().Init(
                isPlayer: true,
                damage: Managers.Data.Player.explodeDamage,
                range: Stat.PLAYER_EXPLODE_RANGE);
        }
        Hp = Mathf.RoundToInt(Hp * 0.7f);
        _explode.transform.position = transform.position;
        _explode.GetComponent<Explode>().Exploding();
        yield return new WaitForSecondsRealtime(0.5f);
        _isExploding = false;
    }

    private void SettingSavePoint()
    {
        if (!_canMakeSavepoint)
            return;

        int slotNum = Managers.Game.SaveData.slotNum;
        Managers.Game.SaveData.playTime = _forestScene.PlayTime; // 플레이 타임 반영
        //Managers.Game.SaveGameData(slotNum);
        Collider2D savePoint = Physics2D.OverlapCircle(
            transform.position, 1.0f, LayerMask.GetMask("SavePoint"));

        if (savePoint != null)
        {
            // 널일 경우 아무 작동 X
            // if (savePoint.GetComponent<SavePoint>().index == 1)
            // {
            //     transform.position = _forestScene.SavePointPos[2];
            // }
            // else
            // {
            //     transform.position = _forestScene.SavePointPos[1];
            // }
        }
        else
        {
            _forestScene.MakeSavePoint(transform.position);
            SaveGame();
        }
    }

    public void GetTerraformingGauge(int gauge)
    {
        _forestScene.UpdateTerraformingGauge(gauge, "Enemy"); // 수치 증가
    }
    #endregion

    #region Unity Event
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
            _standOnPlatform = false;
        else if (other.gameObject.CompareTag("Platform"))
            _standOnPlatform = true;
        else
            return;

        if (_isTakeDowning)
        {
            _sword.Swing();

        }
        _isTakeDowning = false;

        if (_jumpCount > 0 && _shootCount != 0)
        {
            _gun.ChangeState(canShoot: true);
        }
        // 착지된 상태
        _jumpCount = 0;

        // 점프 후 착지된 상태
        if (_currentState == PlayerState.Jump)
        {
            _shadow.SetActive(true);
            _canDash = true;
            Managers.Sound.Play(_landingAudio, Define.Sound.Effect);
            _currentState = PlayerState.None;
            animator.SetBool("IsJumping", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("player trigger: " + other.gameObject.name);
        if (other.gameObject.CompareTag("DeathTrigger"))
        {
            Damaged(_maxHp);
        }
    }
    #endregion

}
