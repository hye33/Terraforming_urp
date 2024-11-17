using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PuzzleLock : MonoBehaviour
{
    float duration = 0.7f;
    private bool puzzleChecking; // GameManager 에서 값을 가져옴
    private bool miniBossChecking;
    private bool firstEnter = true; // 플레이어가 처음 퍼즐 방에 들어왔는지 확인 (갇혔을 때 메시지 출력용)
    [SerializeField]
    private GameObject upTarget;
    [SerializeField]
    private GameObject downTarget;

    private Sign sign;

    GameObject upObject;
    GameObject downObject;

    private GameObject player;

    private Vector3[] startPoint = new Vector3[2]; // 오브젝트 출발 위치를 담는다
    private Vector3[] endPoint = new Vector3[2];
    private void Start()
    {
        puzzleChecking = Managers.Game.SaveData.puzzleSolved;
        miniBossChecking = Managers.Game.SaveData.miniBossDie;
        upObject = transform.Find("Up").gameObject;
        downObject = transform.Find("Down").gameObject;

        startPoint[0] = upObject.transform.position;
        startPoint[1] = downObject.transform.position;

        endPoint[0] = upTarget.transform.position;
        endPoint[1] = downTarget.transform.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            if (this.gameObject.name == "PuzzleLock")
            {
                HandleLock(puzzleChecking);
            }
            else if (this.gameObject.name == "MiniBossLock")
            {
                HandleLock(miniBossChecking);
            }
            // 세이브 포인트 생성 불가 업데이트
            player.GetComponent<PlayerController>().SetCanMakeSavePoint(false);
        }
    }
    private void HandleLock(bool isSolved)
    {
        if (isSolved) return;
        if (firstEnter)
        {
            firstEnter = false;
            StartCoroutine(MoveObject(startPoint, endPoint, gameObject.name));

            if (this.gameObject.name == "MiniBossLock")
            {
                player.GetComponent<PlayerController>().AutoSaveGame(2);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (this.gameObject.name == "PuzzleLock")
            {
                ShowMessage(puzzleChecking, "퍼즐을 풀어야 문이 열릴 것 같다.");
            }
            else if (this.gameObject.name == "MiniBossLock")
            {
                ShowMessage(miniBossChecking, "보스를 해치워야 문이 열릴 것 같다.");
                Camera.main.GetComponent<CameraController>().ChangeCameraState((int)CameraController.ForestMapCameraState.MiddleBoss); // 카메라 state 변경
            }
        }
    }

    private void ShowMessage(bool isSolved, string message)
    {
        Debug.Log(isSolved + message);
        if (isSolved) return;
        if (!firstEnter)
        {
            if (sign == null)
            {
                sign = Managers.UI.ShowPopupUI<Sign>("Sign");
                sign.ID = 0;
                sign.Check = false;
                sign.Init();
                sign.ShowMessage(message);
            }
        }
    }

    IEnumerator MoveObject(Vector3[] startPoints, Vector3[] endPoints, string targetObject)
    {
        Debug.Log(this.gameObject.name + "MoveObject");
        if(this.gameObject.name == targetObject)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                upObject.transform.position = Vector3.Lerp(startPoints[0], endPoints[0], elapsedTime / duration);
                downObject.transform.position = Vector3.Lerp(startPoints[1], endPoints[1], elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            upObject.transform.position = endPoints[0];
            downObject.transform.position = endPoints[1];
        }
    }
    public void OpenDoor(string objectName)
    {// 보스 죽었을 때 openDoor 함수 호출해야함 
        Debug.Log(objectName);

        // 문열림 메시지 출력
        sign = Managers.UI.ShowPopupUI<Sign>("Sign");
        sign.ID = 0;
        sign.Check = false;
        sign.Init();
        sign.ShowMessage("문이 열리는 소리가 들린다.");
        Debug.Log(objectName + " Door Opening");

        // 문 열린 후 trigger 제거
        GetComponent<BoxCollider2D>().gameObject.SetActive(false);

        // 세이브 포인트 생성 가능하게 업데이트
        player.GetComponent<PlayerController>().SetCanMakeSavePoint(true);

        // 변수 업데이트 및 게임 저장 데이터 업데이트
        if (objectName == "PuzzleLock")
        {
            puzzleChecking = true;
            Managers.Game.SaveData.puzzleSolved = true;

            player.GetComponent<PlayerController>().AutoSaveGame(1);
        }
        else if (objectName == "MidBossLock")
        {
            miniBossChecking = true;
            Managers.Game.SaveData.miniBossDie = true;

            player.GetComponent<PlayerController>().AutoSaveGame(3);
        }

        firstEnter = true;
        StartCoroutine(MoveObject(endPoint, startPoint, objectName));
    }
    
}
