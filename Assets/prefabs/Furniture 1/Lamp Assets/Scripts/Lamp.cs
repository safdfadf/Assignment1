using UnityEngine;

public class Lamp : MonoBehaviour
{
    [HideInInspector] public GameObject LampLight;

    [HideInInspector] public GameObject DomeOff;

    [HideInInspector] public GameObject DomeOn;

    public bool TurnOn;


    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (TurnOn)
        {
            LampLight.SetActive(true);
            DomeOff.SetActive(false);
            DomeOn.SetActive(true);
        }

        if (TurnOn == false)
        {
            LampLight.SetActive(false);
            DomeOff.SetActive(true);
            DomeOn.SetActive(false);
        }
    }
}