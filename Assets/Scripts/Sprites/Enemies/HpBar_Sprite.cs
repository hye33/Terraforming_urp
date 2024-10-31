using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar_Sprite : MonoBehaviour
{
    Transform slider;
    float xScale;

    private void Awake()
    {
        slider = transform.GetChild(0);
        xScale = slider.transform.localScale.x;
    }

    public void updateHpBar(float hp, float maxHp)
    {
        float fillArea = (hp / maxHp) * xScale;
        slider.transform.localScale = new Vector3(fillArea, slider.transform.localScale.y, slider.transform.localScale.z);
    }
}
