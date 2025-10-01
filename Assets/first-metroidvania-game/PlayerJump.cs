using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerJump : MonoBehaviour
{
    // force, apply, force, 1x
    // rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    [Header("Jump Details")]
    public float jumpForce;
    public float jumpHoldForce;
    public bool grounded;
    public float jumpTime;
    private float jumpTimeCounter;
    private bool stoppingJump;
    private bool isJumpPressed;
    private PlayerInput playerInput;
    private InputAction jumpAction;

    [Header("Ground Details")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radius = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Components")]
    private Rigidbody2D rb2d;
    private Animator myAnimator;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];
        myAnimator = GetComponent<Animator>();

        // Subscribe to the jump action events
        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        isJumpPressed = true;
        Debug.Log("Jump action performed!");
    }

    void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumpPressed = false;
        jumpTimeCounter = 0;
        stoppingJump = true;
        Debug.Log("Jump action canceled!");
    }

    // myAnimator.SetBool("falling", true);
    // myAnimator.SetBool("falling", false);

    // myAnimator.SetTrigger("jump");
    // myAnimator.ResetTrigger("jump");

    void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);

        if (grounded)
        {
            myAnimator.ResetTrigger("jump");
            myAnimator.SetBool("falling", false);
        }

        // Start jump if grounded and jump is pressed
        if (isJumpPressed && grounded && jumpTimeCounter <= 0)
        {
            jumpTimeCounter = jumpTime;
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
            stoppingJump = false;

            // tell the animator to play the jump animation
            myAnimator.SetTrigger("jump");
        }

        // Continue jump while button is held (variable jump height)
        if (isJumpPressed && !stoppingJump && jumpTimeCounter > 0 && !grounded)
        {
            rb2d.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
            jumpTimeCounter -= Time.deltaTime;

            myAnimator.SetTrigger("jump");
        }
        else if (!isJumpPressed && jumpTimeCounter > 0)
        {
            // Cut jump short if button released
            jumpTimeCounter = 0;
            stoppingJump = true;
            if (rb2d.linearVelocity.y > 0)
            {
                rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, rb2d.linearVelocity.y * 0.25f);
            }
            myAnimator.SetBool("falling", true);
            myAnimator.ResetTrigger("jump");
        }

        if (rb2d.linearVelocity.y < 0)
        {
            myAnimator.SetBool("falling", true);
        }
    }

    void OnDestroy()
    {
        Debug.Log("Jump action unsubscribed.");
        // Unsubscribe to prevent memory leaks when the object is destroyed
        jumpAction.performed -= OnJumpPerformed;
        jumpAction.canceled -= OnJumpCanceled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }
    
    private void FixedUpdate()
    {
        HandleLayers();
    }

    // Handle animator layers
    // when jumping, set layer 1 weight to 1 (upper body)
    // when not jumping, set layer 1 weight to 0 (full body)
    private void HandleLayers()
    {
        if (!grounded)
        {
            myAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            myAnimator.SetLayerWeight(1, 0);
        }
    }
}