using UnityEngine;

public class CombinedEnemy : MonoBehaviour
{
    public EnemyAttack melee;
    public EnemyRangedAttack ranaged;
    public float DetectionRadius = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        ChangeAttackMethod();
    }
    void ChangeAttackMethod()
    {
        if(Vector2.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) < DetectionRadius)
        {
            ranaged.enabled = false;
            melee.enabled = true;

        }
        else 
        {
            ranaged.enabled = true;
            melee.enabled = false;
        }
    }
}
