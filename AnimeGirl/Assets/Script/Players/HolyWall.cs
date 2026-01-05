using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HolyWall : MonoBehaviour
{
   
    public float tickInterval = 0.5f;

   
    public float damagePerTick = 5f;

   
    public float healPerTick = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private readonly HashSet<EnemyHealth> _enemiesInside = new HashSet<EnemyHealth>();
    private PlayerHealth _playerInside;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col == null)
        {
            Debug.LogError("HolyWall requires a Collider2D (set as Trigger).");
        }
        else
        {
          
            if (!_col.isTrigger)
                _col.isTrigger = true;
        }

        StartCoroutine(TickRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // player
        if (other.CompareTag("Player"))
        {
            var ph = other.GetComponentInParent<PlayerHealth>();
            if (ph != null) _playerInside = ph;
            return;
        }

      
        if (other.CompareTag("Enemy"))
        {
            var eh = other.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                _enemiesInside.Add(eh);
                return;
            }

            var dmgable = other.GetComponentInParent<IDamageable>();
            if (dmgable is EnemyHealth asEh)
            {
                _enemiesInside.Add(asEh);
            }
           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;

        if (other.CompareTag("Player"))
        {
            var ph = other.GetComponentInParent<PlayerHealth>();
            if (ph != null && ph == _playerInside) _playerInside = null;
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            var eh = other.GetComponentInParent<EnemyHealth>();
            if (eh != null) _enemiesInside.Remove(eh);
        }
    }

    private IEnumerator TickRoutine()
    {
        var wait = new WaitForSeconds(tickInterval);

        while (true)
        {
            // damage enemies inside
            if (_enemiesInside.Count > 0)
            {
                // copy to avoid modification during iteration
                var copy = new EnemyHealth[_enemiesInside.Count];
                _enemiesInside.CopyTo(copy);
                foreach (var e in copy)
                {
                    if (e == null)
                    {
                        _enemiesInside.Remove(e);
                        continue;
                    }

                    e.TakeDamage(damagePerTick);
                }
            }

            // heal player inside
            if (_playerInside != null)
            {
                _playerInside.health = Mathf.Min(_playerInside.maxHealth, _playerInside.health + healPerTick);
            }

            yield return wait;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }


    private void OnDrawGizmosSelected()
    {
        if (TryGetComponent<Collider2D>(out var c))
        {
            Gizmos.color = new Color(0.7f, 1f, 0.7f, 0.35f);
            if (c is CircleCollider2D circle)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);
            }
            else if (c is BoxCollider2D box)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + (Vector3)box.offset, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(Vector3.zero, new Vector3(box.size.x, box.size.y, 0.1f));
                Gizmos.matrix = Matrix4x4.identity;
            }
            else
            {
                // fallback: draw bounds
                var bounds = c.bounds;
                Gizmos.DrawCube(bounds.center, bounds.size);
            }
        }
    }
}
