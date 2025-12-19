using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 10f;
    [SerializeField]private float currentHealth;
    private EnemyManager manager; // Reference to the manager
    public PlayerStats stats;
    public GameObject xpdrop;
    public ParticleSystem critdamaged;
    public Animator animator;
    public float timer = 0f;
    InGameScoreManager scoreManager;
    public int scoreValue = 10;
    void Start()
    {
        currentHealth = maxHealth;
    }

    void Awake()
    {
        // stats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
        scoreManager = GameObject.FindWithTag("GameControlle").GetComponent<InGameScoreManager>();
    }


    // Call this immediately after instantiation
    public void SetManager(EnemyManager mgr)
    {
        manager = mgr;
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0f)
        {
            animator.SetBool("IsHurt", false);
          
        }

            if (stats == null)
        {
            stats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
        }
    }
    public void TakeDamage(float damage)
    {
        animator.SetBool("IsHurt", true);
        timer = 0.2f;
        float critChance = stats?.baseCritChance ?? 0;
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
            critdamaged.Play();
            Debug.Log("Critical Hit! Enemy took " + critDamage + " damage.");
        }
        
        if (currentHealth <= 0)
        {
        Die();
        }
    }

    // inside Die() method, replace or update it to call Necromancy.Instance
    private void Die()
    {
        float xpamount = UnityEngine.Random.Range(1f, 5f);
        int xpCount = Mathf.FloorToInt(xpamount);
        scoreManager.AddScore(scoreValue);
        for (int i = 0; i < xpCount; i++)
        {
            Instantiate(xpdrop, transform.position, Quaternion.identity);
        }
        if (manager != null)
        {
            manager.EnemyDied();
        }

        // Spawn an ally on death (if necromancy exists)
        if (Necromancy.Instance != null)
        {
            Necromancy.Instance.CreateAllys(1f); // spawn 1 ally
        }

        Destroy(gameObject);
    }


}
