using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player_Inventory : MonoBehaviour
{
    public int numberofGears { get; private set; }
    private GameManager gameManager;
    public UnityEvent<Player_Inventory> OnGearCollected;
    public UnityEvent OnLeverActivated;  // Event for activating a lever
    [SerializeField] int gearsNeeded = 5;  // Set this to 5 as required for each lever
    public Light[] lightTurnOn;            // Lights to turn on when switches are activated
    public bool hasReachedSwitch = false;
    [SerializeField] float timeLimit = 60f;
    public float timer;
    private Animator Anim;

    void Start()
    {
        timer = timeLimit;
        gameManager = FindObjectOfType<GameManager>();
        Anim = GetComponent<Animator>();
        Collectable[] collectables = FindObjectsOfType<Collectable>();
        // Optionally adjust gearsNeeded based on the number of collectables if needed
        // gearsNeeded = collectables.Length;
    }

    void Update()
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
        // Check if the collision is with a switch and if gears are sufficient
        if (collision.gameObject.CompareTag("Switch") && numberofGears >= gearsNeeded)
        {
            ActivateSwitch();
            numberofGears -= gearsNeeded;  // Consume gears when a switch is activated
        }
    }

    public void TryActivateLever()
    {
        if (numberofGears >= gearsNeeded)
        {
            numberofGears -= gearsNeeded;  // Use up the gears
            OnLeverActivated.Invoke();  // Trigger the lever activation event
            ActivateSwitch();  // Use the same logic for levers as for switches
        }
        else
        {
            Debug.Log("Not enough gears collected to activate the lever");
        }
    }

    void ActivateSwitch()
    {
        Debug.Log("Switch activated, lights on or level won");
        foreach (Light light in lightTurnOn)
        {
            light.enabled = true;
        }
        Anim.SetBool("Victory", true);
        PlayWinMusic();  // Play win music
        Invoke("CompleteLevel", 5);  // Delay the level completion to allow win animation/music
    }

    void CompleteLevel()
    {
        gameManager.LevelWon();
    }

    public void GearsCollected()
    {
        numberofGears++;
        OnGearCollected.Invoke(this);
    }

    private void PlayWinMusic()
    {
        // Assuming there's a sound manager to handle music
        SoundManager.Instance.PlayWinMusic();
    }
}
