using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool Gamehasended= false;
    public GameObject LevelCompleteUI;
    public GameObject GameOverUI;
   
    
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

            Invoke("CallGameOverUi", 5);
          

        }
    }
    void CallGameOverUi()
    {
        GameOverUI.SetActive(true);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
