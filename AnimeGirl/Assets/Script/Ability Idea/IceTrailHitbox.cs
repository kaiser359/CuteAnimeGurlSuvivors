using System;
using System.Collections;
using System.Reflection;
using UnityEngine;


public class IceTrailHitbox : MonoBehaviour
{
    public float lifeTime = 0.6f;
    public float speedMultiplier = 1.5f;
    public float boostDuration = 1.2f;
    bool _active = true;

    void Awake()
    {
      
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        
        if (lifeTime > 0f)
            Invoke(nameof(Disable), lifeTime);
    }

    void Disable()
    {
        _active = false;
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_active) return;
        if (!other.CompareTag("Player")) return;

        
        var go = other.gameObject;

       
        MonoBehaviour[] comps = go.GetComponents<MonoBehaviour>();
        foreach (var comp in comps)
        {
            if (comp == null) continue;
            Type t = comp.GetType();

           
            FieldInfo f = t.GetField("moveSpeed", BindingFlags.Public | BindingFlags.Instance)
                        ?? t.GetField("speed", BindingFlags.Public | BindingFlags.Instance)
                        ?? t.GetField("walkSpeed", BindingFlags.Public | BindingFlags.Instance);
            if (f != null && f.FieldType == typeof(float))
            {
                float orig = (float)f.GetValue(comp);
                f.SetValue(comp, orig * speedMultiplier);
                StartCoroutine(RestoreFieldAfter(f, comp, orig, boostDuration));
                _active = false;
                return;
            }

          
            PropertyInfo p = t.GetProperty("moveSpeed", BindingFlags.Public | BindingFlags.Instance)
                             ?? t.GetProperty("speed", BindingFlags.Public | BindingFlags.Instance)
                             ?? t.GetProperty("walkSpeed", BindingFlags.Public | BindingFlags.Instance);
            if (p != null && p.PropertyType == typeof(float) && p.CanRead && p.CanWrite)
            {
                float orig = (float)p.GetValue(comp);
                p.SetValue(comp, orig * speedMultiplier);
                StartCoroutine(RestorePropertyAfter(p, comp, orig, boostDuration));
                _active = false;
                return;
            }
        }

        
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 bump = (rb.linearVelocity.magnitude > 0.01f ? rb.linearVelocity.normalized : Vector2.right) * (speedMultiplier - 1f) * 3f;
            rb.AddForce(bump, ForceMode2D.Impulse);
            
            _active = false;
            return;
        }
    }

    IEnumerator RestoreFieldAfter(FieldInfo field, MonoBehaviour comp, float original, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (comp != null && field != null)
        {
            field.SetValue(comp, original);
        }
        Destroy(gameObject);
    }

    IEnumerator RestorePropertyAfter(PropertyInfo prop, MonoBehaviour comp, float original, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (comp != null && prop != null && prop.CanWrite)
        {
            prop.SetValue(comp, original);
        }
        Destroy(gameObject);
    }
}
