using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
  
    public class EnemyType
    {
        public GameObject prefab;
        public float spawnChance; // ( 0.6 for 60%) 
    }

    public List<EnemyType> enemyTypes = new List<EnemyType>();
    public Transform spawnPoint;

    public Transform Player;

  
    private const float BASE_SPAWN_RATE_PER_SECOND = 3.0f;
    private float timeBetweenSpawns;
    private float spawnTimer = 0f;

   
    public int currentWave = 1;
    private int enemiesPerWave = 5; 
    [SerializeField]private int enemiesSpawnedInWave = 0;
   [SerializeField] private int currentEnemiesAlive = 0;

   
    private const float WAVE_BREAK_TIME = 10.0f;
    private float waveBreakTimer = WAVE_BREAK_TIME;
    private bool inWaveBreak = true;

    public float MinSpawnDistance = 10f; 
    public float MaxSpawnDistance = 30f;
  

  
    private const int ENEMIES_INCREASE_PER_WAVE = 1; 
    private const float SPAWN_RATE_INCREASE_PER_WAVE = 0.2f;
    public float timeSinceLastSpawn = 0f;
    void Start()
    {
        
        timeBetweenSpawns = 1f / BASE_SPAWN_RATE_PER_SECOND;
       
    }

    void Update()
    {
        if (inWaveBreak)
        {
            HandleWaveBreak();
        }
        else
        {
            HandleWaveSpawning();
            
        }
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= 20f)
        {
            StartNextWave();
            timeSinceLastSpawn = 0f;
        }
    }

    private void HandleWaveBreak()
    {
        waveBreakTimer -= Time.deltaTime;

        if (waveBreakTimer <= 0f)
        {
            StartNextWave();
        }
    }

    private void HandleWaveSpawning()
    {
       
        if (enemiesSpawnedInWave >= enemiesPerWave)
        {
            
            if (currentEnemiesAlive <= 0)
            {
               
                inWaveBreak = true;
                waveBreakTimer = WAVE_BREAK_TIME;
            }
            return;
        }

       
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= timeBetweenSpawns)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyTypes.Count == 0)
        {
            
            return;
        }

        
        GameObject enemyPrefabToSpawn = SelectRandomEnemyPrefab();

        if (enemyPrefabToSpawn != null)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject newEnemy = Instantiate(enemyPrefabToSpawn, spawnPosition, Quaternion.identity);
           
            newEnemy.GetComponent<EnemyHealth>().SetManager(this);

            enemiesSpawnedInWave++;
            currentEnemiesAlive++;
        }
    }

    private GameObject SelectRandomEnemyPrefab()
    {
        float totalChance = 0f;
        foreach (var enemyType in enemyTypes)
        {
            totalChance += enemyType.spawnChance;
        }

        float randomValue = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (var enemyType in enemyTypes)
        {
            cumulativeChance += enemyType.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                return enemyType.prefab;
            }
        }

      
        return enemyTypes[0].prefab;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized; 
        float randomDistance = Random.Range(MinSpawnDistance, MaxSpawnDistance);
        Vector3 spawnOffset = new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;
        return Player.position + spawnOffset;
    }

    private void StartNextWave()
    {
        currentWave++;
       
        enemiesPerWave += ENEMIES_INCREASE_PER_WAVE;
        timeBetweenSpawns = 1f / (BASE_SPAWN_RATE_PER_SECOND + (currentWave - 1) * SPAWN_RATE_INCREASE_PER_WAVE);

        enemiesSpawnedInWave = 0;
        currentEnemiesAlive = 0; 

        inWaveBreak = false;
        
    }
    private void OnDrawGizmos()
    {
        if (Player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Player.position, MinSpawnDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Player.position, MaxSpawnDistance);

        Gizmos.color = Color.yellow; 
    }



   
    public void EnemyDied()
    {
        currentEnemiesAlive--;
    }
}