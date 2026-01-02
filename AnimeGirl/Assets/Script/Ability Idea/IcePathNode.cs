using UnityEngine;

public class IcePathNode : MonoBehaviour
{

    public float nodeDuration = 10f; // How long the trail stays on the ground

    void Start()
    {
        Destroy(gameObject, nodeDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // You will need to adjust this to match your Player Movement script's variables
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