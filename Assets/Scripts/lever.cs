using UnityEngine;
/// <summary>
/// Why did you end up using an interface here? Just seems like you took code from somewhere
/// </summary>
public interface ILever
{
    void ActivateLever();
}

public class Lever : MonoBehaviour, ILever
{
    [SerializeField]
    private bool isLeverActive = false;
    private Quaternion downRotation;
    private Quaternion upRotation;
    [SerializeField]
    private float speed = 2.0f;

    [Header("Feedback")]
    public AudioSource leverSound;  // Drag an AudioSource component here via the Inspector

    void Start()
    {
        downRotation = transform.rotation;
        upRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 180));  // Assuming the lever flips along the Z axis
    }

    void Update()
    {
        // Only update rotation if the lever is active or not in its final position
        if (isLeverActive && transform.rotation != upRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, upRotation, Time.deltaTime * speed);
        }
        else if (!isLeverActive && transform.rotation != downRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, Time.deltaTime * speed);
        }
    }

    public void ActivateLever()
    {
        isLeverActive = !isLeverActive;
        if (leverSound != null)
            leverSound.Play();  // Play sound on activation
    }
}
