using UnityEngine;

public class EnvironmentChecker : MonoBehaviour
{
    [SerializeField]
    Vector3 rayOffset = new(0, 0.15f, 0);
    [SerializeField]
    float rayLength = 0.9f;

    public LayerMask obstacleLayer;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void CheckObstacle()
    {
        var rayOrigin = transform.position + rayOffset;
        var hitFound = Physics.Raycast(rayOrigin, transform.forward, out var hitInfo, rayLength, obstacleLayer);

        Debug.DrawRay(rayOrigin, transform.forward * rayLength, hitFound ? Color.red : Color.green);
    }
}