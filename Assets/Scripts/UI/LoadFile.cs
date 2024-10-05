using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadFile : UI_Popup
{
    private int _fileNumber;
    public int fileNumber
    {
        set
        {
            _fileNumber = value;
            Init();
        }
    }
    [System.Serializable]
    public class SaveFileData
    {
        public int number;
        public string stage;
        public string playtime;
        public int record;
        public float gameProgress;
    }
    public SaveFileData saveFileData;
    //SaveFileData_Test data;
    enum GameObjects
    {
        NumberTMP,
        StageTitleTMP,
        PlayTimeTMP,
        GetRecordTMP,
        GameProgress,
        ProgressTMP,
        DeleteBtn,
        StartBtn,
        New,
        Data
    }
    public override void Init()
    {
        BindObject(typeof(GameObjects));
        LoadSaveFileInfo(_fileNumber);
        string num = _fileNumber < 10 ? "0" + _fileNumber.ToString() : _fileNumber.ToString(); //10 이하면 앞에 0을 붙임
        GetObject((int)GameObjects.NumberTMP).GetComponent<TextMeshProUGUI>().text = num;
        if (saveFileData == null)
        {
            GetObject((int)GameObjects.New).gameObject.SetActive(true);
            GetObject((int)GameObjects.Data).gameObject.SetActive(false);
            return;
        }
        else
        {
            GetObject((int)GameObjects.New).gameObject.SetActive(false);
            GetObject((int)GameObjects.Data).gameObject.SetActive(true);
        }
        GetObject((int)GameObjects.StageTitleTMP).GetComponent<TextMeshProUGUI>().text = saveFileData.stage;
        GetObject((int)GameObjects.PlayTimeTMP).GetComponent<TextMeshProUGUI>().text = saveFileData.playtime.ToString();
        GetObject((int)GameObjects.GetRecordTMP).GetComponent<TextMeshProUGUI>().text = saveFileData.record.ToString();
        GetObject((int)GameObjects.ProgressTMP).GetComponent<TextMeshProUGUI>().text = saveFileData.gameProgress.ToString();
        GetObject((int)GameObjects.GameProgress).GetComponent<UI_ProgressSlider>().progress = saveFileData.gameProgress;
        GetObject((int)GameObjects.DeleteBtn).BindEvent(Exit);
        GetObject((int)GameObjects.StartBtn).BindEvent(StartGame);
    }
    private void LoadSaveFileInfo(int slotNum)
    {
        Managers.Game.LoadGameData(slotNum);
        GameManagerEx.GameData data = Managers.Game.SaveData;
        // JSON 파일에서 데이터를 불러오는 로직
        // 여기는 임시 데이터 예시

        UnityEngine.Debug.Log(data.slotNum);
        if (data.slotNum == -1) saveFileData = null;
        else
        {
            string stageValue = data.stage.ToString().ToLower();

            // stage가 "Forest"일 때 "숲"으로 변경
            if (stageValue == "forest")
            {
                stageValue = "숲";
            }
            saveFileData = new SaveFileData()
            {
                number = data.slotNum,                      // 슬롯 번호
                stage = stageValue,          // 스테이지 정보
                playtime = FormatPlayTime(data.playTime),   // 플레이타임을 포맷팅하여 할당
                record = data.getRecord.Count,              // getRecord 딕셔너리의 길이
                gameProgress = data.terraformingGauge       // 테라포밍 게이지
            };
            Managers.Game.DataClear();
        }
            
    }

    private string FormatPlayTime(float playTime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(playTime);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
    public void Exit(PointerEventData data)
    {
        UI_DeleteFile deleteFile = Managers.UI.ShowPopupUI<UI_DeleteFile>("DeleteFileUI");
        deleteFile.FileNumber = _fileNumber;
        deleteFile.LoadFileInstance = this;
        Managers.UI.SetCanvas(deleteFile.gameObject, true, CanvasScaler.ScreenMatchMode.Shrink);
    }

    public void StartGame(PointerEventData data) // 세이브 데이터를 전달하도록 수정 
    {
        Managers.Resource.Instantiate("UI/BlackScreen");
        if (saveFileData == null)
        {
            Managers.Game.SaveData.slotNum = _fileNumber;
            UnityEngine.Debug.Log("새 게임 시작");
            SceneManager.LoadScene("Forest");
        }
        else
        {
            UnityEngine.Debug.Log("기존 게임 불러오기 ->" + _fileNumber);
            Managers.Game.LoadGameData(_fileNumber);
            Managers.Game.SaveData.slotNum = _fileNumber;
            SceneManager.LoadScene("Forest");
        }
        Managers.UI.CloseAllPopupUI();
    }

    public void RefreshUI()
    {
        UnityEngine.Debug.Log("데이터 갱신");
        LoadSaveFileInfo(_fileNumber); // 삭제 후 다시 파일 정보를 로드

        // UI 업데이트: 저장된 파일이 없는 경우의 처리
        if (saveFileData == null)
        {
            GetObject((int)GameObjects.New).gameObject.SetActive(true);
            GetObject((int)GameObjects.Data).gameObject.SetActive(false);
            return;
        }
    }

}
