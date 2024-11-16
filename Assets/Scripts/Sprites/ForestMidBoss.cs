using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestMidBoss : Boss
{
    // SerializeField
    [SerializeField] MapScene _scene;
    [SerializeField] PuzzleLock _puzzleLock;
    [SerializeField] GameObject _gas;

    //Enum
    protected enum MidBossAnimEnum { Idle, Pause, Walk, Dash, Jump, Breath }
    private enum MidBossState { Idle, Rush, Drop, Breath, Jump, Death }

    // State
    private MidBossState _state;
    private GameObject _player;
    private CameraController _camera;
    private bool isTrack = false;

    // measurement
    [SerializeField] private GameObject _dropObjectGameObj;
    Vector3[] _dropObjectPositions;

    // Prefab
    private GameObject _damagePrefab;
    private GameObject _dropObjectPrefab;
    private GameObject _gasPrefab;
    private GameObject _explodePrefab;
    private GameObject effect;

    float _dir;

    protected override void Awake()
    {
        base.Awake();
        base.BaseInit();
        _scene = FindObjectOfType<MapScene>();
        _player = FindObjectOfType<PlayerController>().gameObject;
        _camera = Camera.main.GetComponent<CameraController>();
        if (_camera == null)
            Debug.Log(Camera.main.name);
        _rb = GetComponent<Rigidbody2D>();
        _maxHp = Managers.Data.EnemyDict[1010].hp;
        _hp = _maxHp;

        _damagePrefab = Resources.Load<GameObject>("Prefabs/Sprites/DamageRange");
        _dropObjectPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Effect/Stalactite");
        _gasPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Effect/ForestMidBossGas");
        _explodePrefab = Resources.Load<GameObject>("Prefabs/Sprites/Player/PlayerGunEffect");
    }

    private void SetPositions()
    {
        _dropObjectPositions = new Vector3[_dropObjectGameObj.transform.childCount];
        _dropObjectPositions = Util.GetAllChildPositions(_dropObjectGameObj);
    }

    private IEnumerator coDestory(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(go);
    }

    private IEnumerator coSetActive(GameObject go, bool isActive, float t)
    {
        yield return new WaitForSeconds(t);
        go.SetActive(isActive);
    }

    void Start()
    {
        SetPositions();
        _state = MidBossState.Idle;
        attackedAction += StartMidBossStage;
        canAttacked = true;

        _gas.SetActive(false);
        _dropObjectGameObj.SetActive(false);

        _dir = -1;

        PlayLoopAnim((int)MidBossAnimEnum.Idle);
    }
    
    private void StartMidBossStage()
    {
        coChangeTrackState(true);
        attackedAction -= StartMidBossStage;
        StartCoroutine(coPattern());
    }

    private void Update()
    {
        if (isTrack == false)
            return;
        float d = _player.transform.position.x - transform.position.x;
        if (Mathf.Abs(d) < 0.1f) return;

        _dir = Mathf.Sign(d);
        if (_dir != 0) FlipSprite(_dir);
        _rb.velocity = new Vector2(_dir * 5.0f, 0);
    }

    private IEnumerator coChangeTrackState(bool state, float t = 0)
    {
        if (t != 0)
            yield return new WaitForSeconds(t);
        if(state)
        {
            PlayLoopAnim((int)MidBossAnimEnum.Walk);
        }
        else
        {
            PlayLoopAnim((int)MidBossAnimEnum.Idle);
        }
        isTrack = state;
    }

    private IEnumerator coPattern()
    {
        yield return new WaitForSeconds(0.5f);
        // float translteTime = 1.0f;
        while (true)
        {
            DropObject();
            yield return new WaitForSeconds(7.0f);
            if (_state == MidBossState.Death)
                break;

            JumpAttack();
            yield return new WaitForSeconds(7.0f);
            if (_state == MidBossState.Death)
                break;

            EmitGas();
            yield return new WaitForSeconds(7.0f);
            if (_state == MidBossState.Death)
                break;
            // RushToPlayer();
            // yield return new WaitForSeconds(7.0f);
        }
    }

    public override void Death()
    {
        _scene.UpdateTerraformingGauge(10, "Cave"); //중간보스 처치 완료시 테라포밍 게이지 10 증가
        _puzzleLock.OpenDoor("MiniBossLock"); // 중간보스 처치시 문 열림 
        StopAllCoroutines();
        // effect = Instantiate(_explodePrefab);
        // effect.transform.position = transform.position;
        Destroy(gameObject);
        // Invoke("DestroyMidBossEffect", 0.8f);
        _camera.ChangeCameraState((int)CameraController.ForestMapCameraState.FollowPlayer); // 카메라 state 변경
    }

    private void DestroyMidBossEffect()
    {
        Destroy(effect);
    }

    private void RushToPlayer()
    {
        _state = MidBossState.Rush;
        Debug.Log("rush to player");
        PlayOneShotAnim((int)MidBossAnimEnum.Dash);
        Vector3 targetPos = _player.transform.position;
        float rushSpeed = 70.0f;
        _rb.velocity = new Vector3(Mathf.Sign(targetPos.x - transform.position.x) * rushSpeed, 0);
    }

    private void DropObject()
    {
        StartCoroutine(coChangeTrackState(false));
        float jumpPower = 10.0f;
        _state = MidBossState.Drop;
        StartCoroutine(coDropObject(jumpPower, 2, 5));
    }

    private IEnumerator coDropObject(float jumpPower, int jumpCount, int objectNumber)
    {
        for (int i = 0; i < jumpCount; i++)
        {
            //rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            PlayOneShotAnim((int)MidBossAnimEnum.Jump);
            yield return new WaitForSeconds(0.5f);
            _camera.ImpulseCamera(CameraController.impulseCameraType.burst);
        }

        _camera.ImpulseCamera(CameraController.impulseCameraType.burst);

        int[] dropIdx = new int[objectNumber];
        dropIdx = Util.GetRandomIntArray(0, _dropObjectPositions.Length - 1, objectNumber);
        foreach (int drop in dropIdx)
        {
            GameObject go = Instantiate(_dropObjectPrefab);
            go.transform.position = _dropObjectPositions[drop];
            go.GetComponent<DroppableEnemyObject>()._lifeTime = 0.0f;
        }
        StartCoroutine(coChangeTrackState(true, 0.5f));

        yield return new WaitForSeconds(1.2f);
        _camera.ImpulseCamera(CameraController.impulseCameraType.burst);
    }

    private void EmitGas()
    {
        StartCoroutine(coChangeTrackState(false));
        StartCoroutine(coEmitGas());
    }

    private IEnumerator coEmitGas()
    {
        FlipSprite(Mathf.Sign(_player.transform.position.x - transform.position.x));
        PlayOneShotAnim((int)MidBossAnimEnum.Breath);
        yield return new WaitForSeconds(0.8f);
        _gas.SetActive(true);
        _gas.GetComponent<Animator>().SetTrigger("Play");
        StartCoroutine(coSetActive(_gas, false, 1.5f));
        // Vector3 posOffset = new Vector3(3.0f, 0.8f);
        // GameObject go = Instantiate(_gasPrefab, transform);
        // go.transform.position += posOffset;
        // go.transform.localScale = new Vector3(go.transform.localScale.x * Mathf.Sign(transform.localScale.x), go.transform.localScale.y, go.transform.localScale.z);
        // StartCoroutine(coDestory(go, 1.5f));
        StartCoroutine(coChangeTrackState(true, 1.5f));
    }

    private bool CheckStandGround()
    {
        RaycastHit2D hits = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, 0.1f, LayerMask.GetMask("Floor") | LayerMask.GetMask("Platform"));
        if (hits.transform != null)
            return true;
        return false;
    }

    private void JumpAttack()
    {
        StartCoroutine(coChangeTrackState(false));
        StartCoroutine(coJumpAttack(3));
    }

    private IEnumerator coJumpAttack(int jumpCount)
    {
        for (int i = 0; i < jumpCount; i++)
        {
            FlipSprite(Mathf.Sign(_player.transform.position.x - transform.position.x));
            yield return new WaitForSeconds(0.5f);
            float distX = _player.transform.position.x - transform.position.x;
            FlipSprite(Mathf.Sign(distX));
            PlayOneShotAnim((int)MidBossAnimEnum.Jump);
            yield return new WaitForSeconds(0.2f);
            _rb.AddForce(new Vector2(distX, 40.0f), ForceMode2D.Impulse);
            yield return new WaitWhile(() => CheckStandGround()); // 땅에서 공중으로 올라갈 때까지 대기
            yield return new WaitUntil(() => CheckStandGround()); // 공중에서 땅에 도착할 때까지 대기

            _camera.ImpulseCamera(CameraController.impulseCameraType.crash);

            Collider2D hits = Physics2D.OverlapBox(
                transform.position, new Vector2(4.0f, 3.0f), 0, LayerMask.GetMask("Player"));
            if (hits != null)
            {
                yield return new WaitForSeconds(0.3f);
                hits.GetComponent<PlayerController>().Damaged(10, gameObject);
                _player.GetComponent<Rigidbody2D>().AddForce(
                    new Vector2(Mathf.Sign(_player.transform.position.x - transform.position.x) * 6.0f, 0), ForceMode2D.Impulse);
            }
            _rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.5f);
            _state = MidBossState.Jump;
            PlayLoopAnim((int)MidBossAnimEnum.Idle);
        }
        StartCoroutine(coChangeTrackState(true, 0.5f));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        // 보스가 데미지 입는 경우의 Tag도 추가해 주어야 함
        if (!other.gameObject.CompareTag("Player"))
            return;
        PlayerController player = other.GetComponent<PlayerController>();
        player.Damaged(10);
        player.KnockBack(transform.position);
        
    }
}
