using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sign : UI_Popup
{
    public int ID;
    public bool Check;
    TextMeshProUGUI infoText;
    UI_Record _recordUI;
    enum GameObjects
    {
        Record,
        MonsterGuide,
        ExitR,
        ExitM,
        Title,
        Image,
    }

    public override void Init()
    {
        Managers.Sound.Stop(Define.Sound.LoopEffect);
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;

        BindObject(typeof(GameObjects));
        infoText = GetObject((int)GameObjects.Title).GetComponent<TextMeshProUGUI>();
        var objectData = Managers.Data.ObjectDict[ID];

        string information = "";
        if (ID > 0)
        {
            if (!Managers.Game.SaveData.getRecord.Contains(ID))
            {
                Managers.Game.SaveData.getRecord.Add(ID);
                Debug.Log("Save Data at Sign.cs ");
            }
            information = objectData.Title;
        }

        infoText.text = information;
        //GetObject((int)GameObjects.Image).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/RecordObjects/" + objectData.Image);

        GetObject((int)GameObjects.Record).SetActive(false);
        GetObject((int)GameObjects.MonsterGuide).SetActive(false);
        GetObject((int)GameObjects.ExitM).BindEvent(Exit);
        GetObject((int)GameObjects.ExitR).BindEvent(Exit);

        Image imageComponent = GetObject((int)GameObjects.Image).GetComponent<Image>();
        Sprite sprite = Resources.Load<Sprite>("UI/RecordObjects/" + objectData.Image);

        // 이미지 스프라이트를 설정
        imageComponent.sprite = sprite;

        // 원본 이미지 크기 가져오기
        if (sprite != null)
        {
            float originalWidth = sprite.texture.width;
            float originalHeight = sprite.texture.height;

            // 가로 세로 비율 계산
            float aspectRatio = originalWidth / originalHeight;

            // 가로 또는 세로 중 하나가 200을 넘지 않도록 크기 조정
            float targetWidth = originalWidth;
            float targetHeight = originalHeight;

            if (originalWidth > 250 || originalHeight > 250)
            {
                if (aspectRatio >= 1) // 가로가 더 긴 경우
                {
                    targetWidth = 250;
                    targetHeight = 250 / aspectRatio;
                }
                else // 세로가 더 긴 경우
                {
                    targetHeight = 250;
                    targetWidth = 250 * aspectRatio;
                }
            }

            Debug.Log("" + targetHeight + " " + targetWidth + " " + aspectRatio);

            // RectTransform을 통해 크기 조정
            RectTransform rectTransform = imageComponent.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
        }

    }

    public void SetUI(String type)
    {
        switch (type)
        {
            case "Record":
                GetObject((int)GameObjects.Record).SetActive(true);
                break;

            case "MonsterGuide":
                GetObject((int)GameObjects.MonsterGuide).SetActive(true);
                break;
        }
    }

    private void Exit(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Input.UIKeyAction -= InputKey;
        Debug.Log("set TimeScale 1.0");
        Time.timeScale = 1.0f;
    }
    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.Esc:
                Exit(new PointerEventData(EventSystem.current));
                break;

            case Define.KeyEvent.Tab:
                if(_recordUI == null)
                {
                    Exit(new PointerEventData(EventSystem.current));
                    _recordUI = Managers.UI.ShowPopupUI<UI_Record>("RecordUI");
                }
                break;
        }
    }
}
