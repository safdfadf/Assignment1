using UnityEngine;
using UnityEngine.UI;

public class CollectableFinder : MonoBehaviour
{
    public Text distanceText;

    private void Update()
    {
        // Find all game objects tagged as "Collectable"
        var collectables = GameObject.FindGameObjectsWithTag("Collectable");
        var nearestDistance = Mathf.Infinity;
        GameObject nearestCollectable = null;

        // To iterate through all collectables to find the nearest one
        foreach (var collectable in collectables)
        {
            var distance = Vector3.Distance(transform.position, collectable.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCollectable = collectable;
            }
        }

        //  To update the UI text based on whether a nearest collectable exists
        if (nearestCollectable != null && distanceText != null)
            distanceText.text = "Nearest Collectable: " + nearestDistance.ToString("F2") + " meters away";
        else if (distanceText != null)
            // Change the text if no collectables are found
            distanceText.text = "No more collectables";
    }
}