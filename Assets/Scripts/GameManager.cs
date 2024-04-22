using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool Gamehasended = false;  // Flag to prevent multiple game overs
    public GameObject LevelCompleteUI;  // UI panel for level completion
    public GameObject GameOverUI;       // UI panel for game over
    public GameObject player;           // Reference to the player GameObject
    public GameObject ghost;            // Reference to the ghost GameObject

    // Trigger level completion UI
    public void LevelWon()
    {
        LevelCompleteUI.SetActive(true);
        Invoke("NextLevel", 2f);
        
    }
    void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    // Handle game over logic
    public void GameOver()
    {
        if (!Gamehasended)
        {
            Debug.Log("GameOver");
            Gamehasended = true;
            StopCharacters();  // Freeze player and ghost movements
            Invoke("CallGameOverUi", 3);  // Show game over UI after a delay
        }
    }

    // Activate game over UI
    void CallGameOverUi()
    {
        GameOverUI.SetActive(true);
    }

    // Reload the current level
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Freeze both player and ghost movements
    private void StopCharacters()
    {
        FreezeMovement(player);  // Freeze player
        FreezeMovement(ghost);   // Freeze ghost
        Debug.Log("All characters are now frozen.");
    }

    // Disable movement scripts and freeze Rigidbody
    private void DisableMovementScripts(GameObject character)
    {
        MonoBehaviour[] scripts = character.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script is Player_Controller || script is GhostAI)  // Target movement scripts specifically
            {
                script.enabled = false;
            }
        }

        // Optionally disable animations
        Animator anim = character.GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
        }
    }

    // Helper method to freeze Rigidbody components
    private void FreezeMovement(GameObject character)
    {
        if (character != null)
        {
            Rigidbody rb = character.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            DisableMovementScripts(character);
        }
    }
}
