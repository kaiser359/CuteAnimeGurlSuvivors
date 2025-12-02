using UnityEngine;

public class FireScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {

            PlayerHealth inventory = collision.GetComponent<PlayerHealth>();

            if (inventory != null)
            {

                inventory.health = inventory.health + 30;
            }

            // Destroy the item in the scene
            Destroy(gameObject);
        }
    }
}
