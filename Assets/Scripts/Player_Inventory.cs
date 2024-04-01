using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Player_Inventory : MonoBehaviour
{
    public int numberofGears { get; private set; }
    private GameManager gameManager;
    public UnityEvent<Player_Inventory> OnGearCollected;
    public int Gearneeded = 4;
    public Light[] lightTurnOn;
    public bool hasReachedSwitch = false;
    public float TimeLimit = 5f;
    public float timer;
    public void GearsCollected()
    {
        numberofGears++;
        OnGearCollected.Invoke(this);
       /* if(numberofGears>= Gearneeded)
        {
        }*/
    }
    private void Start()
    {
        timer = TimeLimit;
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        if(!hasReachedSwitch)
        {
            timer -= Time.deltaTime;
            if(timer <= 0 )
            {
                gameManager.GameOver();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Switch") && numberofGears >= Gearneeded)
        {
            ActivateSwitch();

        }
    }
    void ActivateSwitch()
    {
        //logic for lights 
        Debug.Log("Light On, LevelWon");
        foreach(Light light in lightTurnOn)
        {
            light.enabled = true;
        }
        //LevelWon
        Invoke("CompleteLevel",5);
    }
    void CompleteLevel()
    {
        FindObjectOfType<GameManager>().LevelWon();
    }

    
}