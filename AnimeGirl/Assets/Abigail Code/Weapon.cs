using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 1;
    public enum WeaponType { Melee, Bullet }
    public GameObject owner;
    public WeaponType weaponType;

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner)
            return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage);
            if(weaponType == WeaponType.Bullet)
            {
                Destroy(gameObject);
            }
        }
        EnemyHealth player = collision.GetComponent<EnemyHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            if (weaponType == WeaponType.Bullet)
            {
                Destroy(gameObject);
            }
        }
        PlayerHealth Player = collision.GetComponent<PlayerHealth>();
        if (Player != null)
        {
            Player.TakeDamage(damage);
            if (weaponType == WeaponType.Bullet)
            {
                Destroy(gameObject);
            }
        }
    }
}
