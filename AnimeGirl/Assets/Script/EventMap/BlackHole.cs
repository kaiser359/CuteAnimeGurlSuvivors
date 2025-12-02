using UnityEngine;

public class BlackHole : MonoBehaviour
{

    public Transform Player;
    public float PullRange = 20f;

    public float PullStrength = 20f;

    public float MaxPullForce = 50f;

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

        
        float distanceFactor = 1f - Mathf.Clamp01(distance / PullRange);

        
        float inputMag = InputManager.Movement.magnitude; 
        float controlReduction = Mathf.Clamp01(inputMag); 
        float effectiveStrength = PullStrength * distanceFactor * (1f - 0.6f * controlReduction); 

        Vector2 force = toHole.normalized * effectiveStrength;
       
        if (force.magnitude > MaxPullForce) force = force.normalized * MaxPullForce;

        if (_playerRb != null)
        {
           
            _playerRb.AddForce(force, ForceMode2D.Force);
        }
        else
        {
            
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
