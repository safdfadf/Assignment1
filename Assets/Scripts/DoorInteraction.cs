using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Make sure this namespace is included if you're using SceneManager
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

public class DoorInteraction : MonoBehaviour
{
    public LeverTrigger[] levers;  // Array of all levers that control the door
    public Text interactionText;   // UI Text for displaying messages
    private bool playerIsNear = false;  // Tracks proximity of the player
    private bool doorIsOpen = false;    // Tracks if the door is open

    void Update()
    {
        // Check player interaction
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            if (AreAllLeversFlipped())
            {
                doorIsOpen = !doorIsOpen;  // Toggle the door state
                if (doorIsOpen)
                {
                    // Call level completion logic when the door opens
                    CompleteLevel();
                }
                StartCoroutine(UpdateTextNextFrame());  // Update UI text after a frame
            }
            else
            {
                interactionText.text = "You need to flip all levers to open the doors";  // Notify player
            }
        }
    }

    IEnumerator UpdateTextNextFrame()
    {
        yield return null;  // Wait for one frame
        interactionText.text = doorIsOpen ? "[E] Close Door" : "[E] Open Door";  // Update text based on door state
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TogglePlayerProximity(true);  // Player has entered the trigger area
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TogglePlayerProximity(false);  // Player has left the trigger area
        }
    }

    private void TogglePlayerProximity(bool isNear)
    {
        playerIsNear = isNear;
        interactionText.gameObject.SetActive(isNear);  // Show/hide UI text based on player proximity
    }

    private bool AreAllLeversFlipped()
    {
        // Check the state of all levers
        foreach (var lever in levers)
        {
            if (!lever.isFlipped) return false;  // Return false if any lever is not flipped
        }
        return true;  // Return true if all levers are flipped
    }

    private void CompleteLevel()
    {
        Debug.Log("Level Completed!");  // Log level completion
        // Additional level completion logic can go here (e.g., load a new scene)
        // Uncomment below to load a new scene
        // SceneManager.LoadScene("NextLevel");
    }
}