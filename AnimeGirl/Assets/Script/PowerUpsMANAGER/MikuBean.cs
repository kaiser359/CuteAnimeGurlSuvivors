using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
[RequireComponent(typeof(PlayerStats))]
public class MikuBean : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 100;
    public Transform lazerFirePoint;
    public LineRenderer m_lineRenderer;
    Transform m_transform;
    public GameObject lazer;
    [SerializeField]private float energy = 20f;
    [SerializeField]private float maxEnergy = 20f;
    [SerializeField] private float cooldownTime = 3f;
    [SerializeField] private bool isCoolingDown = false;
    [SerializeField] private Camera Cam;
    private float damage = 2f;
    [SerializeField]private bool candamage = false;
    [SerializeField] private float damageInterval = 0.3f;
    public AudioSource lazersound;
    private bool isPressing = false;
    private PlayerStats statsPlayer;

    private Vector2 aimDirection; // legacy input fallback
    private Vector2 _currentAimDir = Vector2.right; // normalized direction used by Shootlazer
    private TeleportOnFireHover _teleportHover;

    private void Awake()
    {
        statsPlayer = GetComponent<PlayerStats>();
        m_transform = GetComponent<Transform>();
        candamage = false;
        if (lazer != null) lazer.SetActive(false);

        // try find TeleportOnFireHover on this object, parents or any instance in scene
        _teleportHover = GetComponent<TeleportOnFireHover>() 
                         ?? GetComponentInParent<TeleportOnFireHover>() 
                         ?? FindObjectOfType<TeleportOnFireHover>();
        // ensure we have a camera reference; prefer assigned Cam, else try teleport's camera, else Camera.main
        if (Cam == null && _teleportHover != null)
            Cam = _teleportHover.playerCamera;
        if (Cam == null)
            Cam = Camera.main;
    }
   

    private void Update()
    {
        // update active state/sound
        if (energy >= 0f && cooldownTime >= 3f && isPressing)
        {
            if (lazer != null) lazer.SetActive(true);
            candamage = true;
            if (lazersound != null && !lazersound.isPlaying) lazersound.Play();
        }
        if (energy <= 0f || cooldownTime < 3f || !isPressing)
        {
            Debug.Log("Out of energy");
            if (lazer != null) lazer.SetActive(false);
            candamage = false;
            if (lazersound != null) lazersound.Stop();
        }

        // compute aim direction using TeleportOnFireHover cursor if available,
        // otherwise fallback to legacy InputSystem aimDirection or mouse position
        Vector3 mouseWorldPos;
        if (_teleportHover != null && Cam != null)
        {
            // compute proper z distance so ScreenToWorldPoint maps to the plane of this object
            float zDistance = Mathf.Abs(Cam.transform.position.z - transform.position.z);
            Vector3 screenPoint = new Vector3(_teleportHover.aimPosition.x, _teleportHover.aimPosition.y, zDistance);
            mouseWorldPos = Cam.ScreenToWorldPoint(screenPoint);
            mouseWorldPos.z = transform.position.z;
        }
        else if (Cam != null)
        {
            float zDistance = Mathf.Abs(Cam.transform.position.z - transform.position.z);
            Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance);
            mouseWorldPos = Cam.ScreenToWorldPoint(screenPoint);
            mouseWorldPos.z = transform.position.z;
        }
        else
        {
            // final fallback: use aimDirection from input
            Vector2 fallbackDir = aimDirection.sqrMagnitude > 0.001f ? aimDirection.normalized : (Vector2)transform.right;
            _currentAimDir = fallbackDir;
            mouseWorldPos = (Vector3)m_transform.position + (Vector3)_currentAimDir * defDistanceRay;
        }

        // compute final direction from fire origin to mouseWorldPos
        Vector2 origin = lazerFirePoint != null ? (Vector2)lazerFirePoint.position : (Vector2)m_transform.position;
        Vector2 dir = (mouseWorldPos - (Vector3)origin);
        if (dir.sqrMagnitude > 0.0001f)
            _currentAimDir = dir.normalized;

        // rotate the laser object to face the cursor
        float angledRad = Mathf.Atan2(_currentAimDir.y, _currentAimDir.x);
        float angledDeg = angledRad * Mathf.Rad2Deg;
        if (lazer != null) lazer.transform.rotation = Quaternion.Euler(0, 0, angledDeg);

        Shootlazer();

        if (energy < maxEnergy && candamage == false)
        {
            energy += Time.deltaTime * 3f;
        }
        if (energy >= maxEnergy)
        {
            energy = maxEnergy;
        }
        if (energy <= 0) 
        { 
            cooldownTime = 0f;
            isCoolingDown = true;
        }
        if (isCoolingDown)
        {
            cooldownTime += Time.deltaTime * 3;
            if (cooldownTime >= 3f)
            {
                isCoolingDown = false;
            }
        }
        if (candamage == true) {
            energy -= Time.deltaTime * 5f;
        }

        if (damageInterval <= 0.2f)
        {
            damageInterval += Time.deltaTime * 1.15f;
        }
    }

    public void Aim(InputAction.CallbackContext ctx)
    {
        aimDirection = ctx.ReadValue<Vector2>();
    }

    public void PowerSuperCool(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isPressing = true;
        }
        if (ctx.canceled)
        {
            isPressing = false;
        }
    }

    void Shootlazer()
    {
        if (m_lineRenderer == null) return;

        // use the fire point if assigned, otherwise use this object's transform
        Vector2 origin = lazerFirePoint != null ? (Vector2)lazerFirePoint.position : (Vector2)m_transform.position;
        Vector2 direction = _currentAimDir;

        // cast all hits along the ray so we can damage every hit object
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, defDistanceRay);

        // default end point is full range
        Vector2 endPoint = origin + direction * defDistanceRay;

        if (hits != null && hits.Length > 0)
        {
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // apply damage to all hit objects at once (respecting damage interval)
            if (candamage && damageInterval >= 0.2f)
            {
                foreach (var h in hits)
                {
                    if (h.collider == null) continue;
                    if (h.collider.gameObject == gameObject) continue; // avoid hitting self

                    var enemyHealth = h.collider.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(statsPlayer.baseMikuBean);
                        continue;
                    }

                    var dmgable = h.collider.GetComponent<IDamageable>();
                    if (dmgable != null)
                        dmgable.TakeDamage(statsPlayer.baseMikuBean);
                }

                damageInterval = 0f;
            }

            // if you want the beam to stop at first hit uncomment following:
            // endPoint = origin + direction * hits[0].distance;
        }

        Draw2DRay(origin, endPoint);
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        // ensure renderer has two positions
        if (m_lineRenderer.positionCount < 2)
            m_lineRenderer.positionCount = 2;

        m_lineRenderer.SetPosition(0, new Vector3(startPos.x, startPos.y, 0f));
        m_lineRenderer.SetPosition(1, new Vector3(endPos.x, endPos.y, 0f));
    }

}
