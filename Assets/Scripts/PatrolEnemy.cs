using Unity.Jobs;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public int maxHealth = 20;
    public bool facingLeft = true;
    public float moveSpeed = 2f;
    public Transform checkPoint;
    public float distance = 1f;
    public LayerMask layerMask;
    public bool inRange = false;
    public Transform player;
    public float attackRange = 10f;
    public float retrieveDistance = 2.5f;
    public float chaseSpeed = 4f;
    public Animator animator;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (FindAnyObjectByType<GameManager>().isGameActive == false)
        {
            return; // Exit the Update method if the game is paused
        }


        if (maxHealth <= 0)
        {
            Die(); // Call the Die method if health is zero or below
        }


        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }
        if (inRange)
        {
            if (player.position.x > transform.position.x && facingLeft == true)
            {
                // nếu player ở bên trái và hiện tại đang quay mặt sang phải, lật mặt sang trái
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            }
            else if (player.position.x < transform.position.x && facingLeft == false)
            {
                // nếu player ở bên phải và hiện tại đang quay mặt sang trái, lật mặt sang phải
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
            if (Vector2.Distance(transform.position, player.position) > retrieveDistance)
            {
                animator.SetBool("Attack1", false);
                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Attack1", true);
            }

        }
        else {
            // di chuyển sang trái
            transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);

            // quét tia xuống dưới để kiểm tra mặt đất
            RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);

            if (hit == false && facingLeft)
            {
                // không còn đất bên trước, lật mặt sang phải
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            }
            else if (hit == false && facingLeft == false)
            {
                // không còn đất bên trước, lật mặt sang trái
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
        }
    }
    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

        if (collInfo)
        {
            if (collInfo.gameObject.GetComponent<Player>() != null)
            {
                collInfo.gameObject.GetComponent<Player>().TakeDamage(8); // Gọi phương thức TakeDamage trên đối tượng player
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

    public void Die()
    {
        Debug.Log("Enemy has died");
        Destroy(this.gameObject, 1f); // Destroy the enemy game object
    }


    // Vẽ gizmo để hiển thị tia quét trong Editor
    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
