using Unity.VisualScripting;
using UnityEngine;

public class IcePathNode : MonoBehaviour
{

    public float nodeDuration = 10f; 

    void Start()
    {
        Destroy(gameObject, nodeDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           
            var moveScript = other.GetComponent<PlayerMovement>();
            if (moveScript != null) moveScript._moveSpeed = 15f;
        }
        if (other.CompareTag("Enemy"))
        {
            var enemyScript = other.GetComponent<EnemyHealth>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var moveScript = other.GetComponent<PlayerMovement>();
            if (moveScript != null) moveScript._moveSpeed = moveScript._movementSpeed;
        }
        if(other.CompareTag("Enemy"))
        {
            var enemyScript = other.GetComponent<EnemyHealth>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(5f);
            }
        }
    }
    
}