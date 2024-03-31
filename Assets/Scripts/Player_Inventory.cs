using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Player_Inventory : MonoBehaviour
{
    public int numberofGears { get; private set; }
    public UnityEvent<Player_Inventory> OnGearCollected;
    public int Gearneeded = 4;
    public Light[] lightTurnOn;
    public void GearsCollected()
    {
        numberofGears++;
        OnGearCollected.Invoke(this);
       /* if(numberofGears>= Gearneeded)
        {
        }*/
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Switch") && numberofGears >= Gearneeded)
        {
            ActivateSwitch();

        }
    }
    void ActivateSwitch()
    {
        //logic for lights 
        Debug.Log("Light On, Game over");
        foreach(Light light in lightTurnOn)
        {
            light.enabled = true;
        }
        //Load Next Level
        FindObjectOfType<GameManager>().NextLevel();
    }

    
}