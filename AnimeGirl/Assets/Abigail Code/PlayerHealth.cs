using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public Image healthBar;
    private PlayerStats statsPlayer;
 
    public MaxScore maxScore;

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
            if(maxScore.maxScore < maxScore.score)
            {
                maxScore.maxScore = maxScore.score;
            }
            maxScore.score = 0;
            SceneManager.LoadScene("Main MenuTest");
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
