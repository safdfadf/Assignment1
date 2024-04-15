using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBehavior : MonoBehaviour
{
    public float RoamDistance = 10f;
    public float MoveSpeed = 2f;
    public float ChaseSpeed = 5f;
    public Transform PlayerTarget;
    public float DetectionRadius = 5f;

    private bool _isChasing = false;
    private Vector3 _startPosition;
    private Vector3 _roamPosition;

    private void Start()
    {
        _startPosition = transform.position;
        InitializeRoamPosition();
    }

    private void Update()
    {
        if (!_isChasing)
        {
            MoveHorizontally();
        }
        else
        {
            ChasePlayer();
        }

        DetectPlayerWithSphereCast();
    }

    private void InitializeRoamPosition()
    {
        _roamPosition = _startPosition + new Vector3(RoamDistance, 0, 0);
    }

    private void MoveHorizontally()
    {
        if (!_isChasing)
        {
            transform.position = Vector3.Lerp(_startPosition, _roamPosition, Mathf.PingPong(Time.time * MoveSpeed, 1));
        }
    }

    private void DetectPlayerWithSphereCast()
    {
        RaycastHit hit;
        if (!_isChasing && Physics.SphereCast(transform.position, DetectionRadius, Vector3.forward, out hit, DetectionRadius))
        {
            Debug.Log("SphereCast hit: " + hit.collider.name); // Debug line
            if (hit.collider.transform == PlayerTarget)
            {
                _isChasing = true;
                Debug.Log("Chasing started"); // Debug line
            }
        }
    }

    private void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, PlayerTarget.position, ChaseSpeed * Time.deltaTime);
        Debug.Log("Moving towards player"); // Debug line
    }
}