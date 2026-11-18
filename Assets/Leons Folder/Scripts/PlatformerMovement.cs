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
    }
    public void Move(InputAction.CallbackContext ctx)
    {
        _movement = ctx.ReadValue<Vector2>().x * movespeed;
    }
    // Jump
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 1 && IsGrounded() && (isDashing = false))
        {
            rb2d.linearVelocityY = jumpHeight;
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
    // Dash

    public void Dasher(InputAction.CallbackContext ctx)
    {
        if (canDash == true)
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
        rb2d.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        trailrenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        trailrenderer.emitting = false;
        rb2d.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
