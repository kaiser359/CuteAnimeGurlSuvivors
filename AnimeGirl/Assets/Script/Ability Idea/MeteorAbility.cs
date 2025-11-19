using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class MeteorAbility : MonoBehaviour
{
    public GameObject meteorPrefab;       // prefab that has MeteorAOE component
    public LayerMask groundLayers;        // optional: layers allowed to target
    public KeyCode throwKey = KeyCode.Mouse1; // right click by default
    public Camera mainCamera;

    private PlayerStats stats;

    [SerializeField]  private float cooldownTimer = 0f;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        // cooldown tick
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        // input
        bool pressed = Input.GetKeyDown(throwKey);

        if (pressed && cooldownTimer <= 0f && meteorPrefab != null)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;   // IMPORTANT for 2D accuracy
            Vector2 targetPos = mouseWorldPos;

            // optional ground layer check
            if (groundLayers != 0)
            {
                Collider2D col = Physics2D.OverlapPoint(targetPos, groundLayers);
                // if (col == null) return;  // if you want to restrict
            }

            SpawnMeteor(targetPos);
            cooldownTimer = stats.meteorCooldown;
        }

    }

    private void SpawnMeteor(Vector2 targetPosition)
    {
        GameObject go = Instantiate(meteorPrefab, (Vector3)targetPosition + Vector3.up * 10f, Quaternion.identity);
        // We spawn above and let the prefab handle the "fall" effect or we can position directly
        MeteorAOE aoe = go.GetComponent<MeteorAOE>();
        if (aoe != null)
        {
            aoe.Initialize(targetPosition, stats.meteorDamage, stats.meteorRadius);
        }
        else
        {
            Debug.LogWarning("MeteorAbility: meteorPrefab missing MeteorAOE component.");
        }
    }

    // Optional: allow other scripts to trigger meteor
    public bool TryTriggerMeteorAt(Vector2 worldPos)
    {
        if (cooldownTimer <= 0f && meteorPrefab != null)
        {
            if (CameraShake.Instance != null)
                CameraShake.Instance.Shake(0.2f, 0.25f);

            SpawnMeteor(worldPos);
            cooldownTimer = stats.meteorCooldown;
            return true;
        }
        return false;
    }

    // Optional helper to show cooldown percentage
    public float GetCooldownRemaining() => cooldownTimer;
}
