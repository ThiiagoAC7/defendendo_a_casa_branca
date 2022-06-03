using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpawnModes
{
    Fixed,
    Random
}

public class Spawner : MonoBehaviour
{
    public static Action OnWaveCompleted;

    [Header("Settings")]
    [SerializeField] private Transform SpawnPosition;
    [SerializeField] private SpawnModes spawnMode = SpawnModes.Fixed;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private float delayBtwWaves = 1f;

    [Header("Fixed Delay")]
    [SerializeField] private float delayBtwSpawns;
    
    [Header("Random Delay")]
    [SerializeField] private float minRandomDelay;
    [SerializeField] private float maxRandomDelay;

    [Header("Poolers")]
    [SerializeField] private ObjectPooler enemyWave10Pooler;
    [SerializeField] private ObjectPooler enemyWave11_20Pooler;
    [SerializeField] private ObjectPooler enemyWave21_30Pooler;
    [SerializeField] private ObjectPooler enemyWave31_40Pooler;
    [SerializeField] private ObjectPooler enemyWave41_50Pooler;
 

    private float _spawnTimer;
    private int _enemiesSpawned;
    private int _enemiesRemaining;
    private Waypoint _waypoint;

    private void Start()
    {
        _waypoint = GetComponent<Waypoint>();

        _enemiesRemaining = enemyCount;
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer < 0)
        {
            _spawnTimer = GetSpawnDelay();
            if (_enemiesSpawned < enemyCount)
            {
                _enemiesSpawned++;
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject newInstance = GetPooler().GetInstanceFromPool();
        Enemy enemy = newInstance.GetComponent<Enemy>();
        enemy.Waypoint = _waypoint;
        enemy.ResetEnemy();

        enemy.transform.localPosition = SpawnPosition.position;
        newInstance.SetActive(true);
    }

    private ObjectPooler GetPooler() {

        int currentWave = LevelManager.Instance.CurrentWave;

        if (currentWave <= 2){
            return enemyWave10Pooler;
        }
        else if (currentWave%3 == 0 ){
            enemyCount = 10*currentWave;
            return enemyWave11_20Pooler;
        }
        else if (currentWave%4 == 0 ){
            enemyCount = 10*currentWave;
            return enemyWave10Pooler;
        }
        else if (currentWave%5 == 0){
            enemyCount = 10*currentWave;
            return enemyWave21_30Pooler;
        }
        else if (currentWave%6 != 0){
            enemyCount = 10*currentWave;
            return enemyWave31_40Pooler;
        }
        else
        {
            enemyCount = 10*currentWave;
            return enemyWave41_50Pooler;
        }

    }

    private float GetSpawnDelay()
    {
        float delay = 0f;
        if (spawnMode == SpawnModes.Fixed)
        {
            delay = delayBtwSpawns;
        }
        else
        {
            delay = GetRandomDelay();
        }

        return delay;
    }
    
    private float GetRandomDelay()
    {
        float randomTimer = Random.Range(minRandomDelay, maxRandomDelay);
        return randomTimer;
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(delayBtwWaves);
        _enemiesRemaining = enemyCount;
        _spawnTimer = 0f;
        _enemiesSpawned = 0;
    }
    
    private void RecordEnemy(Enemy enemy)
    {
        _enemiesRemaining--;
        if (_enemiesRemaining <= 0)
        {
            OnWaveCompleted?.Invoke();
            StartCoroutine(NextWave());
        }
    }
    
    private void OnEnable()
    {
        Enemy.OnEndReached += RecordEnemy;
        EnemyHealth.OnEnemyKilled += RecordEnemy;
    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= RecordEnemy;
        EnemyHealth.OnEnemyKilled -= RecordEnemy;
    }
}
