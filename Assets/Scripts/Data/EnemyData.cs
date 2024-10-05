using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStat
{
    public int ID;
    public int hp;
    public int damage;
    public int touchDamage;
    public float moveSpeed;
    public int skillGauge;
    public int objectDropPer;
    public float trackRange;
    public float attackRange;
}

[Serializable]
public class EnemyStatLoader
{
    public List<EnemyStat> enemyStats = new List<EnemyStat>();

    public Dictionary<int, EnemyStat> MakeDict()
    {
        Dictionary<int, EnemyStat> dict = new Dictionary<int, EnemyStat>();
        foreach (EnemyStat enemyStat in enemyStats)
            dict.Add(enemyStat.ID, enemyStat);
        return dict;
    }
}