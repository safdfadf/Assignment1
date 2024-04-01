using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ObjectiveMenuController : MonoBehaviour
{
    public GameObject objectiveMenuUI; // Assign in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Initially show the objective menu
        objectiveMenuUI.SetActive(true);

        // Start coroutine to automatically hide the menu after 5 seconds
        StartCoroutine(HideMenuAfterDelay(5f));
    }

    void Update()
    {
        // After the initial 5 seconds, check if the Tab key is being held down to show/hide the objective menu
        // Note: We're checking if the coroutine is not running to avoid conflicts
        if (Input.GetKey(KeyCode.Tab))
        {
            objectiveMenuUI.SetActive(true);
        }
        else if (Time.time > 5f) // Check if more than 5 seconds have passed since the game started
        {
            objectiveMenuUI.SetActive(false);
        }
    }

    private IEnumerator HideMenuAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Only hide the menu if the Tab key is not currently being pressed
        // This ensures that if the player presses the Tab right before the delay ends, the menu stays visible
        if (!Input.GetKey(KeyCode.Tab))
        {
            objectiveMenuUI.SetActive(false);
        }
    }
}
