using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuUI.activeInHierarchy)
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
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause game time
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Make sure to resume game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }

    public void QuitGame()
    {
        Application.Quit(); // Quit the game
    }
}

