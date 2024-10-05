using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public Action<Define.KeyEvent> UIKeyAction = null;
    public Action<Define.KeyEvent> KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    // UI Key Press Check
    private bool[] pressKey = new bool[(int)Define.KeyEvent.Count];

    private bool pressLMB = false;
    private bool pressRMB = false;
    private bool pressSpace = false;
    private bool pressTab = false;
    private bool pressEscape = false;
    private bool pressW = false;
    private bool pressR = false;

    public void Clear()
    {
        UIKeyAction = null;
        KeyAction = null;
        MouseAction = null;
    }

    public void OnUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (!pressKey[(int)Define.KeyEvent.W] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.W] = true;
                UIKeyAction.Invoke(Define.KeyEvent.W);
            }
        }
        else pressKey[(int)Define.KeyEvent.W] = false;

        if (Input.GetKey(KeyCode.A))
        {
            if (!pressKey[(int)Define.KeyEvent.A] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.A] = true;
                UIKeyAction.Invoke(Define.KeyEvent.A);
            }
        }
        else pressKey[(int)Define.KeyEvent.A] = false;

        if (Input.GetKey(KeyCode.S))
        {
            if (!pressKey[(int)Define.KeyEvent.S] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.S] = true;
                UIKeyAction.Invoke(Define.KeyEvent.S);
            }
        }
        else pressKey[(int)Define.KeyEvent.S] = false;

        if (Input.GetKey(KeyCode.D))
        {
            if (!pressKey[(int)Define.KeyEvent.D] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.D] = true;
                UIKeyAction.Invoke(Define.KeyEvent.D);
            }
        }
        else pressKey[(int)Define.KeyEvent.D] = false;

        if (Input.GetKey(KeyCode.Space))
        {
            if (!pressKey[(int)Define.KeyEvent.Space] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.Space] = true;
                UIKeyAction.Invoke(Define.KeyEvent.Space);
            }
        }
        else pressKey[(int)Define.KeyEvent.Space] = false;

        if (Input.GetKey(KeyCode.Escape))
        {
            if (!pressKey[(int)Define.KeyEvent.Esc] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.Esc] = true;
                UIKeyAction.Invoke(Define.KeyEvent.Esc);
            }
        }
        else pressKey[(int)Define.KeyEvent.Esc] = false;

        if (Input.GetKey(KeyCode.I))
        {
            if (!pressKey[(int)Define.KeyEvent.I] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.I] = true;
                UIKeyAction.Invoke(Define.KeyEvent.I);
            }
        }
        else pressKey[(int)Define.KeyEvent.I] = false;

        if (Input.GetKey(KeyCode.R))
        {
            if (!pressKey[(int)Define.KeyEvent.R] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.R] = true;
                UIKeyAction.Invoke(Define.KeyEvent.R);
            }
        }
        else pressKey[(int)Define.KeyEvent.R] = false;

        if (Input.GetKey(KeyCode.Q))
        {
            if (!pressKey[(int)Define.KeyEvent.Q] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.Q] = true;
                UIKeyAction.Invoke(Define.KeyEvent.Q);
            }
        }
        else pressKey[(int)Define.KeyEvent.Q] = false;

        if (Input.GetKey(KeyCode.E))
        {
            if (!pressKey[(int)Define.KeyEvent.E] && UIKeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.E] = true;
                UIKeyAction.Invoke(Define.KeyEvent.E);
            }
        }
        else pressKey[(int)Define.KeyEvent.E] = false;
    }

    public void OnFixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (pressLMB == false && MouseAction != null)
            {
                pressLMB = true;
                MouseAction.Invoke(Define.MouseEvent.LeftClick);
            }
        }
        else
            pressLMB = false;
        if (Input.GetMouseButton(1))
        {
            if (pressRMB == false && MouseAction != null)
            {
                pressRMB = true;
                MouseAction.Invoke(Define.MouseEvent.RightClick);
            }
        }
        else
            pressRMB = false;

        if (Input.GetKey(KeyCode.D) && KeyAction != null) { KeyAction.Invoke(Define.KeyEvent.D); }
        if (Input.GetKey(KeyCode.A) && KeyAction != null) { KeyAction.Invoke(Define.KeyEvent.A); }
        if (Input.GetKey(KeyCode.W))
        {
            if (!pressW && KeyAction != null)
            {
                pressW = true;
                KeyAction.Invoke(Define.KeyEvent.W);
            }
        }
        else pressW = false;
        if (Input.GetKey(KeyCode.Space))
        {
            if (pressSpace == false && KeyAction != null)
            {
                pressSpace = true;
                KeyAction.Invoke(Define.KeyEvent.Space);
            }
        }
        else
            pressSpace = false;

        if (Input.GetKey(KeyCode.Tab))
        {
            if (pressTab == false && KeyAction != null)
            {
                pressTab = true;
                KeyAction.Invoke(Define.KeyEvent.Tab);
            }
        }
        else
            pressTab = false;

        if (Input.GetKey(KeyCode.S) && KeyAction != null) { KeyAction.Invoke(Define.KeyEvent.S); }
        else { KeyAction?.Invoke(Define.KeyEvent.SUp); }

        if (Input.GetKey(KeyCode.Escape))
        {
            if (pressEscape == false && KeyAction != null)
            {
                pressEscape = true;
                KeyAction.Invoke(Define.KeyEvent.Esc);
            }
        }
        else
            pressEscape = false;

        if (Input.GetKey(KeyCode.R))
        {
            if (pressR == false && KeyAction != null)
            {
                pressR = true;
                KeyAction.Invoke(Define.KeyEvent.R);
            }
        }
        else
            pressR = false;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!pressKey[(int)Define.KeyEvent.LeftShift] && KeyAction != null)
            {
                pressKey[(int)Define.KeyEvent.LeftShift] = true;
                KeyAction.Invoke(Define.KeyEvent.LeftShift);
            }
        }
        else
            pressKey[(int)Define.KeyEvent.LeftShift] = false;
    }
}