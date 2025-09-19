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

    [Header("Public Vars")]
    public float jumpForce = 3.5f;
    public float jumpHoldForce = 0.3f;
    public bool grounded;
    private Rigidbody2D rb2d;
    public float jumpTime = 0.25f;
    private float jumpTimeCounter;
    private bool stoppingJump;
    private bool isJumpPressed;

    private PlayerInput playerInput;
    private InputAction jumpAction;

    [Header("Private Vars")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radius = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];

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

    void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);

        // Start jump if grounded and jump is pressed
        if (isJumpPressed && grounded && jumpTimeCounter <= 0)
        {
            jumpTimeCounter = jumpTime;
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
            stoppingJump = false;
        }

        // Continue jump while button is held (variable jump height)
        if (isJumpPressed && !stoppingJump && jumpTimeCounter > 0 && !grounded)
        {
            rb2d.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
            jumpTimeCounter -= Time.deltaTime;
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
}