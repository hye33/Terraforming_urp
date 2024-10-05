using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        T obj = Resources.Load<T>(path);
        if (obj == null)
            Debug.Log($"Fail to load : {path}");
        return obj;
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;

        return go;
    }

    public void Destory(GameObject go)
    {
        if (go == null)
        {
            Debug.Log($"Failed to Destory : {go.name}");
            return;
        }

        Object.Destroy(go);
    }
}