using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    // Player
    public const float PLAYER_MOVE_SPEED = 7.5f; // 이동 속도
    public const float PLAYER_JUMP_POWER = 14.0f; // 점프력
    public const float PLAYER_TAKEDOWN_POWER = 20.0f; // 내려찍기 공격 속도
    public const float PLAYER_BULLET_SPEED = 20.0f; // 총알 속도

    public const float PLAYER_SWORD_RANGE = 6.0f; // 근거리 공격 범위

    // Player Range (지름 길이)
    public const float PLAYER_EXPLODE_RANGE = 5.0f; // 폭발 공격 범위

    // Player Term
    public const float PLAYER_SHOOT_TERM = 0.2f; // 총알 발사 텀

    // Save Point (ex. 1.0초마다 hp 5씩 회복)
    public const float PLAYER_HEAL_TERM = 0.5f; // 세이브 포인트 근처 회복 텀
    public const int PLAYER_HEAL = 5; // 텀마다의 회복량
    public const float PLAYER_HEAL_RANGE = 3.0f; // 세이브 포인트 근처 힐 범위 ("반지름" 길이입니다)

    // Enemy Term
    public const float ENEMY_SPAWN_TERM = 30.0f; // 몬스터 사망 후 스폰 텀
}
