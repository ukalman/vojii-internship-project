using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KatanaStaminaBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private float targetStamina;
    private float currentStamina;

    public void SetMaxStamina(float stamina)
    {
        slider.maxValue = stamina;
        targetStamina = stamina;
        currentStamina = stamina;
        slider.value = stamina;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetStamina(float stamina)
    {
        targetStamina = stamina;
        StopCoroutine("ChangeStamina"); // Stop the current coroutine if it's already running
        StartCoroutine("ChangeStamina"); // Start the coroutine to smoothly transition to the new stamina value
        
    }

    private IEnumerator ChangeStamina()
    {
        // Continue to update the stamina until the current stamina is approximately equal to the target stamina
        while (!Mathf.Approximately(currentStamina, targetStamina))
        {
            // Lerp the current stamina value towards the target stamina value
            currentStamina = Mathf.Lerp(currentStamina, targetStamina, Time.deltaTime * 10); // Adjust the multiplier to control the speed of the change
            slider.value = currentStamina;
            fill.color = gradient.Evaluate(slider.normalizedValue);
            yield return null; // Wait until the next frame to continue
        }
    }
}
