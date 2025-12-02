using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public GameObject player;
    public GameObject target;
    public float speed;
    public GameObject findPaperFigurine;

    private float distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        findPaperFigurine = GameObject.Find("PaperFigurine");
        if (findPaperFigurine != null)
        {
                       player = findPaperFigurine;
        }
        else
        {
                       player = GameObject.FindWithTag("Player");
        }
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
