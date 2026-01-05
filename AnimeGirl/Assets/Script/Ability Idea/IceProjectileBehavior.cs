using UnityEngine;

public class IceProjectileBehavior : MonoBehaviour
{
    public GameObject speedNodePrefab; // Assign a prefab with a trigger collider
    public float nodeSpawnDistance = 0.5f; // How often to drop a speed zone
    private Vector2 lastSpawnPos;

    public void Launch(Vector2 direction, float speed, float lifetime)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.linearVelocity = direction * speed;

        // Rotate to face travel direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        lastSpawnPos = transform.position;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Drop a "speed node" every few units to create the functional path
        if (Vector2.Distance(transform.position, lastSpawnPos) > nodeSpawnDistance)
        {
            if (speedNodePrefab != null)
                Instantiate(speedNodePrefab, transform.position, Quaternion.identity);

            lastSpawnPos = transform.position;
        }
    }
}