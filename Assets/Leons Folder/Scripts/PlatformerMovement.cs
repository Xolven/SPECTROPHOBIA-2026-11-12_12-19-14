using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerMovement : MonoBehaviour
{
    public float movespeed;
    public float jumpHeight;
    private Rigidbody2D rb2d;
    private float _movement;
    private BoxCollider2D coll;
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower;
    public float dashingTime;
    public float dashingCooldown;
    private float direction;
    private bool JumpS;
    private bool WallReadyLeft = true;
    private bool WallReadyRight = true;
    public float ShardCount;
    public float JumpSHeight;
    public float WallJumpHeight;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private TrailRenderer trailrenderer;
    [SerializeField] private LayerMask WallRight;
    [SerializeField] private LayerMask WallLeft;
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isDashing == false)
        {
            rb2d.linearVelocityX = _movement;
        }
        if (IsGrounded())
        {
            JumpS = true;
            WallReadyLeft = true;
            WallReadyRight = true;
        }

    }
    //walk
    public void Move(InputAction.CallbackContext ctx)
    {
        if (ShardCount <= 0)
        {
            _movement = ctx.ReadValue<Vector2>().x * movespeed / 3;
        }
        else if (ShardCount >= 1) 
        {
            _movement = ctx.ReadValue<Vector2>().x * movespeed;
        }

        if (_movement < 0)
        {
            direction = -1;
        }
        else if (_movement > 0)
        {
            direction = 1;
        }
    }
    // Jump
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 1 && IsGrounded() && (isDashing == false) && (ShardCount < 1))
        {
            rb2d.linearVelocityY = jumpHeight;
        }
        else if (ctx.ReadValue<float>() == 1 && IsGrounded() && (isDashing == false) && (ShardCount >= 1))
        {
            rb2d.linearVelocityY = jumpHeight * 2;
            Debug.Log("Full Jump");
        }
        // Wall Jump
        else if (ctx.ReadValue<float>() == 1 && LeftCling() && (isDashing == false) && (WallReadyLeft == true) && (ShardCount >= 4))
        {
            WallReadyLeft = false;
            WallReadyRight = true;
            rb2d.linearVelocityY = WallJumpHeight;
        }
        else if (ctx.ReadValue<float>() == 1 && RightCling() && (isDashing == false) && (WallReadyRight == true) && (ShardCount >= 4))
        {
            WallReadyLeft = true;
            WallReadyRight = false;
            rb2d.linearVelocityY = WallJumpHeight;
        }
        // double jump
        else if (ctx.ReadValue<float>() == 1 && (isDashing = false) && (JumpS = true) && (ShardCount >= 2))
        {
            Debug.Log("I did it");
            rb2d.linearVelocityY = JumpSHeight;
            JumpS = false;
        }
    }
    //Regular Jump
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    // wall jump pt 2
    private bool LeftCling()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, WallLeft);
    }
    private bool RightCling()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, WallLeft);
    }
    // Dash

    public void Dasher(InputAction.CallbackContext ctx)
    {
        if (canDash == true && (ShardCount >= 3))
        {
            StartCoroutine(Dash());
        }
    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = .1f;
        rb2d.linearVelocity = new Vector2(direction * dashingPower, 0f);
        trailrenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        trailrenderer.emitting = false;
        rb2d.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
