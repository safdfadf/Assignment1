using UnityEngine;

public class GhostAI : MonoBehaviour
{
    public Transform player;              // Player's transform
    public float detectionRadius = 50f;   // Radius to detect the player
    public float moveSpeed = 2f;          // Normal moving speed
    public float chaseSpeed = 5f;         // Speed when chasing the player
    public float roamRadius = 30f;        // Radius within which to roam
    public float killRadius = 2f;         // Radius within which player is killed

    private Vector3 _startPosition;
    private Vector3 _roamPosition;
    private bool _isChasing = false;
    private bool _isReturning = false;    // New state for returning to start position

    private void Start()
    {
        _startPosition = transform.position;
        InvokeRepeating("UpdateRoamPosition", 0f, 5f);  // Update the roam position every 5 seconds
    }

    private void Update()
    {
        if (_isChasing)
        {
            ChasePlayer();
        }
        else if (_isReturning)
        {
            ReturnToStart();
        }
        else
        {
            MoveRandomly();
        }

        DetectPlayer();
        CheckKillPlayer();
    }

    void UpdateRoamPosition()
    {
        if (!_isChasing && !_isReturning) // Only update roam position if not chasing or returning
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float radius = Random.Range(0f, roamRadius);
            _roamPosition = _startPosition + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }
    }

    void MoveRandomly()
    {
        transform.position = Vector3.MoveTowards(transform.position, _roamPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _roamPosition) < 1f)
        {
            UpdateRoamPosition();
        }
    }

    void DetectPlayer()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= detectionRadius)
        {
            _isChasing = true;
            _isReturning = false;
        }
        else
        {
            _isChasing = false;
            if (distance > detectionRadius && !_isReturning)  // Start returning if the player is out of range
            {
                _isReturning = true;
            }
        }
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    void ReturnToStart()
    {
        transform.position = Vector3.MoveTowards(transform.position, _startPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _startPosition) < 1f)
        {
            _isReturning = false;  // Stop returning once the ghost reaches the start position
            UpdateRoamPosition();  // Update roam position to start random movement again
        }
    }

    // Check if the ghost is close enough to kill the player
    void CheckKillPlayer()
    {
        if (Vector3.Distance(player.position, transform.position) <= killRadius)
        {
            PlayerDies();  // Call the method to handle player death
        }
    }

    // Handle the player's death
    void PlayerDies()
    {
        Debug.Log("Player has died."); // Placeholder for player death handling
        // Here you can add any actions that should occur when the player dies
        // For example, reload the scene, display a game over screen, etc.
    }
}
