using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    public LeverTrigger lever; // Single lever that controls the door
    public Text interactionText;
    public Light doorLight;

    private bool playerIsNear = false;
    private bool doorIsOpen = false;

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            if (lever.isFlipped) // Check if the lever is flipped
            {
                doorIsOpen = !doorIsOpen; // Toggle the door state
                if (doorIsOpen)
                {
                    CompleteLevel();
                    doorLight.enabled = true; // Turn on the light when the door opens
                }
                else
                {
                    doorLight.enabled = false; // Turn off the light when the door closes
                }
                StartCoroutine(UpdateTextNextFrame());
            }
            else
            {
                interactionText.text = "You need to flip the lever to turn on the light"; // Notify player
            }
        }
    }

    IEnumerator UpdateTextNextFrame()
    {
        yield return null;
        // Change the text to indicate light control instead of door state
        interactionText.text = doorIsOpen ? "[E] TURN OFF LIGHT" : "[E] TURN ON LIGHT";
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
        interactionText.gameObject.SetActive(isNear); // Show/hide UI text based on player proximity
    }

    private void CompleteLevel()
    {
        Debug.Log("Level Completed!"); // Log level completion
    }
}
