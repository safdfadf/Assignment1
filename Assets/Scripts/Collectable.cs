using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private bool isCollected = false;
    private Vector3 initialScale;  // To store the initial scale
    private Collider collectableCollider;
    private Renderer collectableRenderer;

    public float respawnDelay = 40;  // Time before starting the respawn process
    public float reappearanceDuration = 5.0f;  // Duration for the collectable to fully reappear

    void Awake()
    {
        collectableCollider = GetComponent<Collider>();
        collectableRenderer = GetComponent<Renderer>();
        initialScale = transform.localScale;
        collectableCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Inventory>() && !isCollected)
        {
            isCollected = true;
            other.GetComponent<Player_Inventory>().GearsCollected();
            StartCoroutine(DisappearAndRespawn());
        }
    }
    /// <summary>
    /// Why do these respawn?! This means that the level can never be beaten if the objective was to have them be collected 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisappearAndRespawn()
    {
        collectableCollider.enabled = false;
        // Gradually make the item disappear by scaling down to zero
        for (float t = 0; t < 1; t += Time.deltaTime / 1) // Takes 1 second to disappear
        {
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            yield return null;
        }
        transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(respawnDelay);

        // Gradually make the item reappear
        float timer = 0;
        while (timer < reappearanceDuration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, timer / reappearanceDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale;
        collectableCollider.enabled = true;
        isCollected = false;
    }
}
