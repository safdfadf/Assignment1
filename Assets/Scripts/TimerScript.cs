using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private PlayerInventory playerInventory;

    private TextMeshProUGUI TimerText;

    // Start is called before the first frame update
    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        TimerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        TimerText.text = "Time Left =" + Mathf.RoundToInt(playerInventory.timer);
    }
}