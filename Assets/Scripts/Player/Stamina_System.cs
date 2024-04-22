using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // Required for dealing with UI

public class Stamina_System : MonoBehaviour
{
    public Player_Controller controller;
    public Slider StaminaSlider;  // Using Slider instead of Image
    public float Stamina, MaxStamina;
    public float RunCost;
    public float ChargeRate;
    private Coroutine recharge;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Player_Controller>();
        InitializeStamina();
    }

    // Initialize Stamina values and UI
    void InitializeStamina()
    {
        Stamina = MaxStamina;  // Start with full stamina
        StaminaSlider.maxValue = MaxStamina;  // Set the Slider's max value
        StaminaSlider.value = Stamina;  // Set the Slider's current value
    }

    // Update is called once per frame
    void Update()
    {
        // Optional: Implement logic to handle when the player is sprinting
        // This could check for player input and call Sprint() if appropriate
    }

    public void Sprint()
    {
        Stamina -= RunCost * Time.deltaTime;  // Reduce stamina by the running cost
        if (Stamina < 0) Stamina = 0;  // Ensure stamina doesn't go negative
        StaminaSlider.value = Stamina;  // Update the Slider's value

        // Stop any ongoing stamina recharge and start a new one
        if (recharge != null)
        {
            StopCoroutine(recharge);
        }
        recharge = StartCoroutine(RechargeStamina());
    }

    public IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);  // Wait for 1 second before starting to recharge

        // Gradually increase stamina and update the UI
        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate * Time.deltaTime;  // Increase Stamina
            if (Stamina > MaxStamina) Stamina = MaxStamina;  // Cap Stamina at its max value
            StaminaSlider.value = Stamina;  // Update the Slider's value
            yield return new WaitForSeconds(0.1f);  // Wait for 0.1 seconds before adding more stamina
        }
    }
}
