using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IceTrailHitbox : MonoBehaviour
{
    public float lifeTime = 0.6f;
    public float speedMultiplier = 1.5f;
    public float boostDuration = 1.2f;
    bool _active = true;

    void Awake()
    {
        // ensure trigger collider
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        // self-disable after lifeTime if not already destroyed
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

        // attempt several non-invasive ways to temporarily increase player speed:
        // 1) find public float fields or properties named common variants (moveSpeed, speed, walkSpeed)
        // 2) fallback: apply impulse to Rigidbody2D
        var go = other.gameObject;

        // try to modify a public float field/property using reflection
        MonoBehaviour[] comps = go.GetComponents<MonoBehaviour>();
        foreach (var comp in comps)
        {
            if (comp == null) continue;
            Type t = comp.GetType();

            // try fields
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

            // try properties
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

        // fallback: try Rigidbody2D impulse as temporary velocity bump
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 bump = (rb.linearVelocity.magnitude > 0.01f ? rb.linearVelocity.normalized : Vector2.right) * (speedMultiplier - 1f) * 3f;
            rb.AddForce(bump, ForceMode2D.Impulse);
            // can't easily revert Rigidbody2D velocity reliably; just exit
            _active = false;
            return;
        }
    }

    System.Collections.IEnumerator RestoreFieldAfter(FieldInfo field, MonoBehaviour comp, float original, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (comp != null && field != null)
        {
            field.SetValue(comp, original);
        }
        Destroy(gameObject);
    }

    System.Collections.IEnumerator RestorePropertyAfter(PropertyInfo prop, MonoBehaviour comp, float original, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (comp != null && prop != null && prop.CanWrite)
        {
            prop.SetValue(comp, original);
        }
        Destroy(gameObject);
    }
}
