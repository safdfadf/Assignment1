using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableFinder : MonoBehaviour
{
    public Text distanceText; // UI Text element to display distance

    void Update()
    {
        GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");
        float nearestDistance = Mathf.Infinity;
        GameObject nearestCollectable = null;

        foreach (GameObject collectable in collectables)
        {
            float distance = Vector3.Distance(transform.position, collectable.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCollectable = collectable;
            }
        }

        if (nearestCollectable != null && distanceText != null)
        {
            distanceText.text = "Nearest Collectable: " + nearestDistance.ToString("F2") + " meters away";
        }
    }
}
