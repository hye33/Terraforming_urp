using Cinemachine;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Puzzle : UI_Popup
{
    public float rotateSpeed = 1f;
    private Vector2 dragStartPoint;
    private Dictionary<string, float> angleOffsets = new Dictionary<string, float>();
    private UI_Character _characterUI;
    private PuzzleLock puzzleLock;
    private MapScene scene;

    enum GameObjects
    {
        ImageBase,
        Image1,
        Image2,
        Image3,
        Image4,
    }

    public override void Init()
    {
        Managers.Sound.Stop(Define.Sound.LoopEffect); // 발소리 재생 중지 
        Time.timeScale = 0;
        puzzleLock = GameObject.Find("PuzzleLock").GetComponent<PuzzleLock>();
        scene = FindObjectOfType<MapScene>();
        scene.PuzzleUI = this;
        _characterUI = FindObjectOfType<UI_Character>();
        _characterUI.gameObject.SetActive(false);

        BindObject(typeof(GameObjects));
        string spritePath = "UI/Puzzle/";
        Debug.Log(spritePath);

        string[] imageFiles = { "M1_", "M2_", "M3_", "M4_", "M5_" };

        for (int i = 0; i < imageFiles.Length; i++)
        {
            Sprite imageSprite = Resources.Load<Sprite>(spritePath + imageFiles[i]);
            GetObject(i).GetComponent<Image>().sprite = imageSprite;
        }

        for (int i = 1; i < imageFiles.Length - 1; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            GameObject currentObject;
            currentObject = GetObject(i);
            currentObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            currentObject.transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
            currentObject.BindEvent((data) => RotationImage(data, currentObject), Define.UIEvent.Drag);
            currentObject.BindEvent((data) => OnClick(data, currentObject), Define.UIEvent.Press);
            currentObject.BindEvent(ClearCheck, Define.UIEvent.Click);
        }

        Managers.Input.UIKeyAction -= InputKey;
        Managers.Input.UIKeyAction += InputKey;
    }
    private void Start()
    {
        Init();
    }

    private void RotationImage(PointerEventData data, GameObject imageToRotate)
    {
        Vector2 currentDragPoint = data.position;
        Vector2 startVector = (Vector3)dragStartPoint - imageToRotate.transform.position;
        Vector2 currentVector = (Vector3)currentDragPoint - imageToRotate.transform.position;

        /*
        CircleCollider2D collider = imageToRotate.GetComponentInChildren<CircleCollider2D>();

        if(collider == null)
        {
            Debug.Log("collider null. ");
            return;
        }
        */

        float angle = Vector2.SignedAngle(startVector, currentVector);
        float newAngle = angle + angleOffsets[imageToRotate.name];
        imageToRotate.transform.rotation = Quaternion.Euler(0, 0, newAngle);

        if (imageToRotate.name == GetObject((int)GameObjects.Image1).name)
        {
            GameObject image3 = GetObject((int)GameObjects.Image3);
            float newAngle3 = angle + angleOffsets[image3.name];
            image3.transform.rotation = Quaternion.Euler(0, 0, newAngle3);
        }
        else if (imageToRotate.name == GetObject((int)GameObjects.Image3).name)
        {
            GameObject image2 = GetObject((int)GameObjects.Image2);
            float newAngle3 = angle + angleOffsets[image2.name];
            image2.transform.rotation = Quaternion.Euler(0, 0, newAngle3);
        }
    }

    private void OnClick(PointerEventData data, GameObject imageToRotate)
    {
        dragStartPoint = data.position;
        //angleOffset = imageToRotate.transform.rotation.eulerAngles.z;

        for (int i = 1; i < 4; i++)
        {
            GameObject image = GetObject(i);
            angleOffsets[image.name] = image.transform.rotation.eulerAngles.z;
        }
    }

    private void ClearCheck(PointerEventData data)
    {
        float angle1 = GetObject((int)GameObjects.Image1).transform.rotation.eulerAngles.z;
        float angle2 = GetObject((int)GameObjects.Image2).transform.rotation.eulerAngles.z;
        float angle3 = GetObject((int)GameObjects.Image3).transform.rotation.eulerAngles.z;

        angle1 = (angle1 > 180) ? angle1 - 360 : angle1;
        angle2 = (angle2 > 180) ? angle2 - 360 : angle2;
        angle3 = (angle3 > 180) ? angle3 - 360 : angle3;

        if (angle1 >= -10 && angle1 <= 10 && angle2 >= -10 && angle2 <= 10 && angle3 >= -10 && angle3 <= 10)
        {
            Debug.Log(puzzleLock.name);
            Time.timeScale = 1;
            Managers.UI.ClosePopupUI(this);

            scene.UpdateTerraformingGauge(10,"Cave"); //퍼즐 완료시 테라포밍 게이지 10 증가
            puzzleLock.OpenDoor("PuzzleLock");
        }

    }
    private void InputKey(Define.KeyEvent key)
    {
        switch (key)
        {
            case Define.KeyEvent.Esc:
                Managers.UI.ClosePopupUI(this);
                break;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
        _characterUI.gameObject.SetActive(true);
    }


}
