using System.IO;
using Unity.Jobs;
using UnityEngine;

public class Player : MonoBehaviour
{
   
    private float movement;
    public float moveSpeed = 5f; // Speed at which the player moves
    private bool facingRight = true; // Track the direction the player is facing
    public Rigidbody2D rb; // Reference to the Rigidbody2D component
    public float jumpHeight = 5f; // Height of the jump
    public bool isGround = true; // Track if the player is on the ground
    public Animator animator; // Reference to the Animator component for animations
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movement = Input.GetAxis("Horizontal"); // Get the horizontal input axis
        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f); // Flip the player to face left
            facingRight = false; // Update the facing direction
        }
        else if (movement > 0f && facingRight==false)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f); // Flip the player to face right
            facingRight = true; // Update the facing direction

        }

        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            Jump(); // Call the Jump method when the space key is pressed
            isGround = false; // Set isGround to false to prevent double jumping
            animator.SetBool("Jump",true);
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
        transform.position += new Vector3(movement,0f,0f) *Time.fixedDeltaTime * moveSpeed;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f,jumpHeight) , ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGround = true; // Set isGround to true when the player collides with the grounds 
            animator.SetBool("Jump",false);  
        }
    }
}
