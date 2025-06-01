using System.Collections;
using System.Collections.Generic;
//using Mirror.BouncyCastle.Bcpg;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public GameObject SettingsMenu;

    public bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    public void ResumeGame()
    { 
        pauseMenu.SetActive(false);
        SettingsMenu.SetActive(false);

        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        SettingsMenu.SetActive(false);

        isPaused = true;
    }

   






}





