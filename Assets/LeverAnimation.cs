using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimation : MonoBehaviour
{
    private bool leverActive = false;
    private Quaternion downRotation;
    private Quaternion upRotation;
    public float speed = 2f;

    void Start()
    {
        downRotation = transform.rotation;
        upRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, -10));
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, leverActive ? upRotation : downRotation, Time.deltaTime * speed);
    }

    public void ToggleLever()
    {
        leverActive = !leverActive;
    }
}
