using UnityEngine;

public class MikuBean : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 100;
    public Transform lazerFirePoint;
    public LineRenderer m_lineRenderer;
    Transform m_transform;
    public GameObject lazer;
    [SerializeField]private float energy = 20f;
    [SerializeField]private float maxEnergy = 20f;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();

    }
    private void Update()
    {
        Shootlazer();
        if (Input.GetKey(KeyCode.E) && energy >= 0f)
        {
            lazer.SetActive(true);
            energy -= Time.deltaTime * 5f;
        }
        else
        {
            lazer.SetActive(false);

        }
        if (energy <= maxEnergy)
        {
            energy += Time.deltaTime * 3f;
        }
        if (energy >= maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    void Shootlazer()
    {
        if (Physics2D.Raycast(m_transform.position, transform.right))
        {
            RaycastHit2D hit = Physics2D.Raycast(lazerFirePoint.position, transform.right);
            Draw2DRay(lazerFirePoint.position, hit.point);
        }
        else 
        {
            Draw2DRay(lazerFirePoint.position, lazerFirePoint.transform.right * defDistanceRay);
        }
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

}
