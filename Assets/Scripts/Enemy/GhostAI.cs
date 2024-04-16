using UnityEngine;

public class GhostAI : MonoBehaviour
{
    public Transform player;              // Player's transform
    public float detectionRadius = 50f;   // Radius to detect the player
    public float moveSpeed = 2f;          // Normal moving speed
    public float chaseSpeed = 5f;         // Speed when chasing the player
    public float roamRadius = 30f;        // Radius within which to roam
    public float killRadius = 2f;         // Radius within which player is killed
    public GameManager gameManager;       // Reference to the GameManager

    private Vector3 _startPosition;
    private Vector3 _roamPosition;
    private bool _isChasing = false;
    private bool _isReturning = false;    // Indicates if ghost is returning to start position

    // Initialize ghost's position and update roam position periodically
    private void Start()
    {
        _startPosition = transform.position;
        InvokeRepeating("UpdateRoamPosition", 0f, 5f);
    }

    // Update ghost's behavior each frame
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

    // Calculate a new position to roam to within the designated radius
    void UpdateRoamPosition()
    {
        if (!_isChasing && !_isReturning)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float radius = Random.Range(0f, roamRadius);
            _roamPosition = _startPosition + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }
    }

    // Move towards a random position
    void MoveRandomly()
    {
        transform.position = Vector3.MoveTowards(transform.position, _roamPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _roamPosition) < 1f)
        {
            UpdateRoamPosition();
        }
    }

    // Detect if the player is within the detection radius
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
            if (!_isReturning)
            {
                _isReturning = true;
            }
        }
    }

    // Chase the player
    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    // Return to the initial start position
    void ReturnToStart()
    {
        transform.position = Vector3.MoveTowards(transform.position, _startPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _startPosition) < 1f)
        {
            _isReturning = false;
            UpdateRoamPosition();
        }
    }

    // Check if the ghost is close enough to kill the player
    void CheckKillPlayer()
    {
        if (Vector3.Distance(player.position, transform.position) <= killRadius)
        {
            PlayerDies();
        }
    }

    // Handle the player's death and trigger game over
    void PlayerDies()
    {
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Debug.LogError("GameManager not set on " + gameObject.name);
        }
    }
}
