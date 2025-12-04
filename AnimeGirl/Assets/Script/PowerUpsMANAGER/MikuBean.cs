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

    private PlayerStats statsPlayer;

    private void Awake()
    {
        statsPlayer = GetComponent<PlayerStats>();
        m_transform = GetComponent<Transform>();
        candamage = false;
        lazer.SetActive(false);

    }
   

    private void Update()
    {
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

        Vector3 MousePos = (Vector2)Cam.ScreenToWorldPoint(Input.mousePosition);
        float angledRad = Mathf.Atan2(MousePos.y - transform.position.y, MousePos.x - transform.position.x);
        float angledDeg = (180 / Mathf.PI) * angledRad ; //offset this by 90 degrees
        lazer.transform.rotation = Quaternion.Euler(0, 0, angledDeg);
        if (damageInterval <= 0.2f)
        {
            damageInterval += Time.deltaTime * 3;
        }
    }
    public void PowerSuperCool(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (energy >= 0f && cooldownTime >= 3f)
            {
                lazer.SetActive(true);

                candamage = true;
                if (!lazersound.isPlaying) lazersound.Play();
            }
        }
        if (ctx.canceled || energy <= 0f || cooldownTime < 3f)
        {
            lazer.SetActive(false);
            candamage = false;
            lazersound.Stop();
        }
    }

    void Shootlazer()
    {
        if (m_lineRenderer == null) return;

        // use the fire point if assigned, otherwise use this object's transform
        Vector2 origin = lazerFirePoint != null ? (Vector2)lazerFirePoint.position : (Vector2)m_transform.position;
        Vector2 direction = lazerFirePoint != null ? (Vector2)lazerFirePoint.right : (Vector2)transform.right;

        // cast all hits along the ray so we can damage every hit object
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, defDistanceRay);

        // default end point is full range; you can change to hits[0].point if you want beam to stop at first obstacle
        Vector2 endPoint = origin + direction.normalized * defDistanceRay;

        if (hits != null && hits.Length > 0)
        {
            // sort hits by distance so behavior is deterministic
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
