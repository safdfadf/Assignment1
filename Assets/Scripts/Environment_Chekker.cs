using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment_Chekker : MonoBehaviour
{
    /// <summary>
    /// You use the name Rayoffset differently in another file. As well, these should all be serialize field variables.
    /// </summary>
    public Vector3 Rayoffset= new Vector3(0,0.15f,0);
    public float Raylength = 0.9f;
    public LayerMask obstacleLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// when I see you writing things like var, I wonder if you know why or took the code from somewhere else.
    /// </summary>
    public void CheckObstacle()
    {
        var rayorigin= transform.position + Rayoffset;
        bool Hitfound= Physics.Raycast(rayorigin,transform.forward,out RaycastHit hitInfo ,Raylength, obstacleLayer);

        Debug.DrawRay(rayorigin, transform.forward * Raylength, (Hitfound) ? Color.red : Color.green);
    }
}
