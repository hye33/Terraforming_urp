using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MapScene : MonoBehaviour
{
    public Define.LoadMap _map;

    public Action<Define.ForestEnemyType> GetSkill;
    public Action<Define.ForestEnemyType> AddSkillGuage;
    public Action<float> AddTerraformingGauge; // 테라포밍 게이지 업데이트 이벤트

    private float _terraformingGauge = 0;
    private float _monsterTerraforming = 0; // 몬스터 처치로 획득한 테라포밍 게이지 
    private float _maxMonsterTerraforming = 60; // 몬스터로 획득 가능한 최대 테라포밍 게이지 
    private float _inflaTerraforming = 0; // 염증 처치로 획득한 테라포밍 게이지  
    private float _maxInflaTerraforming = 40; // 염증으로 획득 가능한 최대 테라포밍 게이지 

    // BGM
    private AudioClip forestBgm;
    private AudioClip forestBossBgm;

    private PlayerController _player;
    public PlayerController Player { get => _player; }
    private GameObject MainCamera;
    private Vector3 _playerInitPos = new Vector3(-6.5f, 13.3f, 0.0f);
    private GameObject _savePointPrefab;
    private GameObject[] _savePoints = new GameObject[3];
    private Vector3[] _savePointsPos = new Vector3[3];
    public Vector3[] SavePointPos { get => _savePointsPos; }
    [SerializeField]
    private int _savePointCount = 0;
    public int SavePointCount { get => _savePointCount; }
    private int _currentIndex = 1;
    private int[] _killEnemyCount = new int[(int)Define.ForestEnemyType.MaxCount];
    private bool[] _getSkill = new bool[(int)Define.ForestEnemyType.MaxCount];
    private int[] _skillGuage = new int[(int)Define.ForestEnemyType.MaxCount];
    public int[] KillEnemyCount { get => _killEnemyCount; private set { _killEnemyCount = value; } }
    public int[] SkillGuage { get => _skillGuage; }

    private EnemySpawner[] _enemySpawners;

    public bool IsStop = false;

    public int _currentSavepointIndex = 0;

    private UI_Pause _pauseUI;

    private Puzzle _puzzleUI;

    private UI_Record _recordUI;
    public Puzzle PuzzleUI { set { _puzzleUI = value; } }
    public float PlayTime = 0; //플레이타임 기록 변수

    private Volume _globalVolume;
    private Vignette _vignette;

    private void SettingScene()
    {
        //_player = Instantiate(Resources.Load<GameObject>("Prefabs/Sprites/Player"), transform).GetComponent<PlayerController>();
        forestBgm = Managers.Resource.Load<AudioClip>("Sounds/Bgm/ForestMapFieldBgm");
        forestBossBgm = Managers.Resource.Load<AudioClip>("Sounds/Bgm/ForestBossBgm");

        GameObject map = null;
        switch ((int)_map)
        {
            case (int)Define.LoadMap.ForestMap:
                map = (GameObject)Instantiate(Resources.Load("Prefabs/Sprites/ForestMap"), transform);
                _player = FindObjectOfType<PlayerController>();
                Managers.Sound.Play(forestBgm, Define.Sound.Bgm);
                //_player.transform.position = _playerInitPos;                
                Debug.Log("Managers.Game.SaveData.bossDie " + Managers.Game.SaveData.bossDie);
                if (Managers.Game.SaveData.bossDie || Managers.Game.SaveData.enterBossStage && !Managers.Game.SaveData.bossDie)
                    _player.transform.position = new Vector3(133.5f, -4.0f, 0);
                break;
            case (int)Define.LoadMap.ForestBoss:
                map = (GameObject)Instantiate(Resources.Load("Prefabs/Sprites/ForestBossMap"), transform);
                _player = FindObjectOfType<PlayerController>();
                Managers.Sound.Play(forestBossBgm, Define.Sound.Bgm);
                Managers.Game.SaveData.enterBossStage = true;
                //_player.transform.position = new Vector3(0, 0, 0);
                break;
        }

        // Instantiate(Resources.Load<GameObject>("Prefabs/Main Camera"));
        // Camera.main.transform.GetChild(0).GetComponent<CinemachineConfiner>().m_BoundingVolume
        //     = map.transform.GetChild(0).GetComponent<BoxCollider>();

        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;

        UI_Character characterUI = Managers.UI.ShowSceneUI<UI_Character>("CharacterUI");
        Managers.UI.SetCanvas(characterUI.gameObject);

        // 저장된 데이터 불러오기
        _terraformingGauge = Managers.Game.SaveData.terraformingGauge;
        _monsterTerraforming = Managers.Game.SaveData.monsterTerraforming;
        _inflaTerraforming = Managers.Game.SaveData.inflaTerraforming;
        PlayTime = Managers.Game.SaveData.playTime;
    }


    private void Awake()
    {
        SettingScene();
        _savePointPrefab = Resources.Load<GameObject>("Prefabs/Sprites/SavePoint");
        _savePointsPos[0] = _player.transform.position;

        _enemySpawners = FindObjectsOfType<EnemySpawner>();
        _globalVolume = FindObjectOfType<Volume>();
        if (_globalVolume != null)
            _globalVolume.profile.TryGet(out _vignette);

        for (int i = 0; i < (int)Define.ForestEnemyType.MaxCount; i++)
        {
            _killEnemyCount[i] = 0;
            _getSkill[i] = false;
            _skillGuage[i] = Managers.Data.EnemyDict[1000 + i + 1].skillGauge;
        }
    }

    private void Start()
    {
        Managers.Input.UIKeyAction = null;
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;
    }

    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.Esc:
                //if (_pauseUI == null && _puzzleUI == null && _recordUI == null)
                if(Time.timeScale != 0)
                {
                    _pauseUI = Managers.UI.ShowPopupUI<UI_Pause>("PauseUI");
                    Managers.UI.SetCanvas(_pauseUI.gameObject);
                    Time.timeScale = 0;
                    Debug.Log("Game Pause");
                }
                else
                {
                    Managers.UI.ClosePopupUI(_pauseUI);
                }
                break;

            case Define.KeyEvent.Tab:
                Debug.Log(_recordUI);
                Debug.Log(_pauseUI);
                if (_pauseUI == null && _recordUI == null)
                {
                    _recordUI = Managers.UI.ShowPopupUI<UI_Record>("RecordUI");
                    Managers.UI.SetCanvas(_recordUI.gameObject);
                }
                break;
        }
    }

    public void MakeSavePoint(Vector3 pos)
    {
        if (Player._jumpCount > 0)
            return;
        _savePointCount++;
        GameObject go = Instantiate(_savePointPrefab, transform);
        go.transform.position = pos + new Vector3(0, 1.5f, 0);

        Destroy(_savePoints[_currentIndex]);
        _savePoints[_currentIndex] = go;

        // if (_savePointCount == 3)
        // {
        //     Destroy(_savePoints[_currentIndex]);
        //     _savePoints[_currentIndex] = go;
        //     _savePointCount = 2;
        // }
        // _savePointsPos[_currentIndex] = pos;
        // _savePoints[_currentIndex] = go;

        // if (_currentIndex == 1)
        // {
        //     go.GetComponent<SavePoint>().index = 1;
        //     _currentIndex = 2;
        // }
        // else
        // {
        //     go.GetComponent<SavePoint>().index = 2;
        //     _currentIndex = 1;
        // }
    }

    public void AddKillCount(Define.ForestEnemyType type)
    {
        _killEnemyCount[(int)type]++;
        Debug.Log(type + ", " + _killEnemyCount[(int)type]);
        if (AddSkillGuage != null)
        {
            AddSkillGuage.Invoke(type);
        }
        if (_killEnemyCount[(int)type] == _skillGuage[(int)type])
        {
            _getSkill[(int)type] = true;
            if (GetSkill != null)
            {
                GetSkill.Invoke(type);
            }
        }

    }

    public void UpdateTerraformingGauge(float amount, string type) // 테라포밍 게이지 증가
    {
        Debug.Log(amount + type);
        switch (type)
        {
            case "Enemy":
                if (_monsterTerraforming < _maxMonsterTerraforming)
                {
                    _monsterTerraforming += amount;
                    _terraformingGauge += amount;
                    Managers.Game.SaveData.monsterTerraforming = _monsterTerraforming;
                    Managers.Game.SaveData.terraformingGauge = _terraformingGauge;
                }
                break;

            case "Infla":
                if (_inflaTerraforming < _maxInflaTerraforming)
                {
                    _inflaTerraforming += amount;
                    _terraformingGauge += amount;
                    Managers.Game.SaveData.inflaTerraforming = _inflaTerraforming;
                    Managers.Game.SaveData.terraformingGauge = _terraformingGauge;
                }
                break;

            default:
                _terraformingGauge = Mathf.Clamp(_terraformingGauge + amount, 0, 90);
                Managers.Game.SaveData.terraformingGauge = _terraformingGauge;
                break;
        }
        Debug.Log(_terraformingGauge);

        if (_terraformingGauge >= 100)
        {
            foreach (var spawner in _enemySpawners)
            {
                spawner.isActive = false;
                spawner.DeadEnemy();
            }
        }
        else if (_terraformingGauge < 100 && _terraformingGauge >= 90)
        {
            _vignette.intensity.Override(0.0f);
            Managers.Game.SaveData.playerMeleeDamage += 3;
            Managers.Game.SaveData.playerRangeDamage += 3;
        }
        else if (_terraformingGauge < 90 && _terraformingGauge >= 60)
        {
            _vignette.intensity.Override(0.25f);
            Managers.Game.SaveData.savePointHeal += 2;
        }
        else if (_terraformingGauge < 60 && _terraformingGauge >= 30)
        {
            _vignette.intensity.Override(0.6f);
            Managers.Game.SaveData.playerMaxHp += 30;
        }

        _terraformingGauge = Mathf.Clamp(_terraformingGauge, 0, 100);
        AddTerraformingGauge.Invoke(_terraformingGauge);
    }

    private void Update() // 플레이타임 증가
    {
        if (_pauseUI == null)
        {
            PlayTime += Time.fixedDeltaTime;
        }
    }
}