using UnityEngine;

public class xpDrop : MonoBehaviour
{
    [Tooltip("Optional: assign player transform. If empty the script will auto-find a GameObject tagged 'Player'")]
    public Transform Player;
    [Tooltip("Range at which the XP starts moving toward the player")]
    public float attractionRange = 5f;
    [Tooltip("Max approach speed when very close")]
    public float attractionSpeed = 3f;

    private Rigidbody2D _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Player == null)
        {
            var found = GameObject.FindWithTag("Player");
            if (found != null) Player = found.transform;
        }

        _rb = GetComponent<Rigidbody2D>();
    }

  

    void FixedUpdate()
    {
        if (Player == null) return;

        float distance = Vector2.Distance(transform.position, Player.position);
        if (distance > attractionRange) return;

        // Speed scales with proximity so the motion is gentle when far and slightly stronger when close
        float t = 1f - Mathf.Clamp01(distance / attractionRange);
        float currentSpeed = Mathf.Lerp(0f, attractionSpeed, t);

        if (_rb != null)
        {
            Vector2 newPos = Vector2.MoveTowards(_rb.position, Player.position, currentSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPos);
        }
        else
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, Player.position, currentSpeed * Time.deltaTime);
            transform.position = newPos;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            GrindingLevels grindingLevels = collider.gameObject.GetComponent<GrindingLevels>();
            if (grindingLevels != null)
            {
                grindingLevels.AddXP(1); // Add 1 XP to the player
                Destroy(gameObject); // Destroy the XP drop after collection
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attractionRange);
    }
}
