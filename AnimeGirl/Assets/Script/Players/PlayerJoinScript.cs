using UnityEngine;

public class PlayerJoinScript : MonoBehaviour
{
    public Transform SpawnPoint1, SpawnPoint2;
    public GameObject Player1,player2;

    private void Awake()
    {
        Instantiate(Player1, SpawnPoint1.position, SpawnPoint1.rotation);
        Instantiate(player2, SpawnPoint2.position, SpawnPoint2.rotation);
    }
}
