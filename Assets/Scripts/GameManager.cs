using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool Gamehasended= false;
    public GameObject LevelCompleteUI;
 public void LevelWon()
    {
        LevelCompleteUI.SetActive(true);
    }
    public void GameOver()
    {
        if (Gamehasended == false)
        {
            Debug.Log("GameOver");
            Gamehasended = true;
            Invoke("Restart", 3f);

        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
