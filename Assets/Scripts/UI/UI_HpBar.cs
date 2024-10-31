using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : MonoBehaviour
{
    Slider slider;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void updateHpBar(float hp, float maxHp)
    {
        slider.value = hp / maxHp;
    }
}
