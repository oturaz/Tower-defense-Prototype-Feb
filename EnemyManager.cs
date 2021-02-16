using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyData> enemyWaves = new List<EnemyData>();
    public int nrOfWaves = 3;
    public float spawnCooldown = 1f;
    public float spawnCooldownLeft = 0f;
    public int enemyIndex;
    public int currentWaveIndex;

    public List<GameObject> enemyPrefabs = new List<GameObject>();

    public Vector3 spawnPoint;
    Quaternion spawnRotation;
    private GameObject enemiesParent;

    public GameObject spawner;

    void Awake()
    {
        spawnCooldown = 1f;
        enemyIndex = 0;
        currentWaveIndex = 0;

        enemyPrefabs.Add((GameObject)Resources.Load("Prefabs/Enemies/SmallEnemy"));

        if(GameObject.Find("enemiesParent") != null)
        {
            enemiesParent = GameObject.Find("enemiesParent");
        }
        else
        {
            enemiesParent = new GameObject("enemiesParent");
        }
        PopulateEnemyWaves();
    }

    private void Start()
    {
        spawner = GameObject.Find("Paths");
        //TO DO: calculate the start point and rotation in start
    }

    void Update()
    {
        Debug.Log(enemyWaves[0].enemyType);
        spawnCooldownLeft -= Time.deltaTime;
        if(spawnCooldownLeft <= 0)
        {
            spawnCooldownLeft = spawnCooldown;

            SpawnEnemy();

            if(IsAllWaveSpawned())
            {
                StartNextWave();
            }
        }
    }

    public void AddEnemyToWaves(int _waveIndex = 0, int _enemyType = 0, int _pathIndex = 0)
    {
        enemyWaves.Add(new EnemyData(_waveIndex, _enemyType, _pathIndex));
    }

    public void SpawnEnemy()
    {
        if(enemyIndex < enemyWaves.Count)
        {
            while (enemyWaves[enemyIndex].waveIndex != currentWaveIndex && enemyIndex < enemyWaves.Count - 1)
            {
                enemyIndex++;
            }
            if (enemyWaves[enemyIndex].waveIndex == currentWaveIndex)
            {//TO DO: move this back in start
                spawnPoint = spawner.transform.GetChild(enemyWaves[enemyIndex].pathIndex).GetChild(0).transform.position;
                Vector3 direction = (spawner.transform.GetChild(enemyWaves[enemyIndex].pathIndex).GetChild(1).transform.position - spawnPoint).normalized;
                spawnRotation = Quaternion.LookRotation(direction);

                GameObject enemy = Instantiate(enemyPrefabs[enemyWaves[enemyIndex].enemyType], spawnPoint, spawnRotation, enemiesParent.transform);
                enemy.GetComponent<Enemy>().pathGO = spawner.transform.GetChild(enemyWaves[enemyIndex].pathIndex).gameObject;

                enemyWaves[enemyIndex].quantity--;
                if(enemyWaves[enemyIndex].quantity > 0)
                {
                    enemyIndex--;
                }
            }
            enemyIndex++;

        }
    }

    public void StartNextWave()
    {
        if(currentWaveIndex < nrOfWaves - 1)
        { 
            enemyIndex = 0;
            currentWaveIndex++;
        }
    }

    public bool IsAllWaveSpawned()
    {
        if (enemyIndex == enemyWaves.Count)
        {
            return true;
        }
        return false;
    }

    public bool IsAllLevelSpawned()
    {
        if(currentWaveIndex == nrOfWaves - 1 && enemyIndex == enemyWaves.Count)
        {
            return true;
        }
        return false;
    }

    public void PopulateEnemyWaves()//TODO make level editor, this is just temporary
    {
        for (int i = 0; i < nrOfWaves; i++)
        {
            AddEnemyToWaves(i, 0);
            AddEnemyToWaves(i, 0);
            AddEnemyToWaves(i, 0);
            AddEnemyToWaves(i, 0);
        }
    }
}