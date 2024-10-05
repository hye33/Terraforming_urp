using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PauseUI;
    private bool pause = false;
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        PauseUI = GameObject.Find("PauseUI");
        PauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) // esc ������ �� ���� ���� 
        {
            pause = !pause;

            if (pause)
            {
                PauseUI.SetActive(true);
                Time.timeScale = 0f;
            }

            else
            {
                PauseUI.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        
    }

    // ���� ���� ��ư Ŭ�� �� ������ ���� (forest�� �� �̵�) 
    public void GameStart() {
        SceneManager.LoadScene("Forest");
    }

    // ���� ���� ��ư Ŭ�� �� ���� ���� 
    public void GameExit()
    {
        Debug.Log("������ ����Ǿ����ϴ�.");
        Application.Quit();
    }

    public void GameContinue()
    {
        SceneManager.LoadScene("Continue");
    }

    public void PlayContinue() // ���� ���� ȭ�鿡�� ���� �簳
    {
        pause = false;
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
