using UnityEngine;

public class IceProjectileBehavior : MonoBehaviour
{
    public GameObject speedNodePrefab;
    public float nodeSpawnDistance = 0.5f; 
    private Vector2 lastSpawnPos;

    public void Launch(Vector2 direction, float speed, float lifetime)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        lastSpawnPos = transform.position;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
      
        if (Vector2.Distance(transform.position, lastSpawnPos) > nodeSpawnDistance)
        {
            if (speedNodePrefab != null)
                Instantiate(speedNodePrefab, transform.position, Quaternion.identity);

            lastSpawnPos = transform.position;
        }
    }
}