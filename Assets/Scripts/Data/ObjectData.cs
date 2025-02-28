using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ObjectData
{
    public int ID;
    public string Title;
    public string Type;
    public string Used;
    public string Explain;
    public string Image;
}

[Serializable]
public class ObjectDataLoader
{
    public List<ObjectData> objectDatas = new List<ObjectData>();
    
    public Dictionary<int, ObjectData> MakeDict()
    {
        Dictionary<int, ObjectData> dict = new Dictionary<int, ObjectData> ();
        foreach (ObjectData obj in objectDatas)
            dict.Add(obj.ID, obj);
        return dict;
    }
}