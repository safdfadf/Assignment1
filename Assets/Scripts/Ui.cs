using TMPro;
using UnityEngine;

public class Ui : MonoBehaviour
{
    private TextMeshProUGUI diamondText;

    // Start is called before the first frame update
    private void Start()
    {
        diamondText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateGear(PlayerInventory inventory)
    {
        diamondText.text = inventory.numberofGears.ToString();
    }
}