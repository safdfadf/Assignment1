using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftDoorController : MonoBehaviour
{
    public bool isOpen = false;
    public float openSpeed = 5f;
    private Quaternion originalRotation;
    private Quaternion openRotation;

    void Start()
    {
        originalRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
    }

    void Update()
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
