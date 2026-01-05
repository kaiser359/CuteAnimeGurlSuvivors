using UnityEngine;

public class IcePathNode : MonoBehaviour
{

    public float nodeDuration = 10f; 

    void Start()
    {
        Destroy(gameObject, nodeDuration);// node timing 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           
            var moveScript = other.GetComponent<PlayerMovement>();
            if (moveScript != null) moveScript._moveSpeed = 15f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var moveScript = other.GetComponent<PlayerMovement>();
            if (moveScript != null) moveScript._moveSpeed = 5f;
        }
    }
}