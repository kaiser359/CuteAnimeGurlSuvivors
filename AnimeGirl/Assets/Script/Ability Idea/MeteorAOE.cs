using System.Collections;
using UnityEngine;

public class MeteorAOE : MonoBehaviour
{
    public float radius = 2.5f;
    public float delayBeforeImpact = 0.6f;
    public LayerMask enemyLayer;
    public GameObject impactVFX;    
    public float destroyAfter = 1f;
    public float fallDuration = 1f; 

    private float damage = 50f;
    private Vector2 target;
    private Vector3 startPosition;

    public GameObject Fire;
    public int fireCount = 10; 

    
    public void Initialize(Vector2 targetPosition, float damageAmount, float r)
    {
        target = targetPosition;
        damage = damageAmount;
        radius = r;
        startPosition = (Vector3)target + Vector3.up * 10f; 
        transform.position = startPosition;

        StartCoroutine(FallAndImpact());
    }

    private IEnumerator FallAndImpact()
    {
        float elapsed = 0f;
       

        
        yield return new WaitForSeconds(delayBeforeImpact);
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fallDuration);
            transform.position = Vector3.Lerp(startPosition, (Vector3)target, t);
            yield return null;
        }
       
        if (impactVFX != null)
            Instantiate(impactVFX, (Vector3)target, Quaternion.identity);



        
        Collider2D[] hits = Physics2D.OverlapCircleAll(target, radius, enemyLayer);
        foreach (var col in hits)
        {
            var enemyHealth = col.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                var dmgable = col.GetComponent<IDamageable>();
                if (dmgable != null) dmgable.TakeDamage(damage);
            }
        }

        
        if (Fire != null)
        {
            for (int i = 0; i < fireCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * radius;
                Vector3 spawnPos = (Vector3)target + new Vector3(offset.x, offset.y, 0f);
                Instantiate(Fire, spawnPos, Quaternion.identity);
            }
        }
        else
        {
          
        }

        
        yield return new WaitForSeconds(destroyAfter);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
