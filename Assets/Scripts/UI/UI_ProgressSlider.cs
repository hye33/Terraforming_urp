using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProgressSlider : MonoBehaviour
{
    public float progress;
    public Image _bar;
    public RectTransform button;

    private void Update()
    {
        Change(progress);
    }
    void Change(float progress)
    {
        float amount = (progress / 100.0f);
        float fillAmount = Mathf.Lerp(0.1f, 0.9f, amount);
        _bar.fillAmount = fillAmount;
        //float buttonAngle = amount * 360; // 각도 범위 : -55 ~ -355
        float buttonAngle = Mathf.Lerp(-305f, -55f, amount);
        button.localEulerAngles = new Vector3(0, 0, -buttonAngle);
    }
   
}
