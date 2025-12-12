using System.Linq;
using UnityEngine;

public class BlackHole : MonoBehaviour
{

    public float PullRange = 20f;

    public float PullStrength = 20f;

    public float MaxPullForce = 50f;

    public float FallbackMoveSpeed = 2f;

    private Rigidbody2D _playerRb;
    private bool _shouldPull;

    [SerializeField]private float timer;

    void Update()
    {
        var player = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None)
            .Where(item => Vector2.Distance(item.transform.position, transform.position) < PullRange)
            .FirstOrDefault()
            ?.GetComponent<PlayerMovement>();

        if (!player)
            return;

        float distance = Vector2.Distance(player.transform.position, transform.position);
        _shouldPull = distance <= PullRange;

        timer += Time.deltaTime * 2;
        if (timer >= 20f)
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        var player = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None)
            .Where(item => Vector2.Distance(item.transform.position, transform.position) < PullRange)
            .FirstOrDefault()
            ?.GetComponent<PlayerMovement>();

        if (!_shouldPull || player == null) return;

        Vector2 holePos = transform.position;
        Vector2 playerPos = player.transform.position;
        Vector2 toHole = holePos - playerPos;
        float distance = toHole.magnitude;
        if (distance < 0.001f) return;

        
        float distanceFactor = 1f - Mathf.Clamp01(distance / PullRange);

        float inputMag = player.lastMoveDirection.magnitude; 
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
            player.transform.position = newPos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, PullRange);
    }


}
