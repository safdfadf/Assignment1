using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public UnityEvent<PlayerInventory> OnGearCollected;

    [SerializeField] private int gearsNeeded = 4;

    public Light[] lightTurnOn;
    public bool hasReachedSwitch;
    [SerializeField] private float timeLimit = 5f;
    public float timer;
    private Animator animator;
    private GameManager gameManager;
    public int numberofGears { get; private set; }

    private void Start()
    {
        timer = timeLimit;
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        var collectables = FindObjectsOfType<Collectable>();
        gearsNeeded = collectables.Length;
    }

    private void Update()
    {
        if (!hasReachedSwitch)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                animator.SetBool("GameOver", true);
                gameManager.GameOver();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Switch") && numberofGears >= gearsNeeded) ActivateSwitch();
    }

    private void ActivateSwitch()
    {
        // Logic for lights
        Debug.Log("Light On, Level Won");
        foreach (var light in lightTurnOn) light.enabled = true;

        // Level Won
        animator.SetBool("Victory", true);
        PlayWinMusic(); // Play win music
        Invoke("CompleteLevel", 5); // Delay the level completion to allow win animation/music
    }

    private void CompleteLevel()
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