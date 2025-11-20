using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 10f;
    private float currentHealth;
    private EnemyManager manager; // Reference to the manager

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this immediately after instantiation
    public void SetManager(EnemyManager mgr)
    {
        manager = mgr;
    }

  
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

        LevelUpSystem.Instance.OnPlayerLeveledUp();
        if (manager != null)
        {
            
            manager.EnemyDied();
        }
        Destroy(gameObject);
    }
}
