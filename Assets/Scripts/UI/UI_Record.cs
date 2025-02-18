using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Record : UI_Popup
{
    List<int> getRecords;
    GameObject prevSelected;
    enum GameObjects
    {
        SelectedTMP,
        InformationTMP,
        SelectedImg,
        Content,
        Exit,
        Map,
        MonsterGuide
    }
    private void Awake()
    {
        Init();
    }
    public override void Init()
    {
        Managers.Sound.Stop(Define.Sound.LoopEffect);
        Time.timeScale = 0;
        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;
        BindObject(typeof(GameObjects));
        getRecords = Managers.Game.SaveData.getRecord;

        int i = 0;
        foreach ( int record in getRecords)
        {
            int ID = record;
            GameObject current = GetObject((int)GameObjects.Content).transform.GetChild(i).gameObject;

            current.SetActive(true);
            var objectData = Managers.Data.ObjectDict[ID];

            string path = "UI/RecordObjects/" + objectData.Image;
            //current.transform.Find("Img").GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
            // Sprite 로드 및 이미지 설정
            Image imgComponent = current.transform.Find("Img").GetComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>(path);
            imgComponent.sprite = sprite;

            // RectTransform 크기 조정
            if (sprite != null)
            {
                float originalWidth = sprite.texture.width;
                float originalHeight = sprite.texture.height;

                // 비율 계산
                float aspectRatio = originalWidth / originalHeight;

                // 목표 크기 설정
                float targetWidth = originalWidth;
                float targetHeight = originalHeight;

                if (originalWidth > 90 || originalHeight > 90)
                {
                    if (originalWidth > originalHeight) // 가로가 더 긴 경우
                    {
                        targetWidth = 90; // 가로를 90으로 제한
                        targetHeight = 90 / aspectRatio; // 세로는 비율에 맞춰 조정
                    }
                    else // 세로가 더 긴 경우
                    {
                        targetHeight = 90; // 세로를 90으로 제한
                        targetWidth = 90 * aspectRatio; // 가로는 비율에 맞춰 조정
                    }
                }

                // RectTransform 크기 조정
                RectTransform rectTransform = imgComponent.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
            }
            current.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = objectData.Title;
            current.transform.Find("SelectedFrame").gameObject.SetActive(false);
            current.transform.Find("UnSelectedFrame").gameObject.SetActive(true);

            current.BindEvent((PointerEventData data) => ObjectSelect(data, objectData, current));
            Debug.Log(current.gameObject.name);
            i++;
        }
        GetObject((int)GameObjects.Exit).BindEvent(Close);
    }

    private void ObjectSelect(PointerEventData data, ObjectData objectData, GameObject current)
    {
        Debug.Log(objectData.Title);
        GetObject((int)GameObjects.SelectedTMP).GetComponent<TextMeshProUGUI>().text = objectData.Title;
        GetObject((int)GameObjects.InformationTMP).GetComponent<TextMeshProUGUI>().text = objectData.Explain;
        //GetObject((int)GameObjects.SelectedImg).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/RecordObjects/" + objectData.Image);
        Image imgComponent = GetObject((int)GameObjects.SelectedImg).GetComponent<Image>();
        Sprite sprite = Resources.Load<Sprite>("UI/RecordObjects/" + objectData.Image);
        imgComponent.sprite = sprite;

        Debug.Log(objectData.Image);
        // RectTransform 크기 조정
        if (sprite != null)
        {
            float originalWidth = sprite.texture.width;
            float originalHeight = sprite.texture.height;

            // 비율 계산
            float aspectRatio = originalWidth / originalHeight;

            // 목표 크기 초기화
            float targetWidth = originalWidth;
            float targetHeight = originalHeight;

            // 가로 또는 세로가 440을 넘는 경우 크기 조정
            if (originalWidth > 440 || originalHeight > 440)
            {
                if (originalWidth > originalHeight) // 가로가 더 긴 경우
                {
                    targetWidth = 440; // 가로를 440으로 제한
                    targetHeight = 440 / aspectRatio; // 세로는 비율에 맞춰 조정
                }
                else // 세로가 더 긴 경우
                {
                    targetHeight = 440; // 세로를 440으로 제한
                    targetWidth = 440 * aspectRatio; // 가로는 비율에 맞춰 조정
                }
            }

            // RectTransform 크기 조정
            RectTransform rectTransform = imgComponent.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
        }

        current.transform.Find("SelectedFrame").gameObject.SetActive(true);
        current.transform.Find("UnSelectedFrame").gameObject.SetActive(false);

        if(prevSelected != null && prevSelected != current)
        {
            prevSelected.transform.Find("SelectedFrame").gameObject.SetActive(false);
            prevSelected.transform.Find("UnSelectedFrame").gameObject.SetActive(true);
        }

        prevSelected = current;
    }

    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.Q:
                break;
            case Define.KeyEvent.E:
                break;
            case Define.KeyEvent.Esc:
                Close(null);
                break;
        }

    }
    private void Close(PointerEventData data)
    {
        Debug.Log("Close Called in Ui_Record");
        Managers.UI.CloseAllPopupUI();
        Managers.Input.UIKeyAction -= InputKey;
        Time.timeScale = 1;
    }

}
