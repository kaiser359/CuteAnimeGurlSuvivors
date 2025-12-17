using UnityEngine;

public class destroywall : MonoBehaviour
{
    public float destroyTime = 10f; // seconds
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
