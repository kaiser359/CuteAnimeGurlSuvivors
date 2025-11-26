using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Tooltip("Assign the player's Transform (or leave empty to auto-find GameObject tagged 'Player')")]
    public Transform Player;
    public float PullRange = 20f;
    [Tooltip("Affect how strongly the black hole pulls (force units).")]
    public float PullStrength = 20f;
    [Tooltip("Maximum single-frame force applied to the player.")]
    public float MaxPullForce = 50f;
    [Tooltip("Fallback speed when no Rigidbody2D is present.")]
    public float FallbackMoveSpeed = 2f;

    private Rigidbody2D _playerRb;
    private bool _shouldPull;

    [SerializeField]private float timer;

    void Start()
    {
        if (Player == null)
        {
            var found = GameObject.FindWithTag("Player");
            if (found != null) Player = found.transform;
        }

        if (Player != null)
            _playerRb = Player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Player == null) return;

        float distance = Vector2.Distance(Player.position, transform.position);
        _shouldPull = distance <= PullRange;

        timer += Time.deltaTime * 2;
        if (timer >= 20f)
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!_shouldPull || Player == null) return;

        Vector2 holePos = transform.position;
        Vector2 playerPos = Player.position;
        Vector2 toHole = holePos - playerPos;
        float distance = toHole.magnitude;
        if (distance < 0.001f) return;

        // Strength scales with proximity (closer -> stronger)
        float distanceFactor = 1f - Mathf.Clamp01(distance / PullRange);

        // Reduce pull when player is actively moving (so player retains control)
        float inputMag = InputManager.Movement.magnitude; // 0..1
        float controlReduction = Mathf.Clamp01(inputMag); // reduces pull up to 100% based on input
        float effectiveStrength = PullStrength * distanceFactor * (1f - 0.6f * controlReduction); // reduce pull by up to 60% when input present

        Vector2 force = toHole.normalized * effectiveStrength;
        // clamp the force so it never becomes excessively large
        if (force.magnitude > MaxPullForce) force = force.normalized * MaxPullForce;

        if (_playerRb != null)
        {
            // Use AddForce so physics and player velocity combine naturally
            _playerRb.AddForce(force, ForceMode2D.Force);
        }
        else
        {
            // Fallback: gently move transform but respect player input
            float fallbackFactor = 1f - 0.6f * controlReduction;
            Vector2 newPos = Vector2.MoveTowards(playerPos, holePos, FallbackMoveSpeed * fallbackFactor * Time.fixedDeltaTime);
            Player.position = newPos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, PullRange);
    }
}
