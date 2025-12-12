using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        if(shootTimer > shootCooldown)
        {
            Debug.Log("HELLO IS THIS SHOOTING");
            shootTimer = 0;
            GameObject intBullet = Instantiate(bullet, Aim.position, Aim.rotation);
            intBullet.GetComponent<Rigidbody2D>().AddForce(-Aim.up * fireForce, ForceMode2D.Impulse);
            intBullet.GetComponent<Weapon>().owner = gameObject;
           
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

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

    
}
