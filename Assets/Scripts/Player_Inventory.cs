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
    private Animator Anim;

    private void Start()
    {
        timer = TimeLimit;
        gameManager = FindObjectOfType<GameManager>();
        Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!hasReachedSwitch)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Anim.SetBool("GameOver", true);
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
        // Logic for lights
        Debug.Log("Light On, Level Won");
        foreach (Light light in lightTurnOn)
        {
            light.enabled = true;
        }

        // Level Won
        Anim.SetBool("Victory", true);
        PlayWinMusic(); // Play win music
        Invoke("CompleteLevel", 5); // Delay the level completion to allow win animation/music
    }

    void CompleteLevel()
    {
        FindObjectOfType<GameManager>().LevelWon();
    }

    public void GearsCollected()
    {
        numberofGears++;
        OnGearCollected.Invoke(this);
    }

    
    private void PlayWinMusic()
    {
        SoundManager.Instance.PlayWinMusic(); 
    }
}
