using UnityEngine;
using UnityEngine.InputSystem;

public class Icefrost : MonoBehaviour
{
    void Start() { }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    [Header("Ice Skill")]
    public GameObject icePrefab;           // assign prefab (preferably with sprite)
    public Transform firePoint;            // optional spawn point; falls back to this.transform
    public Camera cam;                     // optional camera; falls back to Camera.main or teleport's camera
    public float iceForce = 12f;           // now used as follow speed (world units/sec)
    public float lifeTime = 6f;
    public float cooldown = 4f;            // 4 second cooldown required
    public float cooldownTimer = 0f;

    // reference to TeleportOnFireHover (optional)
    private TeleportOnFireHover _teleportHover;

    private void Awake()
    {
        _teleportHover = GetComponent<TeleportOnFireHover>()
                         ?? GetComponentInParent<TeleportOnFireHover>()
                         ?? FindObjectOfType<TeleportOnFireHover>();

        if (cam == null && _teleportHover != null)
            cam = _teleportHover.playerCamera;
    }

    // Player presses button to spawn/throw an ice object. 4s cooldown enforced.
    public void iceSkill(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;           // only handle press start
        if (cooldownTimer > 0f) return;     // still cooling down
        if (icePrefab == null) return;      // nothing to spawn

        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;

        // instantiate projectile
        GameObject ice = Instantiate(icePrefab, origin, Quaternion.identity);

        // attach or configure IceProjectile for continuous following
        IceProjectile proj = ice.GetComponent<IceProjectile>();
        if (proj == null) proj = ice.AddComponent<IceProjectile>();
        proj.followCamera = cam != null ? cam : Camera.main;
        proj.teleportHover = _teleportHover;
        proj.speed = iceForce;
        proj.lifetime = lifeTime;

        // orient initial rotation toward current cursor/aim
        Vector2 initialDir = transform.right;
        if (_teleportHover != null && _teleportHover.playerCamera != null)
        {
            Camera pc = _teleportHover.playerCamera;
            float zDistance = Mathf.Abs(pc.transform.position.z - transform.position.z);
            Vector3 screenPoint = new Vector3(_teleportHover.aimPosition.x, _teleportHover.aimPosition.y, zDistance);
            Vector3 world = pc.ScreenToWorldPoint(screenPoint);
            world.z = transform.position.z;
            Vector2 toAim = (world - transform.position);
            if (toAim.sqrMagnitude > 0.0001f) initialDir = toAim.normalized;
        }
        else if (proj.followCamera != null)
        {
            float zDistance = Mathf.Abs(proj.followCamera.transform.position.z - transform.position.z);
            Vector3 screenPoint = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), zDistance);
            Vector3 mouseWorld = proj.followCamera.ScreenToWorldPoint(screenPoint);
            mouseWorld.z = transform.position.z;
            Vector2 toMouse = (mouseWorld - transform.position);
            if (toMouse.sqrMagnitude > 0.0001f) initialDir = toMouse.normalized;
        }
        float angle = Mathf.Atan2(initialDir.y, initialDir.x) * Mathf.Rad2Deg;
        ice.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // ensure TrailRenderer to leave a trail while moving
        TrailRenderer trail = ice.GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = ice.AddComponent<TrailRenderer>();
            trail.time = 0.6f;
            trail.startWidth = 0.18f;
            trail.endWidth = 0f;
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = new Color(0.6f, 0.9f, 1f, 0.9f);
            trail.material = mat;
            trail.startColor = new Color(0.6f, 0.9f, 1f, 0.9f);
            trail.endColor = new Color(0.2f, 0.5f, 1f, 0f);
        }

        // rely on IceProjectile to destroy itself, but keep a safety fallback
        Destroy(ice, lifeTime + 0.5f);

        cooldownTimer = cooldown;
    }
}