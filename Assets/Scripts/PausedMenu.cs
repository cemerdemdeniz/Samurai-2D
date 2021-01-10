using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedMenu : MonoBehaviour
{
    
   public static bool gameIsPaused = false;
    public GameObject pausedMenuUı;

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    void Resume()
    {
        pausedMenuUı.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pausedMenuUı.SetActive(true);
        Time.timeScale=0f;
        gameIsPaused = true;
            
    }
}
