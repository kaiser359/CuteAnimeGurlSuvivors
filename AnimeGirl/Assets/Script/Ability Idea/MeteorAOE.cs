using System.Collections;
using UnityEngine;

public class MeteorAOE : MonoBehaviour
{
    public float radius = 2.5f;
    public float delayBeforeImpact = 0.6f;
    public LayerMask enemyLayer;
    public GameObject impactVFX;    // optional VFX prefab (spawned at impact)
    public float destroyAfter = 1f; // time after impact to destroy object
    public float fallDuration = 1f; // time to reach the target

    private float damage = 50f;
    private Vector2 target;
    private Vector3 startPosition;

    public GameObject Fire;
    public int fireCount = 10; // number of fires to spawn in the AOE

    
    public void Initialize(Vector2 targetPosition, float damageAmount, float r)
    {
        target = targetPosition;
        damage = damageAmount;
        radius = r;
        startPosition = (Vector3)target + Vector3.up * 10f; // start high above
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
        // spawn impact VFX might delete later
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

        // spawn multiple fires distributed inside the AOE, For loop 
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
            Debug.LogWarning("MeteorAOE: Fire prefab is not assigned.");
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
