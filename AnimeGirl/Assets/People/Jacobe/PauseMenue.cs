using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenue : MonoBehaviour
{

    public GameObject PauseMenu;
    private bool Paused = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Paused == false)
        {
            PausedMenu();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && Paused == true)
        {
            UnPuasedMenu();
            return;
        }
    }

    private void PausedMenu()
    {
        Paused = true;
        PauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
        return;
    }
    public void UnPuasedMenu()
    {
        Paused = false;
        PauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        return;
    }

}