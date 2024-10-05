using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public PlayerData Player { get; private set; }
    public Dictionary<int, EnemyStat> EnemyDict { get; private set; }
    public Dictionary<int, ObjectData> ObjectDict { get; private set; }
    public Dictionary<int, SaveFileData_Test> SaveDict { get; private set; }

    public void Init()
    {
        Player = LoadSingleJson<PlayerData>("PlayerStatData");
        EnemyDict = LoadJson<EnemyStatLoader, int, EnemyStat>("EnemyStatData").MakeDict();
        ObjectDict = LoadJson<ObjectDataLoader, int, ObjectData>("InteractiveObjectData").MakeDict();
        SaveDict = LoadJson<SaveFileLoader_test, int, SaveFileData_Test>("SaveData_Test").MakeDict();
    }

    Item LoadSingleJson<Item>(string path)
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Item>(textAsset.text);
    }

    Loader LoadJson<Loader, Key, Value>(string path)
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}