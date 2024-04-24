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
    /// <summary>
    /// Your input code and checks should be in one script and then directed to other scripts; the way you have done it means you have E being checked for in a bunch of different places, which could lead to issues.
    /// </summary>
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
    /// <summary>
    /// good
    /// </summary>
    /// <param name="isNear"></param>
    private void TogglePlayerProximity(bool isNear)
    {
        playerIsNear = isNear;
        interactionText.gameObject.SetActive(isNear); // Show/hide UI text based on player proximity
    }
    /// <summary>
    /// what is this even for?
    /// </summary>
    private void CompleteLevel()
    {
        Debug.Log("Level Completed!"); // Log level completion
    }
}
