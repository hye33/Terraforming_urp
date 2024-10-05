using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UI_Continue : UI_Popup
{
    AudioClip _buttonAudio;
    private int selectedIndex = 0; // 선택된 파일 번호
    private UI_MainMenu _mainMenu;
    public UI_MainMenu UI_MainMenu { set { _mainMenu = value; } }

    private List<LoadFile> loadFiles = new List<LoadFile>(); // 미리 생성된 LoadFile 창 리스트
    private List<Toggle> toggleList = new List<Toggle>(); // 하단 토글 리스트

    enum GameObjects
    {
        PrevBtn,
        NextBtn,
        ExitBtn
    }

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        _buttonAudio = Resources.Load<AudioClip>("Sounds/Effect/cursor6");
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;
        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.PrevBtn).BindEvent((PointerEventData data) => UpdateActiveFiles(-1));
        GetObject((int)GameObjects.NextBtn).BindEvent((PointerEventData data) => UpdateActiveFiles(1));
        GetObject((int)GameObjects.ExitBtn).BindEvent(Close);

        GameObject saveFilesParent = GameObject.Find("SaveFiles");
        GameObject toggleParent = GameObject.Find("SaveFileList");

        LoadFile[] allLoadFiles = saveFilesParent.GetComponentsInChildren<LoadFile>(true);
        Toggle[] allToggles = toggleParent.GetComponentsInChildren<Toggle>(true);

        for (int i = 0; i < allLoadFiles.Length; i++)
        {
            LoadFile file = allLoadFiles[i];
            int index = i; // 파일 번호 할당
            if (i == 0) index = 10;
            file.fileNumber = index;
            loadFiles.Add(file);
            int toggleIndex = (i - 1 + allToggles.Length) % allToggles.Length;
            toggleList.Add(allToggles[toggleIndex]);
        }
        UpdateActiveFiles(1);
    }

    private void UpdateActiveFiles(int dir)
    {
        Managers.Sound.Play(_buttonAudio, Define.Sound.Effect);
        toggleList[selectedIndex].isOn = false;

        int maxFiles = loadFiles.Count;
        selectedIndex = (selectedIndex + dir + loadFiles.Count) % maxFiles;
        toggleList[selectedIndex].isOn = true;

        int previousIndex = (selectedIndex - (2 * dir) + loadFiles.Count) % maxFiles; // 비활성화 되는 파일 인덱스
        int nextIndex = (selectedIndex + (1 * dir) + loadFiles.Count) % maxFiles; // 새롭게 활성화 되는 파일 인덱스
        loadFiles[previousIndex].gameObject.SetActive(false);
        loadFiles[nextIndex].gameObject.SetActive(true);

        if(dir > 0)
        {
            loadFiles[(previousIndex + 1) % maxFiles].transform.SetSiblingIndex(0);
            loadFiles[(previousIndex + 1) % maxFiles].transform.Find("Black").gameObject.SetActive(true);
            loadFiles[(previousIndex + 1) % maxFiles].transform.Find("SelectedImg").gameObject.SetActive(false);
            loadFiles[selectedIndex].transform.SetSiblingIndex(1);
            loadFiles[selectedIndex].transform.Find("Black").gameObject.SetActive(false);
            loadFiles[selectedIndex].transform.Find("SelectedImg").gameObject.SetActive(true);
            loadFiles[nextIndex].transform.SetSiblingIndex(2);  // 다음 파일을 두 번째로 배치
            loadFiles[nextIndex].transform.Find("Black").gameObject.SetActive(true);
            loadFiles[nextIndex].transform.Find("SelectedImg").gameObject.SetActive(false);
        }
        else if (dir < 0)
        {
            loadFiles[(previousIndex - 1 + maxFiles) % maxFiles].transform.SetSiblingIndex(2);
            loadFiles[(previousIndex - 1 + maxFiles) % maxFiles].transform.Find("Black").gameObject.SetActive(true);
            loadFiles[(previousIndex - 1 + maxFiles) % maxFiles].transform.Find("SelectedImg").gameObject.SetActive(false);
            loadFiles[selectedIndex].transform.SetSiblingIndex(1);
            loadFiles[selectedIndex].transform.Find("Black").gameObject.SetActive(false);
            loadFiles[selectedIndex].transform.Find("SelectedImg").gameObject.SetActive(true);
            loadFiles[(nextIndex) % maxFiles].transform.SetSiblingIndex(0);  // 다음 파일을 두 번째로 배치
            loadFiles[(nextIndex) % maxFiles].transform.Find("Black").gameObject.SetActive(true);
            loadFiles[(nextIndex) % maxFiles].transform.Find("SelectedImg").gameObject.SetActive(false);
        }
    }
    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.A:
                UpdateActiveFiles(-1);
                break;

            case Define.KeyEvent.D:
                UpdateActiveFiles(1);
                break;

            case Define.KeyEvent.Space:
                loadFiles[selectedIndex].StartGame(null);
                break;

            case Define.KeyEvent.Esc:
                Close(new PointerEventData(EventSystem.current));
                break;

            case Define.KeyEvent.S:
                loadFiles[selectedIndex].Exit(null);
                break;
        }

    }

    private void Close(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
    }

    private void OnDestroy()
    {
        Managers.Input.UIKeyAction -= InputKey;
        loadFiles.Clear();
        toggleList.Clear();
        _mainMenu.RegisterInputKeyAction();
    }
    public void RegisterInputKeyAction()
    {
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;
    }
}
