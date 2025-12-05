using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ally : MonoBehaviour
{
    [Tooltip("Movement speed of the ally.")]
    public float moveSpeed = 3f;

    [Tooltip("Damage applied to enemy health.")]
    public float damage = 5f;

    [Tooltip("How often (seconds) the ally deals damage while in range.")]
    public float attackInterval = 0.5f;

    [Tooltip("Range at which the ally considers itself in melee range and deals damage.")]
    public float attackRange = 0.8f;

    [Tooltip("How far the ally will search for enemies (from the ally's position).")]
    public float detectionRadius = 10f;

    [Tooltip("Max distance the ally may stray from the player (owner).")]
    public float maxDistanceFromOwner = 6f;

    [Tooltip("How long (seconds) the ally lives before dying automatically.")]
    public float lifeTime = 30f;

    [HideInInspector]
    public Necromancy OwnerNecromancy;

    private Rigidbody2D _rb;
    private Transform _target;
    private float _attackTimer;
    private Vector2 _cachedOwnerPos;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _attackTimer = attackInterval;

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

        // If no target, idle near owner
        if (_target == null)
        {
            ReturnTowardOwner();
            return;
        }

        // If target destroyed unexpectedly, clear and find a new one immediately
        if (_target == null || !_target.gameObject.activeInHierarchy)
        {
            _target = null;
            FindNearestTarget();
            return;
        }

        Vector2 dir = ((Vector2)_target.position - _rb.position);
        float dist = dir.magnitude;

        // If chasing target would put ally further than allowed distance from owner, don't chase
        if (OwnerNecromancy != null)
        {
            float distTargetToOwner = Vector2.Distance(_cachedOwnerPos, _target.position);
            if (distTargetToOwner > maxDistanceFromOwner)
            {
                // instead go back toward the owner (stay nearby)
                ReturnTowardOwner();
                _attackTimer = attackInterval; // reset (don't spam leftover timer)
                return;
            }
        }

        // Move toward target
        if (dist > 0.01f)
        {
            Vector2 vel = dir.normalized * moveSpeed;
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
            // reset timer when out of range so cadence is consistent
            _attackTimer = Mathf.Min(_attackTimer, attackInterval);
        }
    }

    private void ReturnTowardOwner()
    {
        // move to a nearby point around the owner (so ally doesn't stack exactly on player)
        if (OwnerNecromancy == null) return;

        Vector2 ownerPos = (Vector2)OwnerNecromancy.transform.position;
        float currentDist = Vector2.Distance(_rb.position, ownerPos);

        // If too far, move closer; else do minor idle (small wander)
        if (currentDist > 1.2f)
        {
            Vector2 dir = (ownerPos - _rb.position).normalized;
            _rb.MovePosition(_rb.position + dir * moveSpeed * Time.fixedDeltaTime);
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

        if (OwnerNecromancy != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(OwnerNecromancy.transform.position, maxDistanceFromOwner);
        }
    }
}
