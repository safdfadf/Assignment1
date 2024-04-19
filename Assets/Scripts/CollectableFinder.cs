using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class CollectableFinder : MonoBehaviour
{
    public TextMeshProUGUI distanceText;  // Change the type from Text to TextMeshProUGUI

    void Update()
    {
        // Find all game objects tagged as "Collectable"
        GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");
        float nearestDistance = Mathf.Infinity;
        GameObject nearestCollectable = null;

        // To iterate through all collectables to find the nearest one
        foreach (GameObject collectable in collectables)
        {
            float distance = Vector3.Distance(transform.position, collectable.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCollectable = collectable;
            }
        }

        //  To update the UI text based on whether a nearest collectable exists
        if (nearestCollectable != null && distanceText != null)
        {
            distanceText.text = "Nearest Collectable: " + nearestDistance.ToString("F2") + " meters away";
        }
        else if (distanceText != null)
        {
            // Change the text if no collectables are found
            distanceText.text = "No more collectables";
        }
    }
}
