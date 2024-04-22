using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class CustomDoorController : MonoBehaviour
{
    public Transform doorTransform;
    public Vector3 openRotation;
    public float openSpeed = 2.0f;
    private bool isDoorOpen = false;
    public TMP_Text interactionText;  

    void Start()
    {
        interactionText.gameObject.SetActive(false);  // Initially hide the text
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interactionText.gameObject.activeInHierarchy)
        {
            isDoorOpen = true;  // Toggle the door state to open
        }

        if (isDoorOpen)
        {
            // Smoothly rotate the door to the open position using Slerp
            Quaternion targetRotation = Quaternion.Euler(openRotation);
            doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionText.gameObject.SetActive(true);  // Show the prompt when the player is near
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionText.gameObject.SetActive(false);  // Hide the prompt when the player moves away
        }
    }
}
