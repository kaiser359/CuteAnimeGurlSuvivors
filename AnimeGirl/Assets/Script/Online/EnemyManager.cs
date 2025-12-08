using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
  
    public class EnemyType
    {
        public GameObject prefab;
        public float spawnChance; // Chance to spawn this type ( 0.6 for 60%) 
    }

    public List<EnemyType> enemyTypes = new List<EnemyType>();
    public Transform spawnPoint; // Assign a spawn point in the Inspector

    public Transform Player;

    // Spawning parameters
    private const float BASE_SPAWN_RATE_PER_SECOND = 3.0f;
    private float timeBetweenSpawns; // Time needed to pass for one spawn
    private float spawnTimer = 0f;

    // Wave management
    public int currentWave = 1;
    private int enemiesPerWave = 10; // Start with 10 enemies in wave 1
    [SerializeField]private int enemiesSpawnedInWave = 0;
   [SerializeField] private int currentEnemiesAlive = 0;

    // Wave transition
    private const float WAVE_BREAK_TIME = 10.0f;
    private float waveBreakTimer = WAVE_BREAK_TIME;
    private bool inWaveBreak = true;

    public float MinSpawnDistance = 10f; // Min Spawn Distance Away From Player
    public float MaxSpawnDistance = 30f; // Max Spawn Distance Away From Player

    // Difficulty scaling (adjust as needed)
    private const int ENEMIES_INCREASE_PER_WAVE = 10; //const int, and other functions make the game run more smoothly, i recommend. 
    private const float SPAWN_RATE_INCREASE_PER_WAVE = 0.2f;
    void Start()
    {
        // Calculate the initial time required between spawns
        timeBetweenSpawns = 1f / BASE_SPAWN_RATE_PER_SECOND;
        Debug.Log("Game started. Waiting for the first wave to begin.");
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
        // Check if the current wave is complete (all enemies spawned)
        if (enemiesSpawnedInWave >= enemiesPerWave)
        {
            // If all spawned enemies are also dead, end the wave and start the break
            if (currentEnemiesAlive <= 0)
            {
                Debug.Log($"Wave {currentWave} complete! Starting 20 second break.");
                inWaveBreak = true;
                waveBreakTimer = WAVE_BREAK_TIME;
            }
            return;
        }

        // Handle the slow spawning process
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
            Debug.LogError("No enemy types defined in the manager!");
            return;
        }

        // Select an enemy type based on chance
        GameObject enemyPrefabToSpawn = SelectRandomEnemyPrefab();

        if (enemyPrefabToSpawn != null)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject newEnemy = Instantiate(enemyPrefabToSpawn, spawnPosition, Quaternion.identity);
            // We need a reference to the manager within the enemy script. the script for enemy i did is subject to change.
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

        // Should not happen if chances add up correctly
        return enemyTypes[0].prefab; // later i will add a guarantee chance to come a boss fight using similar code here
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized; // Random direction
        float randomDistance = Random.Range(MinSpawnDistance, MaxSpawnDistance);
        Vector3 spawnOffset = new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;
        return Player.position + spawnOffset; // Final spawn position
    }

    private void StartNextWave()
    {
        currentWave++;
        // Increase difficulty:
        enemiesPerWave += ENEMIES_INCREASE_PER_WAVE;
        timeBetweenSpawns = 1f / (BASE_SPAWN_RATE_PER_SECOND + (currentWave - 1) * SPAWN_RATE_INCREASE_PER_WAVE);

        enemiesSpawnedInWave = 0;
        currentEnemiesAlive = 0; // Should be 0 already from the check

        inWaveBreak = false;
        Debug.Log($"Starting Wave {currentWave} with {enemiesPerWave} enemies at a rate of 1 every {timeBetweenSpawns:F2} seconds.");
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



    // Public method called by the Enemy Health script when an enemy dies
    public void EnemyDied()
    {
        currentEnemiesAlive--;
    }
}