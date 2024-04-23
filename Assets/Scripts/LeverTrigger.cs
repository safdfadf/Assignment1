using System.Collections;
using UnityEngine;
using TMPro;

public class LeverTrigger : MonoBehaviour
{
    public GameObject uiText;
    public TextMeshProUGUI feedbackText;
    public LeverAnimation leverAnimationScript;
    public Player_Inventory playerInventory;
    public bool isFlipped = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiText.SetActive(true);
            playerInventory = other.GetComponent<Player_Inventory>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiText.SetActive(false);
            feedbackText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (uiText.activeInHierarchy && Input.GetKeyDown(KeyCode.X))
        {
            TryActivateLever();
        }
    }

    private void TryActivateLever()
    {
        if (playerInventory != null && playerInventory.numberofGears >= 5)
        {
            leverAnimationScript.ToggleLever();
            isFlipped = !isFlipped;
            playerInventory.UseGears(5);
            feedbackText.gameObject.SetActive(false);
        }
        else
        {
            feedbackText.text = "Collect at least 5 collectables to flip this lever.";
            feedbackText.gameObject.SetActive(true);
        }
    }
}
