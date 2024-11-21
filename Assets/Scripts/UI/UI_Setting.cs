using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Setting : UI_Popup
{
    private Slider bgmSlider;
    private Slider effectSlider;
    private Slider soundSlider;
    private TMP_Dropdown resolutionDropdown;
    private bool isFullScreen = false;
    private List<Resolution> resolutions = new List<Resolution>
    {
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1280, height = 720 }
    };
    private int resolutionIndex;
    private Transform content;
    private GameObject SaveText;

    private UI_MainMenu _mainMenu = null;
    public UI_MainMenu UI_MainMenu { set { _mainMenu = value; } }
    private UI_Pause _pause = null;
    public UI_Pause UI_Pause { set { _pause = value; } }

    private Color normalColor = new Color32(113, 124, 57, 255); 
    private Color highlightedColor = new Color32(205, 197, 173, 255);

    enum GameObjects
    {
        SoundSlider,
        BGMSlider,
        EffectSlider,
        WindowToggle, 
        ResolutionDropDown, 
        SaveBtn//,
        //SaveTMP,
        //ExitBtn, 
    }

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {

        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;

        BindObject(typeof(GameObjects));
        bgmSlider = GetObject((int)GameObjects.BGMSlider).GetComponent<Slider>();
        effectSlider = GetObject((int)GameObjects.EffectSlider).GetComponent<Slider>();
        soundSlider = GetObject((int)GameObjects.SoundSlider).GetComponent<Slider>();
        resolutionDropdown = GetObject((int)GameObjects.ResolutionDropDown).GetComponent<TMP_Dropdown>();

        Debug.Log(resolutionDropdown); 
        resolutionDropdown.ClearOptions();

        //현재 해상도 가져오기
        Resolution currentResolution = Screen.currentResolution;
        string currentResolutionString = currentResolution.width + "x" + currentResolution.height;
        Debug.Log($"현재 해상도: {currentResolutionString}");

        List<string> options = new List<string>();
        int currentResolutionIndex = -1;
        for (int i = 0; i < resolutions.Count; i++)
        {
            //resolutions.Add(new Resolution { width = settings[i,0], height = settings[i,1] });
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
                resolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        if (currentResolutionIndex != -1)
        {
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.captionText.text = currentResolutionString; // 레이블에 현재 해상도 표시
        }
        else
        {
            // 현재 해상도가 리스트에 없을 경우 기본값으로 첫 번째 설정
            resolutionDropdown.value = 0;
            resolutionDropdown.captionText.text = options[0];
        }

        resolutionDropdown.RefreshShownValue();
        GetObject((int)GameObjects.WindowToggle).BindEvent(SetWindowMode);
        GetObject((int)GameObjects.ResolutionDropDown).BindEvent(SetTextColor);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        GetObject((int)GameObjects.SaveBtn).BindEvent(Save);
        //GetObject((int)GameObjects.ExitBtn).BindEvent(ExitBtn);
        //SaveText = GetObject((int)GameObjects.SaveTMP);
        //SaveText.SetActive(false);

        bgmSlider.value = Managers.Sound.BGM; 
        effectSlider.value = Managers.Sound.Effect;
        soundSlider.value = Managers.Sound.Sound;

        isFullScreen = Screen.fullScreen; // windowed -> false , FullScreen -> true 
        GetObject((int)GameObjects.WindowToggle).GetComponent<Toggle>().isOn = !isFullScreen;

        Debug.Log(isFullScreen);
    }

    private void SetWindowMode(PointerEventData data)
    {
        isFullScreen = !isFullScreen;
        if (isFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Debug.Log("Set FullScreenMode");
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Debug.Log("Set WindowMode");
        }

        //Screen.SetResolution(1920, 1080, isFullScreen);
    }

    private void SetResolution(int index)
    {
        Debug.Log("ChangeResolution - " + resolutionDropdown.options[index].text);
        resolutionIndex = index; 
        resolutionDropdown.captionText.text = resolutionDropdown.options[index].text;
        resolutionDropdown.RefreshShownValue();
    }

    private void SetTextColor(PointerEventData data)
    {
        content = resolutionDropdown.transform.Find("Dropdown List").Find("Viewport").Find("Content");
        Debug.Log(content);
        Debug.Log(resolutionIndex);
        content.GetChild(resolutionIndex).GetComponentInChildren<TMP_Text>().color = normalColor;
        Debug.Log("SetTExtColor : " + resolutionDropdown.value);
        Debug.Log(resolutionDropdown.value + " and " + resolutionIndex);
        resolutionIndex = resolutionDropdown.value + 1;
        content.GetChild(resolutionIndex).GetComponentInChildren<TMP_Text>().color = highlightedColor;
    }
    
    private void Save(PointerEventData data)
    {
        Resolution resolution = resolutions[resolutionIndex];
        int width = resolution.width;
        int height = resolution.height;
        if (isFullScreen)
            Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
        else
            Screen.SetResolution(width, height, FullScreenMode.Windowed);

        Managers.Sound.Sound = soundSlider.value;
        Managers.Sound.BGM = bgmSlider.value;
        Managers.Sound.Effect = effectSlider.value;
        Managers.Sound.SetVolume();
        Exit();
    }


    private void Exit()
    {
        if (_mainMenu != null)
        {
            Debug.Log("MainMenu is not null in UI Setting");
            _mainMenu.RegisterInputKeyAction();
        }
        
        Managers.Input.UIKeyAction -= InputKey;
        Managers.UI.ClosePopupUI(this); 
        if (_pause != null)
        {
            _pause.SetActiveTrue();
        }

    }
    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.Esc:
                Save(new PointerEventData(EventSystem.current));
                break;
        }
    }
    
}
