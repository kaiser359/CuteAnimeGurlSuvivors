using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class anotherScore : MonoBehaviour
{
    public MaxScore max;
    public TextMeshProUGUI meshpa;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        meshpa.text = "Max Score: " + max.maxScore.ToString(); 
    }

    private void OnApplicationQuit()
    {
        max.maxScore = 0;
    }
}
