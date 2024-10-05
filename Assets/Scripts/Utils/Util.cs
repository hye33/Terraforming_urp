using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static bool checkProbability(float triggerProbability)
    {
        int randomNumber = Random.Range(0, 100);
        return randomNumber < triggerProbability ? true : false;
    }

    // 중복되지 않는 랜덤한 int 배열을 반환
    // min~max 사이의 수를 number개 반환한다
    public static int[] GetRandomIntArray(int min, int max, int number)
    {
        int[] array = new int[number];
        List<int> list = new List<int>();

        for (int i = min; i < max + 1; i++)
        {
            list.Add(i);
        }

        for (int i = 0; i < number; i++)
        {
            int idx = Random.Range(min, max + 1 - i);
            array[i] = list[idx];
            list.RemoveAt(idx);
        }

        return array;
    }

    // 모든 자식 요소들(자기 자신 제외)의 위치 정보를 반환
    public static Vector3[] GetAllChildPositions(GameObject obj)
    {
        Transform[] t = obj.transform.GetComponentsInChildren<Transform>();
        Vector3[] result = new Vector3[t.Length - 1];
        for (int i = 0; i < t.Length - 1; i++) // 자기 자신의 좌표는 제외하고 저장
        {
            result[i] = t[i + 1].position;
        }

        return result;
    }
}
