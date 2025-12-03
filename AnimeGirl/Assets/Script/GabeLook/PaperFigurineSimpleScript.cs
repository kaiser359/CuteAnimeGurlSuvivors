using UnityEngine;

public class PaperFigurineSimpleScript : MonoBehaviour
{
    [SerializeField]private float timetodisappear;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timetodisappear += Time.deltaTime * 1.15f;
        if (timetodisappear >= 5f)
        {
            Destroy(this.gameObject);
        }
    }
}
