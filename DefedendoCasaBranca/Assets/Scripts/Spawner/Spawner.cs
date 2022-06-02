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
    private int _enemiesRamaining;
    private Waypoint _waypoint;

    private void Start()
    {
        _waypoint = GetComponent<Waypoint>();

        _enemiesRamaining = enemyCount;
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

        if (currentWave <= 1){
            return enemyWave10Pooler;
        }
        else if (currentWave > 1 && currentWave <=2 ){
            return enemyWave11_20Pooler;
        }
        else if (currentWave > 2 && currentWave <= 3 ){
            return enemyWave21_30Pooler;
        }
        else if (currentWave > 3 && currentWave <= 4 ){
            return enemyWave31_40Pooler;
        }
        else
            return enemyWave41_50Pooler;
        
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
        _enemiesRamaining = enemyCount;
        _spawnTimer = 0f;
        _enemiesSpawned = 0;
    }
    
    private void RecordEnemy(Enemy enemy)
    {
        _enemiesRamaining--;
        if (_enemiesRamaining <= 0)
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
