using UnityEngine;

public class xpDrop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GrindingLevels grindingLevels = collision.gameObject.GetComponent<GrindingLevels>();
            if (grindingLevels != null)
            {
                grindingLevels.AddXP(1); // Add 1 XP to the player
                Destroy(gameObject); // Destroy the XP drop after collection
            }
        }
    }
}
