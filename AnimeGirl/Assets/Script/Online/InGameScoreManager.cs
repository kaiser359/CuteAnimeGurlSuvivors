using TMPro;
using UnityEngine;

public class InGameScoreManager : MonoBehaviour
{
    public MaxScore maxScore;
    public TextMeshProUGUI meshpro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        meshpro.text = "score: " + maxScore.score.ToString();
    }

    public void AddScore(int points)
    {
        maxScore.score += points;
    }
}
