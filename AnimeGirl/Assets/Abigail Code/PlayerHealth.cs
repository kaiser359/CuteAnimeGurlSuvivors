using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public Image healthBar;
    private PlayerStats statsPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statsPlayer = GetComponent<PlayerStats>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        maxHealth = statsPlayer.baseMaxHealth;
        healthBar.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1);
        DestroyGameObject();

        if(health > maxHealth)
        {
            health -= Time.deltaTime ;
        }
    }

    void DestroyGameObject()
    {
        if(health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
