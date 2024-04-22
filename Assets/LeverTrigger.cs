using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public GameObject uiText;
    public LeverAnimation leverAnimationScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiText.SetActive(false);
        }
    }

    void Update()
    {
        if (uiText.activeInHierarchy && Input.GetKeyDown(KeyCode.X))
        {
            leverAnimationScript.ToggleLever();
        }
    }
}
