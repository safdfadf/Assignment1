using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float moveSpeed = 3f;
    public float sightRange = 8f;
    private int currentpointIndex = 0;
    private Transform currentTarget;

    private void Start()
    {
        currentTarget = patrolPoints[currentpointIndex];

    }
    private void Update()
    {
        Patrol();
        DeletePlayer();
    }
    void Patrol()
    {
        // Move towards the current target point
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        // If reached the target point, update current target
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentpointIndex = (currentpointIndex + 1) % patrolPoints.Length;
            currentTarget = patrolPoints[currentpointIndex];
        }
    }

    void DeletePlayer()
    {
        // Check if player is within sight range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log("Player Detected!");
               // Destroy(hitCollider.gameObject);
                

            }
        }
    }
}
