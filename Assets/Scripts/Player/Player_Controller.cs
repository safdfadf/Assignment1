using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// This name of the file is very poor. I fixed it in my branch and you could have merged changes in. Please review good coding practices in future.
/// </summary>
public class Player_Controller : MonoBehaviour
{
    /// <summary>
    /// These variable names are completely inconsistent and seem to have come about through random choice. You need to be thoughtful about your decisions when making games.
    /// </summary>
    [SerializeField] private float speed;
    private float HorizontalInput;
    private float VerticalInput;
    private Rigidbody playerRb;
    private float rotationspeed = 5f;
    Vector3 movement;
    private Animator animator;
    [SerializeField] private float jumpspeed = 2f;
    private bool isonground;
    public bool moving = false;
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
    public Stamina_System stamina_System;
    private Vector3 initialPosition;
    private Vector3 finalPosition;


    void Start()
    {
        stamina_System = GetComponent<Stamina_System>();
        playerRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isSprinting = false;

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


/// <summary>
/// Why does this not use the new input system? The way you have set up this program, it works on controller for some things and not others.
/// </summary>
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
/// <summary>
/// You should always be collecting input in your update, and then applying changes to your rigid body in the FixedUpdate method.
/// </summary>
    void UpdatePlayerVelocity()// Player Movement
    {
        if (stamina_System.Stamina > 0 && isSprinting)
        {
            float distance = Vector3.Distance(transform.position, finalPosition);
            if (distance > 0.5f)
            {
                stamina_System.Sprint();
                currentspeed = speed * speedmultiplier;
                animator.SetBool("IsSprinting", true);
            }
        }


        else
        {
            currentspeed = speed;
        }

        playerRb.velocity = movement * currentspeed; 
        
    }

    void UpdatePlayerRotation()// Player Rotation
    {
        Quaternion targetRotation = Quaternion.LookRotation(movement);
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
        Invoke("EnableGroundCheck", .1f);
    }

    void EnableGroundCheck()
    {
        DisableGroundCheck = false;
    }
    /// <summary>
    /// Should be handled by input system
    /// </summary>
    void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))// checks player Input for sprint
        {
            initialPosition = transform.position;
            StartSprinting();

        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopSprinting();
        }
    }

    void StartSprinting()
    {
        isSprinting = true;
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
    /// <summary>
    /// This is useful
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, checkRadius);
    }
}