using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player_Inventory inventory = other.GetComponent<Player_Inventory>();
        if(inventory != null )
        {
            inventory.GearsCollected();
            gameObject.SetActive(false);
        }
    }
}