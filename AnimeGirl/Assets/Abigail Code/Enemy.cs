using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float maxHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        DestroyEnemy();
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    void DestroyEnemy()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
