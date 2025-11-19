using UnityEngine;
using System.Collections;

public class EnemyRenderer : MonoBehaviour
{
    public GameObject[] DayEnemies; // Enemies that spawn during the day
    public GameObject[] NightEnemies; // Enemies that spawn at night
    public Transform Player; // Players Position
    public float MinSpawnDistance; // Min Spawn Distance Away From Player
    public float MaxSpawnDistance; // Max Spawn Distance Away From Player
    public float MinSpawnInterval = 10f; // Min Time Between Spawns (in seconds)
    public float MaxSpawnInterval = 20f; // Max Time Between Spawns (in seconds)
    public static bool Night; // Controls whether it's night or day
    public float MaxRenderDistance = 50f; // Max Distance at which enemies should be rendered
    public float MaxTimeBeforeDestroy = 30f; // Time in seconds before destroying an enemy out of range
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            float randomSpawnTime = Random.Range(MinSpawnInterval, MaxSpawnInterval); // Randomize the spawn interval
            yield return new WaitForSeconds(randomSpawnTime);
        }
    }

    private void SpawnEnemy()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player Transform not assigned! : " + "Added One For You No Worry");
            Player = this.transform;
        }

        GameObject randomEnemy = GetRandomEnemy();
        if (randomEnemy == null) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject spawnedEnemy = Instantiate(randomEnemy, spawnPosition, Quaternion.identity);

        // Start tracking the enemy to check if it's too far from the player
        StartCoroutine(CheckEnemyDistance(spawnedEnemy));

        // DEBUG: Log the enemy's stats
     //   Enemy enemyComponent = spawnedEnemy.GetComponent<Enemy>();
     //   if (enemyComponent != null)
        {
       //     Debug.Log($"Spawned {enemyComponent.enemyData.enemyName} with {enemyComponent.enemyData.health} HP");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized; // Random direction
        float randomDistance = Random.Range(MinSpawnDistance, MaxSpawnDistance);
        Vector3 spawnOffset = new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;

        return Player.position + spawnOffset; // Final spawn position
    }

    private GameObject GetRandomEnemy()
    {
        GameObject[] enemyArray = Night ? NightEnemies : DayEnemies;

        if (enemyArray.Length == 0)
        {
            Debug.LogWarning(Night ? "No night enemies assigned!" : "No day enemies assigned!");
            return null;
        }

        int enemyIndex = Random.Range(0, enemyArray.Length);
        return enemyArray[enemyIndex];
    }

    // Check if the enemy is too far from the player for too long
    private IEnumerator CheckEnemyDistance(GameObject enemy)
    {
        float timeOutOfRange = 0f;

        while (enemy != null)
        {
            float distance = Vector3.Distance(enemy.transform.position, Player.position);

            if (distance > MaxRenderDistance)
            {
                // If enemy is too far, start counting the time
                timeOutOfRange += Time.deltaTime;

                if (timeOutOfRange >= MaxTimeBeforeDestroy)
                {
                    // Destroy enemy if it remains out of range for too long
                    Destroy(enemy);
                    Debug.Log($"Enemy {enemy.name} destroyed for being too far from the player.");
                    yield break; // Exit the coroutine
                }
            }
            else
            {
                // Reset time if the enemy is within range
                timeOutOfRange = 0f;
            }

            yield return null; // Wait until next frame to check again
        }
    }

    private void OnDrawGizmos()
    {
        if (Player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Player.position, MinSpawnDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Player.position, MaxSpawnDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Player.position, MaxRenderDistance); // Visualize the max render distance

        // Visualize the max destroy radius (for enemies that are too far)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Player.position, MaxRenderDistance); // Visualize the max destroy distance
    }
}