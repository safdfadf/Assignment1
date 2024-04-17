using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.GearsCollected();
            gameObject.SetActive(false);
        }
    }
}