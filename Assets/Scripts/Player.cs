using System.IO;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int currentCoin = 0; // Track the number of coins collected
    public Text coinText; // UI Text to display the number of coins collected
    public int maxHealth = 100;
    public Text health;
    private float movement;
    public float moveSpeed = 5f; // Speed at which the player moves
    private bool facingRight = true; // Track the direction the player is facing
    public Rigidbody2D rb; // Reference to the Rigidbody2D component
    public float jumpHeight = 5f; // Height of the jump
    public bool isGround = true; // Track if the player is on the ground
    public Animator animator; // Reference to the Animator component for animations
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask enemyLayer; // Layer mask to identify enemies
    private AudioSource audioSource;
    [SerializeField] private AudioClip swordSlashSound;
    [SerializeField] private float swordSlashVolume = 0.1f; // Volume control for sword slash
    [SerializeField] private AudioClip coinCollectSound;
    [SerializeField] private AudioClip jumpSound; // Jump sound effect
    [SerializeField] private AudioClip[] footstepSounds; // Array for dirt footstep sounds
    
    private float footstepTimer = 0f; // Timer for footstep sounds
    private float footstepInterval = 0.3f; // Time between footstep sounds

    void Start()
    {
        UpdateCoinUI(); // Initialize the coin UI

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (maxHealth <= 0)
        {
            Die(); // Call the Die method if health is zero or below
        }

        health.text = maxHealth.ToString();

        // Move left with LeftArrow, right with RightArrow
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement = -1f;
            if (facingRight)
            {
                transform.eulerAngles = new Vector3(0f, -180f, 0f);
                facingRight = false;
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movement = 1f;
            if (!facingRight)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                facingRight = true;
            }
        }
        else
        {
            movement = 0f;
        }

        if (Input.GetKey(KeyCode.X) && isGround)
        {
            Jump();
            isGround = false;
            animator.SetBool("Jump", true);
        }

        if (Mathf.Abs(movement) > .1f)
        {
            animator.SetFloat("Run", 1f);
            
            // Add footstep sounds when running on the ground
            if (isGround)
            {
                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    PlayRandomFootstepSound();
                    footstepTimer = 0f;
                }
            }
        }
        else
        {
            animator.SetFloat("Run", 0f);
            footstepTimer = 0; // Reset timer when not moving
        }

        // Attack with "C" key
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void FixedUpdate()
    {
        // Move the player based on the input
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * moveSpeed;
    }

    void Jump()
    {
        // Play jump sound
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
        
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    void UpdateCoinUI()
    {
        coinText.text = currentCoin.ToString(); // Update the coin text UI
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true; // Set isGround to true when the player collides with the grounds
            animator.SetBool("Jump", false);
        }
    }

    public void Attack()
    {
        // Play sword slash sound
        if (swordSlashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(swordSlashSound, swordSlashVolume); // Use the adjustable volume variable
        }

        Collider2D collInfo = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        if (collInfo)
        {
            PatrolEnemy enemy = collInfo.GetComponent<PatrolEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(5); // Call the TakeDamage method on the enemy with a damage value
            }
        }
    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage; // Reduce the player's health by the damage amount
        if (maxHealth <= 0)
        {
            Die(); // Call the Die method if health is zero or below
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            // Check if the coin has already been collected
            if (other.gameObject.activeInHierarchy)
            {
                // Play coin collect sound
                if (coinCollectSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(coinCollectSound);
                }

                currentCoin++; // Increment the coin count
                UpdateCoinUI(); // Update the UI immediately
                other
                    .gameObject.transform.GetChild(0)
                    .GetComponent<Animator>()
                    .SetTrigger("Collected"); // Trigger the collect animation
                // other.gameObject.SetActive(false); // Immediately deactivate to prevent double collection
                Destroy(other.gameObject, 1f); // Destroy the coin game object
            }
        }

        if (other.gameObject.tag == "Victory")
        {
            Debug.Log("Victory! You have collected all coins.");
            FindFirstObjectByType<SceneManagement>().LoadLevel(); // Load the next level
        }
    }

    public void Die()
    {
        // Handle player death (e.g., play death animation, respawn, etc.)
        Debug.Log("Player has died");
        FindFirstObjectByType<GameManager>().isGameActive = false; // Set the game over state
        Destroy(this.gameObject, 1f); // Destroy the player game object
    }

    private void PlayRandomFootstepSound()
    {
        if (footstepSounds != null && footstepSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            audioSource.PlayOneShot(footstepSounds[randomIndex], 0.7f); // Lower volume for footsteps
        }
    }
}
