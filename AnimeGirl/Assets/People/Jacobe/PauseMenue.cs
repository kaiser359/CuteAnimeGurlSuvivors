using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenue : MonoBehaviour
{

    private GameObject PauseMenu;
    public string pauseMenuTag;

    private void Awake()
    {
        PauseMenu = GameObject.FindWithTag(pauseMenuTag);
    }

    public void TogglePause(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        if (PauseMenu.activeSelf == false)
        {
            PausedMenu();
        }
        else
        {
            UnPuasedMenu();
        }
    }

    private void PausedMenu()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        return;
    }
    public void UnPuasedMenu()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        return;
    }

}