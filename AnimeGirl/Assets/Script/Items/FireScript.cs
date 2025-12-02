using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public float damagePerTick = 5f;
    public float tickInterval = 0.5f;
    public float lifetime = 10f;

    [Tooltip("If true the script will add a kinematic Rigidbody2D to this GameObject when no Rigidbody2D is present. This ensures 2D trigger callbacks fire.")]
    public bool autoAddRigidbody = true;

    private readonly Dictionary<Collider2D, float> _timers = new Dictionary<Collider2D, float>();
    private Collider2D _col;
    private float _age;
    //dude this is insane, this could be easier why it wasant working before??????????? AHHHHHHHHHHHHHHHHHHHHHHHHHHHH
    void Start()
    {
        _col = GetComponent<Collider2D>();
        if (_col == null)
        {
           
            return;
        }

        if (!_col.isTrigger)
        {
            
            _col.isTrigger = true;
        }

      
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null && autoAddRigidbody)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.simulated = true;
           
        }
    }

    void Update()
    {
        _age += Time.deltaTime;
        if (_age >= lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (!_timers.ContainsKey(collision)) _timers[collision] = 0f;
        ApplyDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_timers.ContainsKey(collision)) _timers[collision] = 0f;
        _timers[collision] += Time.deltaTime;
        if (_timers[collision] >= tickInterval)
        {
          
            ApplyDamage(collision);
            _timers[collision] = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        _timers.Remove(collision);
    }

    private void ApplyDamage(Collider2D collision)
    {
        if (collision == null) return;

       
        var damageable = collision.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log($"{name}: applying {damagePerTick} via IDamageable to {collision.name}");
            damageable.TakeDamage(damagePerTick);
            return;
        }

       
        var ehParent = collision.GetComponentInParent<EnemyHealth>();
        if (ehParent != null)
        {
           
            ehParent.TakeDamage(damagePerTick);
            return;
        }

        var ehChild = collision.GetComponentInChildren<EnemyHealth>();
        if (ehChild != null)
        {
           
            ehChild.TakeDamage(damagePerTick);
            return;
        }

        
        var eParent = collision.GetComponentInParent<Enemy>();
        if (eParent != null)
        {
           
            eParent.TakeDamage(damagePerTick);
            return;
        }


    }
}
