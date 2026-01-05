using UnityEngine;
using UnityEngine.InputSystem;

public class Icefrost : MonoBehaviour
{
    [Header("Ice Skill")]
    public GameObject icePrefab;
    public Transform firePoint;
    public Camera cam;
    public float iceSpeed = 12f;
    public float lifeTime = 6f;
    public float cooldown = 4f;
    public float cooldownTimer = 0f;

    private TeleportOnFireHover _teleportHover;

    private void Awake()
    {
        _teleportHover = GetComponent<TeleportOnFireHover>()
                         ?? GetComponentInParent<TeleportOnFireHover>()
                         ?? FindObjectOfType<TeleportOnFireHover>();

        if (cam == null && _teleportHover != null)
            cam = _teleportHover.playerCamera;
    }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void iceSkill(InputAction.CallbackContext ctx)
    {
        if (!ctx.started || cooldownTimer > 0f || icePrefab == null) return;

        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        Vector2 fireDirection = transform.right; // Default

        // Use your specific AimPosition/Camera logic
        if (_teleportHover != null && _teleportHover.playerCamera != null)
        {
            Camera pc = _teleportHover.playerCamera;
            float zDistance = Mathf.Abs(pc.transform.position.z - transform.position.z);
            Vector3 screenPoint = new Vector3(_teleportHover.aimPosition.x, _teleportHover.aimPosition.y, zDistance);
            Vector3 worldPoint = pc.ScreenToWorldPoint(screenPoint);
            worldPoint.z = transform.position.z;

            fireDirection = ((Vector2)worldPoint - origin).normalized;
        }

        // Spawn Projectile
        GameObject ice = Instantiate(icePrefab, origin, Quaternion.identity);

        // Setup Movement
        IceProjectileBehavior proj = ice.GetComponent<IceProjectileBehavior>();
        if (proj == null) proj = ice.AddComponent<IceProjectileBehavior>();

        proj.Launch(fireDirection, iceSpeed, lifeTime);

        cooldownTimer = cooldown;
    }
}