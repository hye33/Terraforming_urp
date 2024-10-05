using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);

        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    protected void BindObject(Type type) { Bind<GameObject>(type); }
    protected void BindText(Type type) { Bind<TMP_Text>(type); }
    protected void BindButton(Type type) { Bind<Button>(type); }
    protected void BindImage(Type type) { Bind<Image>(type); }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TMP_Text GetText(int idx) { return Get<TMP_Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }

    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click, Action<PointerEventData> actionEx = null)
    {
        switch (type)
        {
            case Define.UIEvent.Click:
                UI_ClickEventHandler clickEvt = Util.GetOrAddComponent<UI_ClickEventHandler>(go);
                clickEvt.OnClickHandler -= action;
                clickEvt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                UI_DragEventHandler dragEvt = Util.GetOrAddComponent<UI_DragEventHandler>(go);
                dragEvt.OnDragHandler -= action;
                dragEvt.OnDragHandler += action;
                break;
            case Define.UIEvent.Press:
                UI_PressEventHandler pressEvt = Util.GetOrAddComponent<UI_PressEventHandler>(go);
                pressEvt.OnDownHandler -= action;
                pressEvt.OnUpHandler -= actionEx;
                pressEvt.OnDownHandler += action;
                pressEvt.OnUpHandler += actionEx;
                break;
        }
    }

    protected Color highlightColor = new Color32(249, 222, 110, 255); // 기본색상
    protected Color nomalColor = new Color32(205, 197, 173, 255); // 하이라이트 색상 
    protected void MoveFocus(int dir, ref int currentButtonIndex, GameObject[] buttons, GameObject[] Imgs, TextMeshProUGUI[] TMPs = null, bool changeTextColor = false)
    {
        AudioClip _buttonAudio = Resources.Load<AudioClip>("Sounds/Effect/cursor6");

        if (buttons == null || buttons.Length == 0 || Imgs == null || Imgs.Length == 0) return;

        // 현재 버튼의 이미지와 텍스트 색상 처리 (이전 상태 비활성화)
        if (currentButtonIndex <= 0)
        {
            Imgs[0].SetActive(false);
            if (changeTextColor && TMPs != null) TMPs[0].color = nomalColor;
        }
        else if (currentButtonIndex >= (buttons.Length - 1))
        {
            Imgs[(buttons.Length - 1)].SetActive(false);
            if (changeTextColor && TMPs != null) TMPs[(buttons.Length - 1)].color = nomalColor;
        }
        else
        {
            Imgs[currentButtonIndex].SetActive(false);
            if (changeTextColor && TMPs != null) TMPs[currentButtonIndex].color = nomalColor;
        }

        // 새로운 버튼 포커스 설정
        if (currentButtonIndex < 1) currentButtonIndex += buttons.Length;
        currentButtonIndex = (currentButtonIndex + dir) % buttons.Length;

        EventSystem.current.SetSelectedGameObject(buttons[currentButtonIndex]);

        // 새로운 포커스된 버튼의 이미지와 텍스트 색상 처리 (활성화)
        Imgs[currentButtonIndex].SetActive(true);
        if (changeTextColor && TMPs != null) TMPs[currentButtonIndex].color = highlightColor;

        // 소리 재생 (옵션으로 유지)
        Managers.Sound.Play(_buttonAudio, Define.Sound.Effect);
    }
}
