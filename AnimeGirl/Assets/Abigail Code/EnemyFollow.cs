using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public GameObject player;
    public GameObject target;
    public float speed;
    public GameObject findPaperFigurine;

    public float distanceToStop = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        findPaperFigurine = GameObject.FindWithTag("PaperFigurine");

        if (findPaperFigurine != null)
        {
            player = findPaperFigurine;
        }
        else
        {
            player = GameObject.FindWithTag("Player");
        }
        if (target == null)
        {
            target = GameObject.Find("Player");
        }
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    
        if (distance > distanceToStop)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }
}
