using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer_Script : MonoBehaviour
{
    private Player_Inventory playerInventory;
    private TextMeshProUGUI TimerText;
    // Start is called before the first frame update
    void Start()
    {
        playerInventory = FindObjectOfType<Player_Inventory>(); 
        TimerText = GetComponent<TextMeshProUGUI>();
       
    }

    // Update is called once per frame
    void Update()
    {

        TimerText.text = "Time Left =" + Mathf.RoundToInt(playerInventory.timer).ToString();
        
        
    }
}
