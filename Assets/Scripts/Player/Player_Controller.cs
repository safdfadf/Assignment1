using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private float speed;
    private float HorizontalInput;
    private float VerticalInput;
    private Rigidbody playerRb;
    private float rotationspeed = 5f;
    Vector3 movement;
    private Animator animator;
    [SerializeField] private float jumpspeed = 2f;
    private bool isonground;
    private bool moving = true;
    private float currentspeed;
    [SerializeField] private float speedmultiplier;
    public bool isSprinting = false;
    private bool isclimbing = false;
    [SerializeField] private float climbspeed = 2f;// speed while climbing
    bool isonFurniture;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask FurnitureLayer;
    [SerializeField] private Transform GroundCheckLocation;
    private bool DisableGroundCheck = false;
    [SerializeField] private float checkRadius = 0.1f;


    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleSprint();
        HandleGroundAndFurniture();
        // Visualize SphereCast for debugging
     }

    void HandleMovement()
    {// Player Input
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");

        float movementAmount = Mathf.Clamp01(Mathf.Abs(HorizontalInput) + Mathf.Abs(VerticalInput));
        movement = new Vector3(HorizontalInput, 0, VerticalInput).normalized;

        if (movementAmount > 0 && (isonground || isonFurniture))
        {
            UpdatePlayerVelocity();
            UpdatePlayerRotation();
        }

        animator.SetFloat("movementValue", movementAmount, 0.1f, Time.deltaTime);
        if (!isonground)
        {
            moving = false;// stops movemnt if the player is in air 
        }


    }

    void UpdatePlayerVelocity()// Player Movement
    {
        if (isSprinting)
        {
            currentspeed = speed * speedmultiplier;
        }
        else
        {
            currentspeed = speed;
        }

        playerRb.velocity = movement * currentspeed;
    }

    void UpdatePlayerRotation()// Player Rotation
    {
        Quaternion targetRotation = Quaternion.LookRotation(-movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationspeed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (isonground || isonFurniture)) // checks player Input for jump
        {
            Jump();
            animator.SetBool("IsJumping", true);
        }
    }
    void Jump()// Adds Jump force
    {
        playerRb.AddForce(Vector3.up * jumpspeed, ForceMode.Impulse);
        isonground = false;
        isonFurniture = false;
        DisableGroundCheck = true;
        Invoke("EnableGroundCheck",.1f);
    }

    void EnableGroundCheck()
    {
        DisableGroundCheck = false;
    }

    void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))// checks player Input for sprint
        {
            Debug.Log("Player is Sprinting");
            StartSprinting();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Debug.Log("Stop_Sprinting");
            StopSprinting();
        }
    }

    void StartSprinting()
    {
        isSprinting = true;
        animator.SetBool("IsSprinting", true);
    }

    void StopSprinting()
    {
        isSprinting = false;
        animator.SetBool("IsSprinting", false);
    }
    void HandleGroundAndFurniture()
    { // Ground detection
        if (DisableGroundCheck) return;
        if (Physics.CheckSphere(GroundCheckLocation.position, checkRadius, groundLayer))
        {
            Debug.Log("OnGround");
            isonground = true;
            moving = true;
            animator.SetBool("IsJumping", false);
        }
        else
        {
            isonground = false;
        }

        // Furniture detection
        if (Physics.CheckSphere(GroundCheckLocation.position, checkRadius, FurnitureLayer))
        {
            Debug.Log("OnFurniture");
            isonFurniture = true;
            moving = true;
            animator.SetBool("IsJumping", false);
        }
        else
        {
            isonFurniture = false;
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, checkRadius);
    }
}
