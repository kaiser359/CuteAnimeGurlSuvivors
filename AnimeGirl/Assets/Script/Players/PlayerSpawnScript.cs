using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnScript : MonoBehaviour
{
    public Transform[] SpawnPoints;
    private int m_playerCount;

    public void onPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.position = SpawnPoints[m_playerCount].transform.position;
        if (m_playerCount == 0)
        {
            playerInput.GetComponent<PlayerMovement>();
            m_playerCount++;
        }
    }
}
