using UnityEngine;

public class HpPLus: MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
