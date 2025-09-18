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
    public float horizontalMovement; //=1 || -1 || 0

    // Input System
    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Start()
    {
        // define the game objects found on the player
        rb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        
        // setup input system
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    // handles the inputs physics
    private void Update()
    {
        // check if the player has inputs movement
        if (moveAction != null)
        {
            horizontalMovement = moveAction.ReadValue<Vector2>().x;
        }
    }

    // handles running the physics
    private void FixedUpdate()
    {
        // move the character left and right
        rb2d.linearVelocity = new Vector2(horizontalMovement * speed, rb2d.linearVelocity.y);

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
}
