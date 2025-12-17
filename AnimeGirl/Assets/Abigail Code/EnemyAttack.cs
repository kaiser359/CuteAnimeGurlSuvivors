using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public PlayerHealth pHealth;
    public float damage;
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pHealth == null)
        {
            pHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("IsAttacking", true);
            other.gameObject.GetComponent<PlayerHealth>().health -= damage;
        }
    }
}
