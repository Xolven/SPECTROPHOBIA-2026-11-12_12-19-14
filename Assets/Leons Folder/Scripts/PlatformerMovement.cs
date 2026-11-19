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
    public float dashingPower = 100f;
    public float dashingTime = 1f;
    public float dashingCooldown = 5f;
    private float direction;
    private bool JumpS;

    public float ShardCount;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private TrailRenderer trailrenderer;
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
        }
    }
    public void Move(InputAction.CallbackContext ctx)
    {
        if (ShardCount <= 0)
        {
            _movement = ctx.ReadValue<Vector2>().x * movespeed / 3;
        }
        else if (ShardCount > 0) 
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
        if (ctx.ReadValue<float>() == 1 && IsGrounded() && (isDashing == false))
        {
            rb2d.linearVelocityY = jumpHeight;
        }
        else if (JumpS == true && ctx.ReadValue<float>() == 1 && (ShardCount >= 2))
        {
            rb2d.linearVelocityY = jumpHeight;
            JumpS = false;
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
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
