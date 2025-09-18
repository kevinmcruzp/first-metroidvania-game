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
    public float jumpForce = 3.0f;
    public bool grounded;
    private Rigidbody2D rb2d;

    private PlayerInput playerInput;
    private InputAction jumpAction;

    [Header("Private Vars")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radius = 0.03f;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];

        // Subscribe to the 'performed' event of the Jump action
        jumpAction.performed += OnJumpPerformed;
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // This method will be called when the Jump action is performed (e.g., Spacebar is pressed)
        Debug.Log("Jump action performed!");
        // Add your jump logic here (e.g., applying force to a Rigidbody)
        grounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);
        if (grounded)
        {
            // rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void OnDestroy()
    {
        Debug.Log("Jump action unsubscribed.");
        // Unsubscribe to prevent memory leaks when the object is destroyed
        jumpAction.performed -= OnJumpPerformed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }
}