using System.Collections;
using System.Collections.Generic;
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
    private float jumpspeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        float movementAmount =Mathf.Clamp01( Mathf.Abs(HorizontalInput) + Mathf.Abs(VerticalInput));
        movement = new Vector3(HorizontalInput, 0, VerticalInput).normalized;
        
        if (movementAmount>0)
        {
            playerRb.velocity = movement * speed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(-movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationspeed * Time.deltaTime);
        }
        Anim.SetFloat("movementValue",movementAmount,0.1f,Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))    
        {
            playerRb.AddForce(Vector3.up * jumpspeed,ForceMode.Impulse);
        }
    }
}
    