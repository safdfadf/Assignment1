using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider staminaSlider;
    private float maxStamina = 1.0f;
    private float currentStamina;
    private float staminaDecayRate = 0.2f; // Stamina depletes fully in 5 seconds
    private float staminaRegenRate = 0.05f; // Stamina regenerates fully in 20 seconds
    private bool isUsingStamina = false;

    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.value = currentStamina;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) // Assuming space bar usage depletes stamina
        {
            UseStamina();
        }
        else
        {
            RegenerateStamina();
        }

        staminaSlider.value = currentStamina;
    }

    void UseStamina()
    {
        if (currentStamina > 0)
        {
            currentStamina -= Time.deltaTime * staminaDecayRate;
            currentStamina = Mathf.Max(currentStamina, 0);
            isUsingStamina = true;
        }
    }

    void RegenerateStamina()
    {
        if (!isUsingStamina && currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime * staminaRegenRate;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
        isUsingStamina = false;
    }
}