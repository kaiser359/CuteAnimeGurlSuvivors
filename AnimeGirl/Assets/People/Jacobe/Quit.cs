using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Quit : MonoBehaviour
{
    private Button button1;
    private Button button2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button1 = transform.Find("Exit").GetComponent<Button>();

        button2 = transform.Find("MainMenu").GetComponent<Button>();

        button1.onClick.AddListener(() =>
        {
            Quit1();
        });

        button2.onClick.AddListener(() =>
        {
            MainMenu();
        });
    }

    public void Quit1()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main MenuTest");
    }

    // Update is called once per frame
    void Update()
    {

    }
}