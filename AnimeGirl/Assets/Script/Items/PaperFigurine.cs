using UnityEngine;

public class PaperFigurine : MonoBehaviour
{
    [SerializeField] private float timetodisappear;
    public void Update()
    {
        timetodisappear += Time.deltaTime * 1.15f;
        if (timetodisappear >= 10f)
        {
            Destroy(this.gameObject);
        }
    }
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
