using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player_Inventory : MonoBehaviour
{
    public int numberofGears { get; private set; }  // Number of gears currently held by the player
    private GameManager gameManager;
    public UnityEvent<Player_Inventory> OnGearCollected;
    public UnityEvent OnLeverActivated;  // Event triggered when a lever is activated
    [SerializeField] int gearsNeeded = 5;  // Gears required to activate a lever or switch
    public Light[] lightTurnOn;            // Array of lights that can be turned on
    public bool hasReachedSwitch = false;
    [SerializeField] float timeLimit = 60f; // Time limit to reach a switch
    public float timer;
    private Animator Anim;

    void Start()
    {
        timer = timeLimit;
        gameManager = FindObjectOfType<GameManager>();
        Anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!hasReachedSwitch)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                GameOver();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Switch") && numberofGears >= gearsNeeded)
        {
            ActivateSwitch();
        }
    }

    public void TryActivateLever()
    {
        if (numberofGears >= gearsNeeded)
        {
            UseGears(gearsNeeded);  // Deduct the necessary number of gears
            OnLeverActivated.Invoke();  // Signal that the lever has been activated
            ActivateSwitch();  // Additional functionality tied to switch activation
        }
        else
        {
            Debug.Log("Not enough gears collected to activate the lever.");
        }
    }

    public void ActivateSwitch()
    {
        foreach (Light light in lightTurnOn)
        {
            light.enabled = true;  // Turn each light on
        }
        Anim.SetBool("Victory", true);
        PlayWinMusic();
        Invoke("CompleteLevel", 5);  // Allow some time for animations and music to play
    }

    public void GearsCollected()
    {
        numberofGears++;
        OnGearCollected.Invoke(this);
    }

    public void UseGears(int amount)
    {
        numberofGears -= amount;
        if (numberofGears < 0)
            numberofGears = 0;  // Prevent gears from going negative
    }

    private void GameOver()
    {
        Anim.SetBool("GameOver", true);
        gameManager.GameOver();
    }

    private void PlayWinMusic()
    {
        SoundManager.Instance.PlayWinMusic();  // Play victory music
    }

    private void CompleteLevel()
    {
        gameManager.LevelWon();  // Notify the game manager that the level has been completed
    }
}
