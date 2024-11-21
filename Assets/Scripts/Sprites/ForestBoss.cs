using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestBoss : Boss
{
    protected enum BossAnimState { Idle, Blink, Hit_wall_L, Hit_wall_R, Hit_wall_C, ThornBullet, Laser, SpiderEgg }

    [SerializeField] MapScene _scene;

    PlayerController _player;
    CameraController _camera;
    Vector3 initPos = new Vector3(0, -1, 0);

    // 보스 눈 기준
    Vector3 positionOffset = new Vector3(0, 6.6f, 0);

    float firePower = 10.0f;

    bool isTrack = true;

    float _moveSpeed = 1.0f;

    Vector2 zeroVelo = Vector2.zero;

    // measurement
    [SerializeField] private GameObject _dropObjectGameObj;
    Vector3[] _dropObjectPositions;
    [SerializeField] private GameObject _bossPosGameObj;
    Vector3[] _bossPositions;

    GameObject _damagePrefab;
    GameObject _bulletPrefab;
    GameObject _bombPrefab;
    GameObject _warningPrefab;
    GameObject _groundThornPrefab;
    GameObject _wallThornPrefab;
    GameObject _laserPrefab;
    GameObject _chipsPrefab;
    GameObject _warningSpacePrefab;
    Ground[] _grounds;

    private int[] _mapSize = new int[] { -10, 28, 15, -28 };

    // 각 변의 정가운데 위치
    private Vector3[] _mapPos = new Vector3[]{
        new Vector3(0, -12, 0),
        new Vector3(24, 0, 0),
        new Vector3(0, 16, 0),
        new Vector3(-24, 0, 0)};

    private void StatSetting()
    {
        _maxHp = Managers.Data.EnemyDict[1100].hp;
        _hp = _maxHp;
    }

    private void SetPositions()
    {
        _dropObjectPositions = Util.GetAllChildPositions(_dropObjectGameObj);
        _dropObjectGameObj.SetActive(false); // active 해제
        _bossPositions = Util.GetAllChildPositions(_bossPosGameObj);
        _bossPosGameObj.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        base.BaseInit();

        _scene = FindObjectOfType<MapScene>();

        _rb = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<PlayerController>();
        _camera = Camera.main.GetComponent<CameraController>();
        initPos = transform.position;

        _damagePrefab = Resources.Load<GameObject>("Prefabs/Sprites/DamageRange");
        _bulletPrefab = Resources.Load<GameObject>("Prefabs/Sprites/ThornBullet");
        _bombPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Effect/ForestBossBomb");
        _warningPrefab = Resources.Load<GameObject>("Prefabs/Sprites/WarningSimbol");
        _groundThornPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Effect/GroundThorn");
        _laserPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Effect/Laser");
        _grounds = FindObjectsOfType<Ground>();
        _wallThornPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Effect/WallThorns");
        _chipsPrefab = Resources.Load<GameObject>("Effect/Chips");
        _warningSpacePrefab = Resources.Load<GameObject>("Prefabs/Sprites/Effect/WarningSpace");
    }

    private void Start()
    {
        StatSetting();
        SetPositions();
        PlayLoopAnim((int)BossAnimState.Idle);
        Invoke("Phase1", 3.0f); // 몇 초 후에 1페이즈 시작할지
        // 눈 열림
    }

    private void ChangeState(bool isActive)
    {
        isTrack = !isActive;
        canAttacked = !isActive;
        _rb.velocity = Vector2.zero;
    }
    public override void Death()
    {
        Managers.Game.SaveData.bossDie = true;
        Managers.Game.SaveData.enterBossStage = false;
        _scene.UpdateTerraformingGauge(20, "Boss");
        SceneManager.LoadScene("Forest");
    }

    private void FixedUpdate()
    {
        TrackingPlayer();
    }

    private void TrackingPlayer()
    {
        if (isTrack == false)
            return;
        //transform.position = Vector2.SmoothDamp(transform.position, _player.transform.position, ref zeroVelo, 2f);
        _rb.velocity = (_player.transform.position - transform.position).normalized;
    }

    private IEnumerator coMoveToRandomPos()
    {
        float speed = 3.0f;
        while (true)
        {
            int idx = Random.Range(0, _bossPositions.Length);
            Vector3 newPos = _bossPositions[idx];
            Vector3 dir = newPos - transform.position;
            _rb.velocity = dir.normalized * speed;
            yield return new WaitForSeconds(5.0f);
        }
    }

    private IEnumerator coGoToInitPos()
    {
        while((transform.position - initPos).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.SmoothDamp(transform.position, initPos, ref zeroVelo, 1f);
            yield return null;
        }
    }

    private IEnumerator coShake(GameObject go, float shakeDuration, float shakeMagnitude)
    {
        Vector3 initialPosition = go.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration)
        {
            go.transform.position = initialPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        go.transform.position = initialPosition;
    }

    private IEnumerator coShowWarning(Vector2 pos, float time)
    {
        GameObject simbol = Instantiate(_warningPrefab);
        simbol.transform.position = pos;
        yield return new WaitForSeconds(time);
        Destroy(simbol);
    }

    private void ReturnIdleAnim()
    {
        PlayLoopAnim((int)BossAnimState.Idle);
        PlayOneShotAnim((int)BossAnimState.Blink);
    }

    private int CheckPhase()
    {
        if (_hp < _maxHp * 0.6f)
        {
            if (_hp < _maxHp * 0.25f)
                return 3;
            else
                return 2;
        }
        else
            return 1;
    }

    private void Phase1()
    {
        ChangeState(isActive: false);
        StartCoroutine(coPhase1());
    }

    private IEnumerator coPhase1()
    {
        while (true)
        {
            FireBossAndBurstThorns();
            yield return new WaitForSeconds(6.5f);
            DropObjectRandomly(_bombPrefab);
            yield return new WaitForSeconds(6.0f);
            GroundThorns();
            yield return new WaitForSeconds(5.0f);
            EyeLaserEx();
            yield return new WaitForSeconds(10.0f);
            BurstCircleThorns(number: 15);
            yield return new WaitForSeconds(7.0f);
        }
    }

    private IEnumerator coPhase2()
    {
        while (true)
        {
            FireBossAndBurstThorns();
            yield return new WaitForSeconds(7.0f);
            Lighting();
            yield return new WaitForSeconds(7.0f);
            DropObjectRandomly(_bombPrefab);
            yield return new WaitForSeconds(7.0f);
            EyeLaserEx();
            yield return new WaitForSeconds(12.0f);
            BurstCircleThorns(number: 15);
            yield return new WaitForSeconds(8.0f);
        }
    }

    private IEnumerator coPhase3()
    {
        while (true)
        {
            FireBossAndBurstThorns();
            yield return new WaitForSeconds(7.0f);
            GroundThorns();
            yield return new WaitForSeconds(7.0f);
            EyeLaser();
            yield return new WaitForSeconds(12.0f);
            BurstCircleThorns(number: 15);
            yield return new WaitForSeconds(8.0f);
        }
    }

    #region FireBossAndBurstThorns
    private void FireBossAndBurstThorns()
    {
        ChangeState(isActive: true);
        int fireDir = Random.Range(0, 4);
        switch (fireDir)
        {
            case 0:
                PlayOneShotAnim((int)BossAnimState.Hit_wall_C);
                StartCoroutine(coFireBoss(Vector2.down, fireDir));
                break;
            case 1:
                PlayOneShotAnim((int)BossAnimState.Hit_wall_R);
                StartCoroutine(coFireBoss(Vector2.right, fireDir));
                break;
            case 2:
                PlayOneShotAnim((int)BossAnimState.Hit_wall_C);
                StartCoroutine(coFireBoss(Vector2.up, fireDir));
                break;
            case 3:
                PlayOneShotAnim((int)BossAnimState.Hit_wall_L);
                StartCoroutine(coFireBoss(Vector2.left, fireDir));
                break;
        }
    }

    private IEnumerator coFireBoss(Vector2 dir, int posIdx)
    {
        if (posIdx == 0 || posIdx == 2)
        {
            while (Mathf.Abs(transform.position.y - _mapPos[posIdx].y) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector3(transform.position.x, _mapPos[posIdx].y, 0),
                    0.3f); // 마지막 인수: 속도
                yield return null;
            }
        }
        else
        {
            while (Mathf.Abs(transform.position.x - _mapPos[posIdx].x) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector3(_mapPos[posIdx].x, transform.position.y, 0),
                    0.3f); // 마지막 인수: 속도
                yield return null;
            }
        }
        StartCoroutine(coReturnInitPos());
        _camera.ImpulseCamera(CameraController.impulseCameraType.crash);
        yield return new WaitForSeconds(0.3f); // 부딪히고 난 뒤 경고 표시까지의 텀
        ShowWarningThorn(posIdx);
        yield return new WaitForSeconds(2.6f); // 부딪히고 난 뒤 가시 발사까지의 텀
        _camera.ImpulseCamera(CameraController.impulseCameraType.crash);
        BurstThorns(posIdx);
    }

    private void ShowWarningThorn(int dir)
    {
        GameObject waring = Instantiate(_warningSpacePrefab);
        waring.transform.rotation = Quaternion.Euler(0, 0, dir * 90);
        waring.transform.position = new Vector3(_mapPos[dir].x, _mapPos[dir].y, 0);

        switch (dir)
        {
            case 0: // 아래
                waring.transform.localScale = new Vector3(48, 11.5f, 1);
                break;
            case 1: // 오른쪽
                waring.transform.localScale = new Vector3(48, 24, 1);
                break;
            case 2: // 위
                waring.transform.localScale = new Vector3(48, 16, 1);
                break;
            case 3: // 왼쪽
                waring.transform.localScale = new Vector3(48, 24, 1);
                break;
        }
    }

    private void BurstThorns(int idx)
    {
        GameObject go = Instantiate<GameObject>(_damagePrefab);
        GameObject thorns = Instantiate<GameObject>(_wallThornPrefab);
        GameObject effect = Instantiate(_chipsPrefab, thorns.transform);
        go.transform.rotation = Quaternion.Euler(0, 0, idx * 90);
        thorns.transform.rotation = Quaternion.Euler(0, 0, idx * 90);
        // 돌진 방향이 상하일 경우
        if (idx == 0 || idx == 2)
        {
            thorns.transform.position = new Vector3(_mapPos[idx].x, idx == 0 ? _mapPos[idx].y - 8 : _mapPos[idx].y + 4, 0);
            thorns.transform.localScale = new Vector3(1, 1.3f, 1);
            go.transform.position = new Vector3(_mapPos[idx].x, _mapPos[idx].y, 0);
            go.transform.localScale = new Vector3(48, idx == 2 ? 16 : 9.5f, 1);
        }
        // 돌진 방향이 좌우일 경우
        else
        {
            thorns.transform.position = new Vector3(idx == 1 ? _mapPos[idx].x + 6 : _mapPos[idx].x - 6, _mapPos[idx].y, 0);
            thorns.transform.localScale = new Vector3(1, 2, 1);
            go.transform.position = new Vector3(_mapPos[idx].x, _mapPos[idx].y, 0);
            go.transform.localScale = new Vector3(16, 24, 1);
        }
        StartCoroutine(coBurst(_mapPos[idx], go, thorns, 0.2f, 1.5f));
    }

    private IEnumerator coBurst(Vector3 target, GameObject go, GameObject thorns,  float t, float destroyTime)
    {
        // while((go.transform.position - target).sqrMagnitude > 0.01f)
        // {
        //     go.transform.position = Vector2.SmoothDamp(go.transform.position, target, ref zeroVelo, t);
        //     yield return null;
        // }
        yield return new WaitForSecondsRealtime(destroyTime);
        Destroy(go);
        Destroy(thorns);
        ChangeState(isActive: false);
        ReturnIdleAnim();
    }

    private IEnumerator coReturnInitPos()
    {
        yield return new WaitForSecondsRealtime(1.75f);
        while ((transform.position - initPos).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.SmoothDamp(transform.position, initPos, ref zeroVelo, 0.5f);
            yield return null;
        }
    }
    #endregion

    #region DropObjectRandomly
    private void DropObjectRandomly(GameObject prefab)
    {
        ChangeState(isActive: true);
        StartCoroutine(coDropObjectRandomly(prefab));
    }

    private IEnumerator coDropObjectRandomly(GameObject prefab)
    {
        int number = 5;
        int[] dropIdx = Util.GetRandomIntArray(0, _dropObjectPositions.Length - 1, number); // 선택된 인덱스
        Vector3[] dropPos = new Vector3[number]; // 인텍스 + 오프셋값
        PlayOneShotAnim((int)BossAnimState.SpiderEgg);
        yield return new WaitForSeconds(1.0f);

        foreach (int idx in dropIdx)
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = _dropObjectPositions[idx];
        }

        yield return new WaitForSeconds(4.0f);
        ChangeState(isActive: false);
        ReturnIdleAnim();
    }
    #endregion

    #region GroundThorns
    private void GroundThorns()
    {
        ChangeState(isActive: true);
        StartCoroutine(coGroundThorns());
    }

    private IEnumerator coGroundThorns()
    {
        int number = 7;
        int[] shakeIdx = Util.GetRandomIntArray(0, _grounds.Length - 1, number);
        foreach (int idx in shakeIdx)
        {
            StartCoroutine(coShake(_grounds[idx].gameObject, 1.0f, 0.2f));
        }

        yield return new WaitForSecondsRealtime(1.5f);

        _camera.ImpulseCamera(CameraController.impulseCameraType.burst);
        foreach (int idx in shakeIdx)
        {
            GameObject effect = Instantiate(_chipsPrefab, _grounds[idx].transform);
            //GameObject damage = Instantiate(_damagePrefab, _grounds[idx].transform);
            GameObject thorn = Instantiate(_groundThornPrefab, _grounds[idx].transform);
            //damage.transform.localScale = new Vector3(5, 3, 0);
            Vector3 posOffset = new Vector3(0, -0.3f);
            thorn.transform.position = _grounds[idx].transform.position + posOffset;
            thorn.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }

        yield return new WaitForSecondsRealtime(2.0f);

        foreach (int idx in shakeIdx)
        {
            foreach (Transform child in _grounds[idx].transform)
            {
                Destroy(child.gameObject);
            }
        }
        ChangeState(isActive: false);
    }
    #endregion

    #region EyeLaser
    private void EyeLaser()
    {
        ChangeState(isActive: true);
        StartCoroutine(coEyeLaser());
    }

    private IEnumerator coEyeLaser()
    {
        Vector2 posOffset = new Vector2(0.06f, -1.04f);
        Vector2 mainPos = transform.position + positionOffset;
        PlayOneShotAnim((int)BossAnimState.Laser, returnLoop: false);
        yield return new WaitForSeconds(2.0f);
        GameObject go = Instantiate(_laserPrefab);
        go.transform.position = mainPos + posOffset;
        go.transform.localRotation = Quaternion.Euler(0, 0, 180);

        yield return new WaitForSeconds(2.0f);

        ReturnIdleAnim();
        Destroy(go);
        ChangeState(isActive: false);
    }

    private void EyeLaserEx()
    {
        StartCoroutine(coGoToInitPos());
        ChangeState(isActive: true);
        StartCoroutine(coEyeLaserEx());
    }

    private IEnumerator coLaserTracking(GameObject laser)
    {
        float runTime = 10.0f;
        float t = 0.0f;

        Vector2 targetPos = _player.transform.position;
        Vector2 mainPos = transform.position + positionOffset;
        Vector2 v = targetPos - mainPos;
        Vector2 pastV;
        float offset;
        Vector3 euler;
        laser.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-v.x, v.y) * Mathf.Rad2Deg);

        v = v.normalized;

        yield return new WaitForSeconds(0.2f);

        // while (t < runTime)
        // {
        //     targetPos = _player.transform.position;
        //     v = targetPos - mainPos;
        //     offset = Mathf.Atan2(-v.x, v.y) * Mathf.Rad2Deg * Time.deltaTime * 0.1f;
        //     euler = laser.transform.eulerAngles + new Vector3(0, 0, offset);
        //     laser.transform.eulerAngles = euler;
        //     yield return null;
        //     t += Time.deltaTime;
        // }
        // while (t < runTime)
        // {
        //     targetPos = _player.transform.position;
        //     pastV = v;
        //     v = (targetPos - mainPos).normalized;
        //     if (Vector2.Dot(Vector2.up, v) < Vector2.Dot(Vector2.up, pastV))
        //     offset = Mathf.Atan2(-v.x, v.y) * Mathf.Rad2Deg * Time.deltaTime * 0.1f;
        //     euler = laser.transform.eulerAngles + new Vector3(0, 0, offset);
        //     laser.transform.eulerAngles = euler;
        //     yield return null;
        //     t += Time.deltaTime;
        // }
    }

    private IEnumerator coEyeLaserEx()
    {
        yield return new WaitForSeconds(2.0f);
        PlayOneShotAnim((int)BossAnimState.Laser, returnLoop: false);
        yield return new WaitForSeconds(1.5f);
        Vector2 posOffset = new Vector2(0.06f, -1.04f);
        Vector2 targetPos = _player.transform.position;
        Vector2 mainPos = transform.position + positionOffset;
        yield return new WaitForSeconds(0.5f);
        GameObject go = Instantiate(_laserPrefab, transform);
        Vector2 v = targetPos - mainPos;
        go.transform.position = mainPos;
        go.transform.localScale = new Vector3(1, 1, 1);
        // go.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //yield return new WaitForSecondsRealtime(3.0f);

        // for (int i = 0; i < 2; i++)
        // {
        //     targetPos = _player.transform.position;
        //     mainPos = transform.position + positionOffset;
        //     go.SetActive(false);
        //     yield return new WaitForSeconds(0.5f); // 레이저 사라져 있는 시간
        //     v = targetPos - mainPos;
        //     go.SetActive(true);
        //     go.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(-v.x, v.y) * Mathf.Rad2Deg);
        //     yield return new WaitForSecondsRealtime(1.5f); // 레이저 보이는 시간
        // }
        float angle = 0;
        while(angle < 180)
        {
            angle += 50 * Time.deltaTime;
            go.transform.eulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }
        //StartCoroutine(coLaserTracking(go));
        //yield return new WaitForSeconds(10.5f);
        ReturnIdleAnim();
        Destroy(go);
        ChangeState(isActive: false);
    }
    #endregion

    #region BurstCircleThorns
    private void BurstCircleThorns(int number)
    {
        ChangeState(isActive: true);
        StartCoroutine(coBurstCircleThorns(number));
    }

    private IEnumerator coBurstCircleThorns(int number)
    {
        int n = number;
        yield return new WaitForSecondsRealtime(3.0f);
        PlayOneShotAnim((int)BossAnimState.ThornBullet, returnLoop: false);
        _camera.ImpulseCamera(CameraController.impulseCameraType.burst);
        ShootBullets(n, 20.0f, false);
        yield return new WaitForSecondsRealtime(1.5f);
        PlayOneShotAnim((int)BossAnimState.ThornBullet, returnLoop: false);
        _camera.ImpulseCamera(CameraController.impulseCameraType.burst);
        ShootBullets(n, 20.0f, true);
        yield return new WaitForSeconds(1.0f);
        ChangeState(isActive: false);
        ReturnIdleAnim();
    }

    private void ShootBullets(int n, float speed, bool odd)
    {
        float rad_step = Mathf.PI * 2 / n;
        float rad = odd ? rad_step / 2 : 0;

        for (int i = 0; i < n; i++, rad += rad_step)
        {
            GameObject go = Instantiate(_bulletPrefab);
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            go.GetComponent<Bullet>().SetTermBullet(dir, transform.position + positionOffset, false, 20, speed, false, 0.2f, 0.5f);
        }
    }
    #endregion

    #region Lighting
    private void Lighting()
    {
        ChangeState(isActive: true);
        StartCoroutine(coLighting());
    }

    private IEnumerator coLighting()
    {
        float xPos = Random.Range(_mapPos[3].x, _mapPos[1].x);
        StartCoroutine(coShowWarning(new Vector2(xPos, 0), 1.0f));
        yield return new WaitForSeconds(1.0f);
        GameObject light = Instantiate(_damagePrefab);
        light.transform.position = new Vector2(xPos, _mapPos[0].y);
        light.transform.localScale = new Vector2(2, 24);
        yield return new WaitForSeconds(1.0f);
        Destroy(light);
        yield return new WaitForSeconds(1.0f);
        ChangeState(isActive: false);
    }

    #endregion

}