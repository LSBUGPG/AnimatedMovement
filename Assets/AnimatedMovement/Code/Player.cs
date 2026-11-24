using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference sprint;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float minimumSpeed = 0.01f;
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 5;
    [SerializeField] private float acceleration = 5;
    [SerializeField] private float jumpPower = 3;
    private Vector2 inputMovement;
    private bool sprinting;
    private bool onGround;
    private bool isJumping;

    private void OnEnable()
    {
        move.action.performed += Move;  
        move.action.canceled += Move;
        sprint.action.performed += Sprint;
        sprint.action.canceled += Sprint;
        jump.action.performed += Jump;
    }

    private void OnDisable()
    {
        move.action.performed -= Move;
        move.action.canceled -= Move;
        sprint.action.performed -= Sprint;
        sprint.action.canceled -= Sprint;
        jump.action.performed -= Jump;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (onGround)
        {
            body.AddForce(Vector2.up * jumpPower * body.mass, ForceMode2D.Impulse);
            onGround = false;
            isJumping = true;
            animator.Play("jumping", -1);
            animator.SetBool("jumping", true);
        }
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        sprinting = context.ReadValue<float>() > 0;
    }

    private void Move(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }

    private void Walk()
    {
        animator.SetBool("braking", false);
        animator.Play("walk", -1);
    }

    private void Run()
    {
        animator.SetBool("braking", false);
        animator.Play("run", -1);
    }

    private void Brake()
    {
        animator.Play("start-braking", -1);
        animator.SetBool("braking", true);
    }

    private void Stop()
    {
        animator.SetBool("braking", false);
    }

    private void FaceDirection(float direction)
    {
        if (direction < -minimumSpeed)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction > minimumSpeed)
        {
            spriteRenderer.flipX = false;
        }
    }

    private bool WantToMove(float input)
    {
        return input < 0 || input > 0;
    }

    private bool Moving(float movement)
    {
        return movement > minimumSpeed || movement < -minimumSpeed;
    }

    private bool ChangingDirection(float desired, float current)
    {
        return desired * current < 0;
    }

    private Vector2 GetDesiredMovement()
    {
        Vector2 movement = inputMovement;
        if (sprinting)
        {
            movement *= runSpeed;
        }
        else
        {
            movement *= walkSpeed;
        }
        return movement;
    }

    private void FixedUpdate()
    {
        FaceDirection(body.linearVelocity.x);

        Vector2 desiredMovement = GetDesiredMovement();

        if (isJumping)
        {
        }
        else if (WantToMove(desiredMovement.x))
        {
            if (ChangingDirection(desiredMovement.x, body.linearVelocity.x))
            {
                Brake();
            }
            else
            {
                if (sprinting)
                {
                    Run();
                }
                else
                {
                    Walk();
                }
            }
        }
        else // don't want to move
        {
            if (Moving(body.linearVelocity.x))
            {
                Brake();
            }
            else
            {
                Stop();
            }
        }

        Vector2 difference = desiredMovement - body.linearVelocity;
        difference.y = 0;
        body.AddForce(difference * acceleration * body.mass);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            onGround = true;
            isJumping = false;
            animator.SetBool("jumping", false);
        }
    }
}
