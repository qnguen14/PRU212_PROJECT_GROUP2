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

    void Start()
    {
        UpdateCoinUI(); // Initialize the coin UI
    }

    // Update is called once per frame
    void Update()
    {
        if (maxHealth <= 0)
        {
            Die(); // Call the Die method if health is zero or below
        }

        health.text = maxHealth.ToString();

        movement = Input.GetAxis("Horizontal"); // Get the horizontal input axis
        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f); // Flip the player to face left
            facingRight = false; // Update the facing direction
        }
        else if (movement > 0f && facingRight == false)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f); // Flip the player to face right
            facingRight = true; // Update the facing direction
        }

        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            Jump(); // Call the Jump method when the space key is pressed
            isGround = false; // Set isGround to false to prevent double jumping
            animator.SetBool("Jump", true);
        }

        if (Mathf.Abs(movement) > .1f)
        {
            animator.SetFloat("Run", 1f); // Set the running animation if the player is moving right
        }
        else if (movement < .1f)
        {
            animator.SetFloat("Run", 0f); // Set the idle animation if the player is not moving
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack"); // Trigger the attack animation when the left mouse button is clicked
        }
    }

    private void FixedUpdate()
    {
        // Move the player based on the input
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * moveSpeed;
    }

    void Jump()
    {
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
}
