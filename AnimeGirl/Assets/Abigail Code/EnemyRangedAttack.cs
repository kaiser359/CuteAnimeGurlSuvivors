using JetBrains.Annotations;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class EnemyRangedAttack : MonoBehaviour
{
    public Transform target;
    public float rotateSpeed = 0.0025f;
    private Rigidbody2D rb;
    public GameObject bulletPrefab;
    public float distanceToShoot = 5f;
    public Transform firingPoint;
    public float fireRate;
    private float timeToFire;
    public float fireForce = 1f;
    public float lifeTime = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (!target)
        {
            GetTarget();
        }
        else
        {
            RotateTowardsTarget();
        }

        if (target != null && Vector2.Distance(target.position, transform.position) <= distanceToShoot)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (timeToFire <= 0f)
        {
            Debug.Log("Shoot");
            GameObject intBullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
            intBullet.GetComponent<Rigidbody2D>().AddForce(-firingPoint.up * -fireForce, ForceMode2D.Impulse);
            intBullet.GetComponent<Weapon>().owner = gameObject;
            timeToFire = fireRate;
            Destroy(intBullet, lifeTime);
        }
        else
        {
            timeToFire -= Time.deltaTime;
        }
    }

    private void RotateTowardsTarget()
    {
        Vector2 targetDirection = target.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);
    }

    private void GetTarget()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
