using UnityEngine;

public class RightDoorController : MonoBehaviour
{
    public bool isOpen;
    public float openSpeed = 5f;
    private Quaternion openRotation;
    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
        // Adjust the opening direction by changing the sign for the y-axis rotation
        openRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 90, transform.eulerAngles.z);
    }

    private void Update()
    {
        if (isOpen)
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * openSpeed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}