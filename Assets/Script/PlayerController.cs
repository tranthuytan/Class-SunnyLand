using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    public float directionMove, moveSpeed = 7f;
    bool facingRight = true;
    public float jumpForce = 700f;
    Vector3 localScale;
    [SerializeField]
    private BoxCollider2D headCollider;
    [SerializeField]
    private CircleCollider2D bodyCollider;
    private float crouchSpeed = 3f;
    [SerializeField]
    private LayerMask platformLayerMask;
    private bool isGrounded;
    public ParticleSystem dust;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        localScale = transform.localScale;
        headCollider = GetComponent<BoxCollider2D>();
        bodyCollider = GetComponent<CircleCollider2D>();
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(5, 5), new Vector2(5, 5), 0, Vector2.down);
    }
    void Update()
    {
        headCollider.enabled = true;
        Debug.Log(isGrounded);
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
            playDust();
        }
        IsGrounded();
        SetAnimationState();
        directionMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(directionMove, rb.velocity.y);
    }
    void LateUpdate()
    {
        CheckWhereToFace();
    }
    void SetAnimationState()
    {
        if (directionMove == 0)
        {
            animator.SetBool("isRun", false);
        }
        else
        {
            animator.SetBool("isRun", true);
        }
        if (rb.velocity.y > 5)
        {
            animator.SetBool("isJump", true);
        }
        if (rb.velocity.y < -5)
        {
            animator.SetBool("isFall", true);
            animator.SetBool("isJump", false);
        }
        if (rb.velocity.y == 0)
        {
            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("isCrouch", true);
            headCollider.enabled = false;
            directionMove = Input.GetAxisRaw("Horizontal") * crouchSpeed;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            animator.SetBool("isCrouch", false);
        }
    }
    private void IsGrounded()
    {
        // Get the center and radius of the circle collider
        Vector2 center = (Vector2) transform.position + bodyCollider.offset;
        float radius = bodyCollider.radius * transform.lossyScale.x;

        // Calculate the start and end positions of the boxcast
        Vector2 start = new Vector2(center.x - radius, center.y - 0.1f);
        Vector2 end = new Vector2(center.x + radius, center.y - 0.1f);

        // Perform the boxcast and check for hits
        RaycastHit2D hit = Physics2D.BoxCast(center, new Vector2(radius * 2, 0.1f), 0f, Vector2.down, 0f, platformLayerMask);
        if (hit.collider != null)
        {
            Debug.Log("Hit ground!");
        }
    }
    void CheckWhereToFace()
    {
        if (directionMove > 0)
        {
            facingRight = true;
        }

        else if (directionMove < 0)
            facingRight = false;
        if ((facingRight && localScale.x < 0) || (!facingRight && localScale.x > 0))
        {
            localScale.x *= -1;
            playDust();
        }
        transform.localScale = localScale;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Item"))
        {
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag.Equals("Platform"))
        {
            isGrounded = true;
            playDust();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Platform"))
        {
            isGrounded = false;
            playDust();
        }
    }
    private void playDust()
    {
        dust.Play();
    }
}
