using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ParkourController : MonoBehaviour
{
    EnvironmentChecker environmentChecker;

    private void Awake()
    {
        environmentChecker = GetComponent<EnvironmentChecker>();
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        environmentChecker.CheckObstacle();
    }
}