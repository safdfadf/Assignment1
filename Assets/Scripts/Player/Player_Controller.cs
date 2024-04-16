using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float HorizontalInput;
    [SerializeField] 
    private float VerticalInput;
    private Rigidbody playerRb;
    private float rotationspeed = 5f;
    Vector3 movement;
    private bool Isonground;
    private Animator Anim;
    [SerializeField]
    private float jumpspeed = 2f;
    private bool isonground;
    private bool moving= true;
    private float currentspeed;
    [SerializeField]
    private float speedmultiplier;
    public bool isSprinting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleSprint();
    }

    void HandleMovement()
    {// Player Input
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");

        float movementAmount = Mathf.Clamp01(Mathf.Abs(HorizontalInput) + Mathf.Abs(VerticalInput));
        movement = new Vector3(HorizontalInput, 0, VerticalInput).normalized;

        if (movementAmount > 0 && moving)
        {
            UpdatePlayerVelocity();
            UpdatePlayerRotation();
        }

        Anim.SetFloat("movementValue", movementAmount, 0.1f, Time.deltaTime);
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
       
        playerRb.velocity = movement * currentspeed * Time.deltaTime;
    }

    void UpdatePlayerRotation()// Player Rotation
    {
        Quaternion targetRotation = Quaternion.LookRotation(-movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationspeed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isonground) // checks player Input for jump
        {
            Jump();
        }

        if (!isonground)
        {
            moving = false;// stops movemnt if the player is in air 
        }
    }

    void Jump()// Adds Jump force
    {
        playerRb.AddForce(Vector3.up * jumpspeed, ForceMode.Impulse);
        isonground = false;
        Anim.SetBool("IsJumping", true);
    }

    void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isonground)// checks player Input for sprint
        {
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
        Anim.SetBool("IsSprinting", true);
    }

    void StopSprinting()
    {
        isSprinting = false;
        Anim.SetBool("IsSprinting", false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // checks if the player is on ground
        {
            isonground = true;
            moving = true;
            Anim.SetBool("IsJumping", false);
        }
    }
   
}
    