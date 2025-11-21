using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 10f;
    [SerializeField]private float currentHealth;
    private EnemyManager manager; // Reference to the manager
    public PlayerStats stats;
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
        float critChance = stats.baseCritChance;
        float randomValue = UnityEngine.Random.Range(0.00f,1f);
        currentHealth -= damage;
        if (randomValue > critChance)
        {
            Debug.Log("Enemy took " + damage + " damage.");
        }
        else
        {
            float critDamage = damage * stats.baseCritDamage;
            currentHealth -= critDamage;
            Debug.Log("Critical Hit! Enemy took " + critDamage + " damage.");
        }        
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
