using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class LeverTrigger : MonoBehaviour
{
    public GameObject uiText;  // UI text object that prompts the player to press X
    public TextMeshProUGUI feedbackText;  // TextMeshPro UI Text for displaying feedback messages
    public LeverAnimation leverAnimationScript;  // Reference to the lever animation script
    public Player_Inventory playerInventory;  // Reference to the player's inventory script
    public bool isFlipped = false;  // Track whether the lever is flipped

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiText.SetActive(true);  // Show the UI text when the player is near the lever
            playerInventory = other.GetComponent<Player_Inventory>();  // Get the player's inventory component
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiText.SetActive(false);  // Hide the UI text when the player leaves the lever area
            feedbackText.gameObject.SetActive(false);  // Also hide the feedback text
        }
    }

    void Update()
    {
        if (uiText.activeInHierarchy && Input.GetKeyDown(KeyCode.X))
        {
            TryActivateLever();  // Try to activate the lever when X is pressed
        }
    }

    private void TryActivateLever()
    {
        if (playerInventory != null && playerInventory.numberofGears >= 5)
        {
            leverAnimationScript.ToggleLever();  // Flip the lever
            isFlipped = !isFlipped;  // Toggle the flipped state
            playerInventory.UseGears(5);  // Remove 5 gears from the player's inventory
            feedbackText.gameObject.SetActive(false);  // Hide feedback text after successful activation
        }
        else
        {
            feedbackText.text = "Collect at least 5 collectables to flip this lever.";  // Update feedback message
            feedbackText.gameObject.SetActive(true);  // Show feedback text
        }
    }
}
