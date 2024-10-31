using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private PlayerController player;
    private float TICK = Stat.PLAYER_HEAL_TERM;
    private int HEALING = Stat.PLAYER_HEAL;
    private float HEAL_RANGE = Stat.PLAYER_HEAL_RANGE;

    public int index;

    public bool isHealing = false;

    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = HEAL_RANGE;
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        Vector3 distance = player.transform.position - transform.position;
        if (distance.sqrMagnitude < HEAL_RANGE * HEAL_RANGE)
        {
            if (isHealing)
                return;
            StartCoroutine(nameof(HealPlayer));
        }
        else if (isHealing == true)
        {
            StopCoroutine(nameof(HealPlayer));
            isHealing = false;
        }
    }

    private IEnumerator HealPlayer()
    {
        isHealing = true;
        while(true)
        {
            yield return new WaitForSecondsRealtime(TICK);
            player.Healed(Managers.Game.SaveData.savePointHeal);
        }
    }
}
