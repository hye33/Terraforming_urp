using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum UIEvent
    {
        Click,
        Drag,
        Press
    }

    public enum Sound
    {
        Bgm,
        SubBgm,
        Effect,
        LoopEffect,
        MaxCount
    }

    public enum MouseEvent
    {
        LeftClick,
        RightClick
    }


    public enum KeyEvent { A, D, W, Space, Tab, S, SUp, Esc, I, R, Q, E, LeftShift, Count}

    public enum Stage { Forest }
    public enum StageState { Map, Boss }

    public enum LoadMap { ForestMap, ForestBoss }
    // Sprites
    public enum PlayerWeapon { Sword, Gun }
    public enum EnemyState { Idle, Track, Return, Skill, Death }
    public enum ForestEnemyType { Forest01, Forest02, Forest03, Forest04, MaxCount}
}
