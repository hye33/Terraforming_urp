using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManagerEx
{
    [Serializable]
    public class GameData
    {
        public int playerLife;
        public float playerHp;
        public Vector3 playerPosition;
        public int playerLocalScale; // sprite direction
        public Define.Stage stage;
        Define.StageState stageState;
        Vector3[] savepointPos;
        int[] killEnemyCounts;
        bool[] getSkill;
        public bool puzzleSolved = false; // 퍼즐 풀었는지 확인
        public bool miniBossDie = false; // 동굴 미니 보스 죽었는지 확인
        public bool bossDie = false; // 보스 죽었는지 확인
        public bool enterBossStage = false; // 보스전 진입 여부 확인
        public Dictionary<int,bool> getRelic = new Dictionary<int,bool>(); // 플레이어가 획득한 유물
        public Dictionary<int, bool> getRecord = new Dictionary<int, bool>(); // 플레이어가 조사한 것들
        public int[] Enhancement = new int[2] { 5, 5 }; // 무기 강화 수치
        public float terraformingGauge; // 총 테라포밍 게이지 
        public float monsterTerraforming; //몬스터로 획득한 테라포밍 게이지
        public float inflaTerraforming; // 염증으로 획득한 테라포밍 게이지
        public float playTime; // 플레이타임
        public int slotNum; // 저장 파일 슬롯 번호
        float leftChargeCoolTime;
        
        public int playerMeleeDamage = 7; // 근거리 공격력
        public int playerRangeDamage = 17; // 원거리 공격력
        public int savePointHeal = 3; // 회복량
        public int playerMaxHp = 100; // 최대 hp

        public GameData DeepCopy()
        {
            GameData copy = new GameData();
            copy.playerLife = playerLife;
            copy.playerHp = playerHp;
            copy.playerPosition = playerPosition;
            copy.playerLocalScale = playerLocalScale;
            copy.stage = stage;
            return copy;

            // 나머지 수치들도 추가해야됨...
        }
    }
    
    private GameData _gameData = new GameData();
    private GameData _autoSaveData = new GameData();
    public GameData SaveData { get { return _gameData; } set { _gameData = value; } }
    public GameData AutoSaveData { get { return _autoSaveData; } set { _autoSaveData = value;} }

    public Define.Stage CurrentStage { get; private set; } = Define.Stage.Forest;

    private int autoSaveNum; // 자동 저장 번호(순서)
    public int AutoSaveNum { get { return autoSaveNum; } set { autoSaveNum = value; SaveAutoData(); } }

    private void SaveAutoData()
    {
        SaveGameData(_gameData.slotNum);
        AutoSaveData = _gameData.DeepCopy();
    }

    public void SetGameDataToSaved()
    {
        _gameData = AutoSaveData.DeepCopy();
        SaveGameData(_gameData.slotNum);
    }

    public void SaveGameData(int slotNum)
    {
        _gameData.slotNum = slotNum;
        string jsonData = JsonUtility.ToJson(_gameData, true); // 데이터를 JSON으로 직렬화
        string path = Path.Combine(Application.persistentDataPath, $"gamedata_slot_{slotNum}.json"); // 파일 경로 지정
        File.WriteAllText(path, jsonData); // 파일에 JSON 데이터 저장
        Debug.Log($"파일 {slotNum}에 저장 완료: {path}");

        Debug.Log("UserSave_auto: " + _autoSaveData.playerPosition);
        Debug.Log("UserSave_gameData: " + _gameData.playerPosition);
    }
    public void LoadGameData(int slotNum)
    {
        Debug.Log("LoadGameData");
        string path = Path.Combine(Application.persistentDataPath, $"gamedata_slot_{slotNum}.json"); // 파일 경로 지정
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path); // 파일에서 JSON 데이터를 읽어옴
            _gameData = JsonUtility.FromJson<GameData>(jsonData); // JSON 데이터를 GameData 객체로 역직렬화
            Debug.Log($"게임 데이터 슬롯 {slotNum}에서 불러오기 완료");
        }
        else
        {
            Debug.LogWarning("저장된 게임 데이터가 없습니다.");
            _gameData.slotNum = -1;
        }
    }
    public int nowSlot; // 현재 슬롯번호
    public void DataClear()
    {
        nowSlot = -1;
        _gameData = new GameData();
    }
    public void DeleteGameData(int slotNum)
    {
        // 세이브 파일 경로 생성
        string path = Path.Combine(Application.persistentDataPath, $"gamedata_slot_{slotNum}.json");

        // 파일이 존재하는지 확인
        if (File.Exists(path))
        {
            // 파일 삭제
            File.Delete(path);
            Debug.Log($"슬롯 {slotNum}의 세이브 데이터를 삭제했습니다.");
        }
        else
        {
            Debug.LogWarning($"슬롯 {slotNum}에 저장된 데이터가 존재하지 않습니다.");
        }
    }
}