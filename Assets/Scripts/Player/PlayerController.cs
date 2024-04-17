using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private float horizontalInput;

    [SerializeField] private float verticalInput;

    [SerializeField] private float jumpSpeed = 2f;

    [SerializeField] private float speedMultiplier;

    public bool isSprinting;
    private readonly float rotationSpeed = 5f;
    private Animator animator;
    private float currentSpeed;
    private bool isGrounded;
    private Vector3 movement;
    private bool moving = true;
    private Rigidbody playerRb;

    // Start is called before the first frame update
    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleSprint();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // checks if the player is on ground
        {
            isGrounded = true;
            moving = true;
            animator.SetBool("IsJumping", false);
        }
    }

    private void HandleMovement()
    {
        // Player Input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        var movementAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        movement = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (movementAmount > 0 && moving)
        {
            UpdatePlayerVelocity();
            UpdatePlayerRotation();
        }

        animator.SetFloat("movementValue", movementAmount, 0.1f, Time.deltaTime);
    }

    private void UpdatePlayerVelocity() // Player Movement
    {
        if (isSprinting)
            currentSpeed = speed * speedMultiplier;
        else
            currentSpeed = speed;

        playerRb.velocity = movement * currentSpeed;
    }

    private void UpdatePlayerRotation() // Player Rotation
    {
        var targetRotation = Quaternion.LookRotation(-movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // checks player Input for jump
            Jump();

        if (!isGrounded) moving = false; // stops movemnt if the player is in air 
    }

    private void Jump() // Adds Jump force
    {
        playerRb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        isGrounded = false;
        animator.SetBool("IsJumping", true);
    }

    private void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded) // checks player Input for sprint
            StartSprinting();

        if (Input.GetKeyUp(KeyCode.LeftShift)) StopSprinting();
    }

    private void StartSprinting()
    {
        isSprinting = true;
        animator.SetBool("IsSprinting", true);
    }

    private void StopSprinting()
    {
        isSprinting = false;
        animator.SetBool("IsSprinting", false);
    }
}