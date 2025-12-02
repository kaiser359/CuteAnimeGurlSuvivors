using System.Collections;
using UnityEngine;

public class Evasion : MonoBehaviour
{
    [Tooltip("Key to trigger the dash")]
    public KeyCode dashKey = KeyCode.Space;
    [Tooltip("Distance the player dashes")]
    public float dashDistance = 5f;
    [Tooltip("How long the dash lasts (seconds)")]
    public float dashDuration = 0.15f;
    [Tooltip("Cooldown between dashes (seconds)")]
    public float dashCooldown = 0.5f;

    private Rigidbody2D _rb;
    private Collider2D _col;
    private bool _isDashing;
    private float _lastDashTime = -10f;
    private RigidbodyConstraints2D _originalConstraints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // preserved comment block — cache components
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        if (_rb != null) _originalConstraints = _rb.constraints;
    }

    // Update is called once per frame
    void Update()
    {
        // preserved comment block — handle dash input
        if (Input.GetKeyDown(dashKey) && !_isDashing && Time.time >= _lastDashTime + dashCooldown)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;
        _lastDashTime = Time.time;

        if (_rb != null)
        {
            // ensure rotation remains frozen during dash
            _rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        }

        if (_col != null)
        {
            // disable collider so player can pass through objects while dashing
            _col.enabled = false;
        }

        // determine dash direction from input; fallback to current up if no input
        Vector2 inputDir = InputManager.Movement;
        Vector2 dashDir = inputDir.sqrMagnitude > 0.0001f ? inputDir.normalized : (Vector2)transform.up;
        Vector2 startPos = _rb != null ? _rb.position : (Vector2)transform.position;
        Vector2 targetPos = startPos + dashDir * dashDistance;

        float elapsed = 0f;
        // move using physics-friendly MovePosition in FixedUpdate cadence
        while (elapsed < dashDuration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / dashDuration);
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, t);

            if (_rb != null)
                _rb.MovePosition(newPos);
            else
                transform.position = newPos;

            yield return new WaitForFixedUpdate();
        }

        // ensure final position
        if (_rb != null)
            _rb.MovePosition(targetPos);
        else
            transform.position = targetPos;

        // restore collider and constraints
        if (_col != null) _col.enabled = true;
        if (_rb != null) _rb.constraints = _originalConstraints;

        _isDashing = false;
    }

    // Optional: expose whether currently dashing
    public bool IsDashing => _isDashing;
}
