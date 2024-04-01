using UnityEngine;
using UnityEngine.UI; 
using System.Collections; 

public class DoorInteraction : MonoBehaviour
{
    public LeftDoorController leftDoor;
    public RightDoorController rightDoor;
    public Text interactionText; 
    private bool playerIsNear = false;

    void Update()
    {
        // Check if the player is near and presses the 'E' key
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle the state of both doors
            leftDoor.ToggleDoor();
            rightDoor.ToggleDoor();

            
            StartCoroutine(UpdateTextNextFrame());
        }
    }

    IEnumerator UpdateTextNextFrame()
    {
        yield return null; 
        UpdateInteractionText(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TogglePlayerProximity(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TogglePlayerProximity(false);
        }
    }

    private void TogglePlayerProximity(bool isNear)
    {
        playerIsNear = isNear;
        interactionText.gameObject.SetActive(isNear); // Show or hide the UI text based on proximity

        if (isNear)
        {
            UpdateInteractionText(); 
        }
    }

    
    void UpdateInteractionText()
    {
        if (leftDoor.isOpen || rightDoor.isOpen) // Check if any of the doors are open
        {
            interactionText.text = "[E] Close Door"; // Doors are open, show "Close" text
        }
        else
        {
            interactionText.text = "[E] Open Door"; // Doors are closed, show "Open" text
        }
    }
}
