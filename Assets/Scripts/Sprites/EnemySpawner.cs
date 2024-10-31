using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Define.ForestEnemyType _spawnEnemy;
    private float SPAWN_TERM = Stat.ENEMY_SPAWN_TERM;
    private GameObject[] _enemyPrefabs;

    private MapScene forestScene;
    private GameObject currentEnemy;
    private Vector3 spawnPos;

    public bool isActive = true;

    private void Init()
    {
        forestScene = FindObjectOfType<MapScene>();
        _enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Sprites/Enemy");
        spawnPos = transform.position;
    }

    void Start()
    {
        Init();
        SpawnEnemy();
    }

    public void DeadEnemy()
    {
        Destroy(currentEnemy);
    }

    private void SpawnEnemy()
    {
        if (currentEnemy == null)
        {
            currentEnemy = Instantiate(_enemyPrefabs[(int)_spawnEnemy], transform);
            currentEnemy.GetComponent<ForestEnemy>().isDead = EnemyDead;
            currentEnemy.GetComponent<ForestEnemy>().Init();
        }
        currentEnemy.transform.position = spawnPos;
        currentEnemy.SetActive(true);
        currentEnemy.GetComponent<ForestEnemy>().ResetState();
    }

    private void EnemyDead()
    {
        Invoke(nameof(SpawnEnemy), SPAWN_TERM);
        StartCoroutine(coAddKillCount());
    }

    private IEnumerator coAddKillCount()
    {
        yield return new WaitForSeconds(2.5f);
        forestScene.AddKillCount(_spawnEnemy);
    }
}
