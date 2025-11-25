using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Melee;
    float atkDuration = 0.3f;
    float atkTimer = 0f;
    bool isAttacking = false;
    public float damage = 1;
    public Transform Aim;
    public GameObject bullet;
    public float fireForce = 10f;
    float shootCooldown = 0.25f;
    float shootTimer = 0.5f;
    public enum WeaponType { Melee, Bullet };
    public WeaponType weaponType;
    // Update is called once per frame
    void Update()
    {
        CheckMeleeTimer();
        shootTimer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            OnAttack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnShoot();
        }
    }

    void OnShoot()
    {
        if(shootTimer > shootCooldown)
        {
            shootTimer = 0;
            GameObject intBullet = Instantiate(bullet, Aim.position, Aim.rotation);
            intBullet.GetComponent<Rigidbody2D>().AddForce(-Aim.up * fireForce, ForceMode2D.Impulse);
            Destroy(intBullet, 2f);
        }
    }

    void OnAttack()
    {
        if (!isAttacking)
        {
            Melee.SetActive(true);
            isAttacking = true;
        }
    }

    void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            atkTimer += Time.deltaTime;
            if( atkTimer >= atkDuration)
            {
                atkTimer = 0;
                isAttacking= false;
                Melee.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            if(weaponType == WeaponType.Bullet)
            {
                Destroy(gameObject);
            }
        }
    }
}
