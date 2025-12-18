using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class meleeatk : MonoBehaviour
{
    [Header("Rotation")]
    public bool rotateToMouse = true;
    [Tooltip("Degrees to add to computed angle (useful if sprite faces up/right by default)")]
    public float rotationOffset = 0f;

    [Header("Attack")]
    public KeyCode attackKey = KeyCode.Mouse0;
    public Transform attackOrigin; // optional - defaults to this.transform
    public float attackRange = 0.8f;
    public LayerMask hitMask = ~0;
    public float damage = 10f;
    public float attackCooldown = 0.5f;

    public Animator _animator;


    [Header("Attack window (optional)")]
    [Tooltip("If >0, perform hits over this duration (useful for swing animations)")]
    public float attackHitWindow = 0f;

    private float _cooldownTimer;

    private Vector2 aimDirection;
    private string _currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (attackOrigin == null) attackOrigin = transform;
        _cooldownTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // rotation follows mouse
        if (rotateToMouse)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                //Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
                //Vector2 dir = mouseWorld - transform.position;
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
            }
        }

        // attack input & cooldown
        _cooldownTimer -= Time.deltaTime;
    }

    public void Aim(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>() == Vector2.zero)
            return;

        aimDirection = ctx.ReadValue<Vector2>();
    }

    public void Melee(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        if (_cooldownTimer <= 0f)
        {
            if (attackHitWindow > 0f)
                StartCoroutine(PerformAttackWindow());
            else
                PerformInstantAttack();

            _cooldownTimer = attackCooldown;
        }
    }

    private void PerformInstantAttack()
    {
        Vector2 origin = attackOrigin != null ? (Vector2)attackOrigin.position : (Vector2)transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRange, hitMask);
        ApplyDamageToHits(hits);
        _animator.SetTrigger("Mouse0");
    }

    private IEnumerator PerformAttackWindow()
    {
        float elapsed = 0f;
        float interval = 0.05f; // hit sampling interval
        while (elapsed < attackHitWindow)
        {
            Vector2 origin = attackOrigin != null ? (Vector2)attackOrigin.position : (Vector2)transform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRange, hitMask);
            ApplyDamageToHits(hits);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    private void ApplyDamageToHits(Collider2D[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;

            // try common damage receivers (IDamageable, EnemyHealth, Enemy)
            var dmgable = col.GetComponentInParent<IDamageable>();
            if (dmgable != null)
            {
                dmgable.TakeDamage(damage);
                continue;
            }

            var eh = col.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
                continue;
            }

            var e = col.GetComponentInParent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
                continue;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = (attackOrigin != null) ? attackOrigin.position : transform.position;
        Gizmos.DrawWireSphere(origin, attackRange);
    }

    
}
