using UnityEngine;

public class SpaceHoles : MonoBehaviour
{

    public Transform Player;
    public float MinSpawnDistance = 10f; // Min Spawn Distance Away From Player
    public float MaxSpawnDistance = 30f; // Max Spawn Distance Away From Player
    public GameObject enemyPrefabToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetRandomSpawnPosition();
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject newEnemy = Instantiate(enemyPrefabToSpawn, spawnPosition, Quaternion.identity);
    }
    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized; // Random direction
        float randomDistance = Random.Range(MinSpawnDistance, MaxSpawnDistance);
        Vector3 spawnOffset = new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;
        return Player.position + spawnOffset; // Final spawn position
    }


    private void OnDrawGizmos()
    {
        if (Player == null) return;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(Player.position, MinSpawnDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Player.position, MaxSpawnDistance);

        Gizmos.color = Color.yellow;
    }


}
