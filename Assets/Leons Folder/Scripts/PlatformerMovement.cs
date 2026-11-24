using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerMovement : MonoBehaviour
{
    private bool JumpS;
    public float movespeed = 5f;
    public float jumpHeight;
    private Rigidbody2D rb2d;
    public float _Movement;
    private BoxCollider2D coll;
    private bool canDash = true;
    public bool isDashing;
    public float dashingPower;
    public float dashingTime;
    public float dashingCooldown;
    private float direction;
    private bool WallReadyLeft;
    private bool WallReadyRight;
    public float ShardCount;
    public float JumpSHeight;
    public float WallJumpSpeed;
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
            rb2d.linearVelocityX = _Movement;
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
            _Movement = ctx.ReadValue<Vector2>().x * movespeed / 3;
            
        }
        else if (ShardCount >= 1) 
        {
            _Movement = ctx.ReadValue<Vector2>().x * movespeed;
        }

        if (_Movement < 0)
        {
            direction = -1;
        }
        else if (_Movement > 0)
        {
            direction = 1;
        }
    }
    // Jump
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed == false || isDashing == true)
            return;

        if (IsGrounded())
        {
            Jump();
        }
        // double jump
        else if (JumpS == true && ShardCount >= 2 && LeftCling() == false && RightCling() == false)
        {
            DoubleJump();
        }
        // Wall Jump
        else if (ShardCount >= 4)
        {
            WallJump();
        }
    }
    //Jump
    private void Jump()
    {
        if (ShardCount < 1)
        {
            rb2d.linearVelocityY = jumpHeight;
        }
        else
        {
            rb2d.linearVelocityY = jumpHeight * 2;
        }
    }
    // Double Jump
    private void DoubleJump()
    {
        rb2d.linearVelocityY = JumpSHeight;
        JumpS = false;
        Debug.Log ("Oops");
    }
    
    //Wall Jump
    private void WallJump()
    {
        if (LeftCling() && WallReadyLeft == true)
        {
            WallReadyLeft = false;
            WallReadyRight = true;
            rb2d.linearVelocityY = WallJumpSpeed;
            rb2d.linearVelocityX = -movespeed;
            //rb2d.linearVelocity = new Vector2(direction * WallJumpSpeed.x, WallJumpSpeed.y);
        }
        else if (RightCling() && WallReadyRight == true)
        {
            WallReadyLeft = true;
            WallReadyRight = false;
            rb2d.linearVelocityY = WallJumpSpeed;
            rb2d.linearVelocityX = -movespeed;
            //rb2d.linearVelocity = new Vector2(direction * WallJumpSpeed.x, WallJumpSpeed.y);
        }
    }

    //Regular Jump
    private bool IsGrounded()

    {
        if (LeftCling() || RightCling())
        {
            // Does Nothing
            return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .0f, jumpableGround);
        }
        else
        {
            return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        }
        
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
