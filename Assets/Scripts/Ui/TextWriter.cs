using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextWriter : MonoBehaviour
{

    public GameObject textBox; // Reference to the background text box
    public TMP_Text textMeshProText;
    public float animationSpeed = 100.0f; // Characters per second
    public float uiDelay = 2.0f;

    private string fullText;
    private float startTime;
    private int charactersToShow;

    IEnumerator Start()
    {
        fullText = textMeshProText.text;
        textMeshProText.text = "";

        charactersToShow = 0;
        startTime = Time.time;

        // Calculate the duration based on the desired animation speed
        float duration = fullText.Length / animationSpeed;

        // Show characters gradually
        while (charactersToShow < fullText.Length)
        {
            // Calculate how many characters to show at this moment
            float elapsedTime = Time.time - startTime;
            charactersToShow = Mathf.FloorToInt(elapsedTime * animationSpeed);

            // Ensure we don't exceed the length of the full text
            charactersToShow = Mathf.Clamp(charactersToShow, 0, fullText.Length);

            // Update the displayed text
            textMeshProText.text = fullText.Substring(0, charactersToShow);

            yield return null; // Wait for the next frame
        }

        yield return new WaitForSeconds(uiDelay);

        // Deactivate the text box and text
        textBox.SetActive(false);
        textMeshProText.text = ""; // Hide the text
    }
}

