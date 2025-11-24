using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Awake()
    {
        m_transform = GetComponent<Transform>();

    }
    private void Update()
    {
        Shootlazer();
        if (Input.GetKey(KeyCode.E) && energy >= 0f && cooldownTime >= 3f)
        {
            lazer.SetActive(true);
            energy -= Time.deltaTime * 5f;
            candamage = true;
        }
        else
        {
            lazer.SetActive(false);
            candamage = false;

        }
        if (energy < maxEnergy)
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

        Vector3 MousePos = (Vector2)Cam.ScreenToWorldPoint(Input.mousePosition);
        float angledRad = Mathf.Atan2(MousePos.y - transform.position.y, MousePos.x - transform.position.x);
        float angledDeg = (180 / Mathf.PI) * angledRad ; //offset this by 90 degrees
        lazer.transform.rotation = Quaternion.Euler(0, 0, angledDeg);
        if (damageInterval <= 0.2f)
        {
            damageInterval += Time.deltaTime * 3;
        }
    }

    void Shootlazer()
    {
        if (m_lineRenderer == null) return;

        // use the fire point if assigned, otherwise use this object's transform
        Vector2 origin = lazerFirePoint != null ? (Vector2)lazerFirePoint.position : (Vector2)m_transform.position;
        Vector2 direction = lazerFirePoint != null ? (Vector2)lazerFirePoint.right : (Vector2)transform.right;

        // Cast with a distance so we know where to end the beam if nothing is hit
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, defDistanceRay);
        Vector2 endPoint;
        if (hit.collider != null)
        {
            // endPoint = hit.point;
            endPoint = origin + direction.normalized * defDistanceRay;
            // FIX: RaycastHit2D is a single hit, not a collection. Remove foreach and use hit.collider directly.
            var enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null && candamage && damageInterval >= 0.2f)
            {
                
                enemyHealth.TakeDamage(damage);
                damageInterval = 0f;

            }
            else
            {
                if (candamage == false) return;
                if (damageInterval < 0.2f) return;
                var dmgable = hit.collider.GetComponent<IDamageable>();
                if (dmgable != null) dmgable.TakeDamage(damage);
                damageInterval = 0f;
            }

        }
        else
        {
            // compute a world-space end point: origin + direction * distance
            endPoint = origin + direction.normalized * defDistanceRay;
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
