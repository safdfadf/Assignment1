using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Make sure this is assigned in the Inspector

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false); // Ensure the pause menu is hidden at the start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Make sure to reset the time scale to normal before loading.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
