using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ally : MonoBehaviour
{

    public float moveSpeed = 3f;

    public float damage = 5f;

    public float attackInterval = 0.5f;

    public float attackRange = 0.8f;

    public float detectionRadius = 10f;

    public float maxDistanceFromOwner = 6f;

    public float lifeTime = 30f;

    [HideInInspector]
    public Necromancy OwnerNecromancy;

    // Separation / avoidance settings

    public float separationRadius = 0.8f;

    public float separationStrength = 2.5f;

    // Idle spread so allies don't all sit on the exact same point near owner

    public float idleMinRadius = 0.4f;

    public float idleMaxRadius = 1.2f;

    private Rigidbody2D _rb;
    private Transform _target;
    private float _attackTimer;
    private Vector2 _cachedOwnerPos;
    private Vector2 _idleOffset; // per-ally offset so they spread around owner

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _attackTimer = attackInterval;

        // pick a persistent random idle offset direction & radius so allies spread
        _idleOffset = Random.insideUnitCircle.normalized * Random.Range(idleMinRadius, idleMaxRadius);

        StartCoroutine(TargetAndMoveRoutine());
        StartCoroutine(LifeTimer());
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private IEnumerator TargetAndMoveRoutine()
    {
        // Frequent retargeting keeps behavior responsive
        while (true)
        {
            FindNearestTarget();
            yield return new WaitForSeconds(0.25f);
        }
    }

    void FixedUpdate()
    {
        // Keep owner position cached each frame
        if (OwnerNecromancy != null)
            _cachedOwnerPos = OwnerNecromancy.transform.position;
        else
            _cachedOwnerPos = transform.position;

       
        if (_target == null)
        {
            ReturnTowardOwner();
            return;
        }

        
        if (_target == null || !_target.gameObject.activeInHierarchy)
        {
            _target = null;
            FindNearestTarget();
            return;
        }

        Vector2 dir = ((Vector2)_target.position - _rb.position);
        float dist = dir.magnitude;

        
        if (OwnerNecromancy != null)
        {
            float distTargetToOwner = Vector2.Distance(_cachedOwnerPos, _target.position);
            if (distTargetToOwner > maxDistanceFromOwner)
            {
              
                ReturnTowardOwner();
                _attackTimer = attackInterval; 
                return;
            }
        }

        
        Vector2 separation = ComputeSeparation();

       
        Vector2 moveDir = dir.normalized;
        Vector2 combined = (moveDir + separation).normalized;

        
        if (dist > 0.01f)
        {
            Vector2 vel = combined * moveSpeed;
            _rb.MovePosition(_rb.position + vel * Time.fixedDeltaTime);
        }

        // Attack when in range
        if (dist <= attackRange)
        {
            _attackTimer -= Time.fixedDeltaTime;
            if (_attackTimer <= 0f)
            {
                ApplyDamageToTarget();
                _attackTimer = attackInterval;
            }
        }
        else
        {

            _attackTimer = Mathf.Min(_attackTimer, attackInterval);
        }
    }

    private Vector2 ComputeSeparation()
    {
        Vector2 result = Vector2.zero;
        Collider2D[] hits = Physics2D.OverlapCircleAll(_rb.position, separationRadius);
        int count = 0;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;


            var otherAlly = col.GetComponentInParent<Ally>();
            if (otherAlly == null) continue;
            if (otherAlly == this) continue;

            Vector2 diff = _rb.position - (Vector2)otherAlly.transform.position;
            float d = diff.magnitude;
            if (d <= 0f) continue;

 
            float strength = Mathf.Clamp01((separationRadius - d) / separationRadius);
            result += diff.normalized * strength;
            count++;
        }

        if (count > 0)
        {
            result = result / count; // average
            result = result.normalized * separationStrength * Mathf.Clamp01(result.magnitude);
        }
        return result;
    }

    private void ReturnTowardOwner()
    {
        // move to a nearby point around the owner (so ally doesn't stack exactly on player)
        if (OwnerNecromancy == null) return;

        Vector2 targetPos = (Vector2)OwnerNecromancy.transform.position + _idleOffset;
        Vector2 ownerPos = (Vector2)OwnerNecromancy.transform.position;
        float currentDist = Vector2.Distance(_rb.position, targetPos);

        // If too far, move closer; else do minor idle (small wander)
        if (currentDist > 0.25f) // get reasonably close to idle offset
        {
            Vector2 dir = (targetPos - _rb.position).normalized;

            // apply separation while returning to avoid stacking near owner
            Vector2 separation = ComputeSeparation();
            Vector2 combined = (dir + separation).normalized;

            _rb.MovePosition(_rb.position + combined * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void FindNearestTarget()
    {
        // Use Physics2D.OverlapCircleAll for reliable detection
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        float bestDist = Mathf.Infinity;
        Transform best = null;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;
            if (!col.CompareTag("Enemy")) continue;

            Transform candidate = col.transform;
            float d = Vector2.Distance(transform.position, candidate.position);

            // Optionally ensure candidate is not too far from the owner (so ally won't chase off-screen)
            if (OwnerNecromancy != null)
            {
                float distToOwner = Vector2.Distance(OwnerNecromancy.transform.position, candidate.position);
                if (distToOwner > maxDistanceFromOwner) continue;
            }

            if (d < bestDist)
            {
                bestDist = d;
                best = candidate;
            }
        }

        _target = best;
    }

    private void ApplyDamageToTarget()
    {
        if (_target == null) return;

        // Prefer IDamageable interface or EnemyHealth, fall back to legacy Enemy
        var dmgable = _target.GetComponentInParent<IDamageable>();
        if (dmgable != null)
        {
            dmgable.TakeDamage(damage);
        }
        else
        {
            var eh = _target.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
            }
            else
            {
                var e = _target.GetComponentInParent<Enemy>();
                if (e != null)
                {
                    e.TakeDamage(damage);
                }
            }
        }

        // If the target was destroyed by the damage, clear reference immediately and retarget
        if (_target == null || !_target.gameObject.activeInHierarchy)
        {
            _target = null;
            FindNearestTarget(); // immediate retarget
        }
    }

    void OnDestroy()
    {
        if (OwnerNecromancy != null)
            OwnerNecromancy.NotifyAllyDestroyed(gameObject);
    }

    // visual debug in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, separationRadius);

        if (OwnerNecromancy != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(OwnerNecromancy.transform.position, maxDistanceFromOwner);
        }
    }
}
