using UnityEngine;

public class PaperFigurine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Try get the player's inventory script
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.AddPaperFigurine(1);
            }

            // Destroy the item in the scene
            Destroy(gameObject);
        }
    }
}
