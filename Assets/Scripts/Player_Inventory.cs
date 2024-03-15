using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Player_Inventory : MonoBehaviour
{
    public int numberofGears { get; private set; }
    public UnityEvent<Player_Inventory> OnGearCollected;
    public void GearsCollected()
    {
        numberofGears++;
        OnGearCollected.Invoke(this);
    }
}