using System.Collections;
using UnityEngine;

public class ObjectiveMenuController : MonoBehaviour
{
    public GameObject objectiveMenuUI;


    private void Start()
    {
        // Initially show the objective menu
        objectiveMenuUI.SetActive(true);

        // Start coroutine to automatically hide the menu after 5 seconds
        StartCoroutine(HideMenuAfterDelay(5f));
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
            objectiveMenuUI.SetActive(true);
        else if (Time.time > 5f) // To Ensure if more than 5 seconds have passed since the game started
            objectiveMenuUI.SetActive(false);
    }

    private IEnumerator HideMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);


        // This ensures that if the player presses the Tab right before the delay ends, the menu stays visible
        if (!Input.GetKey(KeyCode.Tab)) objectiveMenuUI.SetActive(false);
    }
}