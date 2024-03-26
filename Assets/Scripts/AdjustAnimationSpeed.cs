using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustAnimationSpeed : MonoBehaviour
{
    public Animator animator; 

    public void SetAnimationSpeed(float speed)
    {
        animator.speed = 1.0f; 
    }
}
