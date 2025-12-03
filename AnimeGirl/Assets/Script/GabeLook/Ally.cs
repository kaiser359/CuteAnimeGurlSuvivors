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

    [Tooltip("How far the ally will search for enemies.")]
    public float detectionRadius = 10f;

    [HideInInspector]
    public Necromancy OwnerNecromancy;

    private Rigidbody2D _rb;
    private Transform _target;
    private float _attackTimer;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _attackTimer = attackInterval;
        StartCoroutine(TargetAndMoveRoutine());
    }

    private IEnumerator TargetAndMoveRoutine()
    {
        while (true)
        {
            FindNearestTarget();
            yield return new WaitForSeconds(0.25f);
        }
    }

    void FixedUpdate()
    {
        if (_target == null) return;

        Vector2 dir = ((Vector2)_target.position - _rb.position);
        float dist = dir.magnitude;
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
            // reset timer when out of range so damage cadence is consistent
            _attackTimer = Mathf.Min(_attackTimer, attackInterval);
        }
    }

    private void FindNearestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float bestDist = detectionRadius;
        Transform best = null;
        Vector2 pos = transform.position;
        foreach (var e in enemies)
        {
            if (e == null) continue;
            float d = Vector2.Distance(pos, e.transform.position);
            if (d <= bestDist)
            {
                bestDist = d;
                best = e.transform;
            }
        }
        _target = best;
    }
    
    private void ApplyDamageToTarget()
    {
        if (_target == null) return;
        Collider2D col = _target.GetComponent<Collider2D>();
        // Prefer IDamageable or EnemyHealth, fall back to legacy Enemy
        var dmgable = _target.GetComponentInParent<IDamageable>();
        if (dmgable != null)
        {
            dmgable.TakeDamage(damage);
            return;
        }

        var eh = _target.GetComponentInParent<EnemyHealth>();
        if (eh != null)
        {
            eh.TakeDamage(damage);
            return;
        }

        var e = _target.GetComponentInParent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(damage);
            return;
        }
    }

    void OnDestroy()
    {
        if (OwnerNecromancy != null)
            OwnerNecromancy.NotifyAllyDestroyed(gameObject);
    }
}