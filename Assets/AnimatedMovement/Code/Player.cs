using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference jumpInput;
    [SerializeField] private InputActionReference runModifier;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float minimumSpeed = 0.01f;
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float runSpeed = 6;
    [SerializeField] private float acceleration = 5;
    [SerializeField] private float jumpAcceleration = 10;
    private Vector2 movement;
    private bool running;
    private bool grounded = false;

    private readonly int run = Animator.StringToHash("run");
    private readonly int walk = Animator.StringToHash("walk");
    private readonly int brake = Animator.StringToHash("start-braking");
    private readonly int braking = Animator.StringToHash("braking");
    private readonly int jumping = Animator.StringToHash("jumping");


    private void OnEnable()
    {
        moveInput.action.performed += Move;
        moveInput.action.canceled += Move;
        runModifier.action.performed += Run;
        runModifier.action.canceled += Run;
        jumpInput.action.performed += Jump;
    }

    private void OnDisable()
    {
        moveInput.action.performed -= Move;
        moveInput.action.canceled -= Move;
        runModifier.action.performed -= Run;
        runModifier.action.canceled -= Run;
        jumpInput.action.performed += Jump;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!grounded)
        {
            return;
        }

        body.AddForce(Vector2.up * jumpAcceleration * body.mass, ForceMode2D.Impulse);
        grounded = false;
    }

    private void Run(InputAction.CallbackContext context)
    {
        running = context.ReadValue<float>() > 0;
    }

    private void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    private void Run()
    {
        animator.SetBool(braking, false);
        animator.Play(run, -1);
    }

    private void Walk()
    {
        animator.SetBool(braking, false);
        animator.Play(walk, -1);
    }

    private void Brake()
    {
        animator.Play(brake, -1);
        animator.SetBool(braking, true);
    }

    private void Stop()
    {
        animator.SetBool(braking, false);
    }

    private void Jump()
    {
        animator.Play(jumping, -1);
        animator.SetBool(braking, false);
        animator.SetBool(jumping, true);
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
        if (desired < 0 && current > 0)
        {
            return true;
        }

        if (desired > 0 && current < 0)
        {
            return true;
        }

        return false;
    }

    private void FixedUpdate()
    {
        Vector2 velocity = movement * (running? runSpeed : walkSpeed);
        FaceDirection(body.linearVelocity.x);

        if (!grounded)
        {
            Jump();
        }
        else if (WantToMove(velocity.x))
        {
            if (ChangingDirection(velocity.x, body.linearVelocity.x))
            {
                Brake();
            }
            else
            {
                if (Mathf.Abs(body.linearVelocity.x) > walkSpeed)
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

        Vector2 difference = velocity - body.linearVelocity;
        difference.y = 0;
        body.AddForce(difference * acceleration * body.mass);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = true;
            animator.SetBool(jumping, false);
        }
    }
}
