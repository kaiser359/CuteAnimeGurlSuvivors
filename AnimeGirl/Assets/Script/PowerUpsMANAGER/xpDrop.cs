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

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            GrindingLevels grindingLevels = collider.gameObject.GetComponent<GrindingLevels>();
            if (grindingLevels != null)
            {
                grindingLevels.AddXP(1); // Add 1 XP to the player
                Destroy(gameObject); // Destroy the XP drop after collection
            }
        }
    }
}
