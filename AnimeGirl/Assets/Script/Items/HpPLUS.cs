using UnityEngine;

public class HpPLus: MonoBehaviour
{
    [SerializeField]private float timetodisappear;
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
