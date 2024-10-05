using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { if (s_instance == null) Init(); return s_instance; } }

    DataManager _data = new DataManager();
    GameManagerEx _game = new GameManagerEx();
    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }  
    public static GameManagerEx Game { get { return Instance._game; } }
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
            s_instance._sound.Init();
        }
    }

    void Awake()
    {
        Init();
    }

    private void Update()
    {
        _input.OnUpdate();
    }

    private void FixedUpdate()
    {
        _input.OnFixedUpdate();
    }

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        UI.Clear();
    }
}
