using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playermovement : MonoBehaviour
{
    [Header("Movement")]
    [Header("Jumping")]
    [Header("GroundCheck")]
    bool isFacingRight = true;
    public Transform groundCheckPos;
    public Vector2 groundCheckSzie = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;
    public float speed = 5;
    public Rigidbody2D rb;
    float horizontalMovement;

    public float jumpower = 10f;
    public int maxJump = 2;
    private int Jumprem;

    [Header("Gravity")]
    public float basegarvity = 2f;
    public float maxfallspeed = 18f;
    public float fallmultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSzie = new Vector2(0.5f, 0.5f);
    public LayerMask wallLayer;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2f;
    bool isWallSliding;
    bool isGrounded;

    bool iswallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 WallJumpPower = new Vector2(5f, 10f);

    void Update()
    {
        GroundCheck();
        Gravity();
        wallslide();
        processwalljump();
        if (!iswallJumping)
        {
            rb.velocity = new Vector2(horizontalMovement * speed, rb.velocity.y);
            flip();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void jump(InputAction.CallbackContext context)
    {
        if (Jumprem > 0)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpower);
                Jumprem--;
            }
            else if (context.canceled)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                Jumprem--;
            }
        }
        if (context.performed && wallJumpTimer > 0f)
        {
            iswallJumping = true;
            rb.velocity = new Vector2(WallJumpPower.x * wallJumpDirection, WallJumpPower.y);
            wallJumpTimer = 0f;
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1;
                transform.localScale = ls;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSzie);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSzie);
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSzie, 0, groundLayer))
        {
            Jumprem = maxJump;
            isGrounded = true;
            isWallSliding = false; // Reset wall sliding when grounded
            cancelwalljump(); // Stop wall jump
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Gravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = basegarvity * fallmultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxfallspeed));
        }
        else
        {
            rb.gravityScale = basegarvity;
        }
    }

    private void flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }

    private void wallslide()
    {
        if (!isGrounded && wallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private bool wallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSzie, 0, wallLayer);
    }

    private void processwalljump()
    {
        if (isWallSliding)
        {
            iswallJumping = false;
            wallJumpTimer = wallJumpTime;
            wallJumpDirection = -transform.localScale.x;
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void cancelwalljump()
    {
        iswallJumping = false;
    }
}
