using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    public LeverTrigger[] levers;  // Array of all levers that control the door
    public Text interactionText;
    private bool playerIsNear = false;

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            if (AreAllLeversFlipped())
            {
                // Code to open the door
                StartCoroutine(UpdateTextNextFrame());
            }
            else
            {
                interactionText.text = "You need to flip all levers to open the doors";  // Show message if not all levers are flipped
            }
        }
    }

    IEnumerator UpdateTextNextFrame()
    {
        yield return null;
        interactionText.text = AreAllLeversFlipped() ? "[E] Close Door" : "[E] Open Door";
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
        interactionText.gameObject.SetActive(isNear);  // Show or hide the UI text based on proximity
    }

    private bool AreAllLeversFlipped()
    {
        foreach (var lever in levers)
        {
            if (!lever.isFlipped) return false;  // If any lever is not flipped, return false
        }
        return true;  // All levers are flipped
    }
}
