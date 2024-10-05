using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[Serializable]
public class SaveFileData_Test
{
    public int number;
    public string stage;
    public string playtime;
    public int record;
    public int gameProgress;
}

[Serializable]
public class SaveFileLoader_test
{
    public List<SaveFileData_Test> SaveFiles = new List<SaveFileData_Test>();
    public Dictionary<int, SaveFileData_Test> MakeDict()
    {
        Dictionary<int, SaveFileData_Test> dict = new Dictionary<int, SaveFileData_Test>();
        foreach (SaveFileData_Test saveFile in SaveFiles)
            dict.Add(saveFile.number, saveFile);
        return dict;
    }
}
