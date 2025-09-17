using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    // necessary for animations and physics
    private Rigidbody2D rb2d;
    private Animator myAnimator;

    private bool facingRight = true;

    // variables to play with
    public float speed = 2.0f;
    public float jumpForce = 8.0f;
    public float horizontalMovement; //=1 || -1 || 0

    // Ground detection
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask;
    private bool isGrounded;

    // Jump cooldown
    private float lastJumpTime;
    public float jumpCooldown = 0.2f;

    // Input System
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;

    private void Start()
    {
        // define the game objects found on the player
        rb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        
        // setup input system
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    // handles the inputs physics
    private void Update()
    {
        // Ground detection (only if groundCheck is assigned)
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        }
        else
        {
            // Fallback: simple ground detection based on vertical velocity
            // Only consider grounded if not moving upward
            isGrounded = rb2d.linearVelocity.y <= 0.1f;
        }

        // check if the player has inputs movement
        if (moveAction != null)
        {
            horizontalMovement = moveAction.ReadValue<Vector2>().x;
        }

        // check for jump input
        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            Jump();
            lastJumpTime = Time.time;
        }
    }

    // handles running the physics
    private void FixedUpdate()
    {
        // move the character left and right
        rb2d.linearVelocity = new Vector2(horizontalMovement * speed, rb2d.linearVelocity.y);

        // Add extra downward force when falling to make jumps snappier
        if (rb2d.linearVelocity.y < 0)
        {
            rb2d.linearVelocity += Vector2.down * Physics2D.gravity.y * 2f * Time.fixedDeltaTime;
        }

        // flip the character to face the correct direction
        Flip(horizontalMovement);

        myAnimator.SetFloat("speed", Math.Abs(horizontalMovement));
    }

    // flipping function
    private void Flip(float horizontal)
    {
        if (horizontal != 0)
        {
            if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
            {
                facingRight = !facingRight;
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }
        }
    }

    private void Jump()
    {
        rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
    }
}
