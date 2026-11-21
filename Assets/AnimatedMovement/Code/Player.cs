using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float minimumSpeed = 0.01f;
    [SerializeField] private float speed = 5;
    [SerializeField] private float acceleration = 5;
    private Vector2 desiredMovement;


    private void OnEnable()
    {
        move.action.performed += Move;
        move.action.canceled += Move;
    }

    private void OnDisable()
    {
        move.action.performed -= Move;
        move.action.canceled -= Move;
    }

    private void Move(InputAction.CallbackContext context)
    {
        desiredMovement = context.ReadValue<Vector2>() * speed;
    }

    private void Walk()
    {
        animator.SetBool("braking", false);
        animator.Play("walk", -1);
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
        FaceDirection(body.linearVelocity.x);

        if (WantToMove(desiredMovement.x))
        {
            if (ChangingDirection(desiredMovement.x, body.linearVelocity.x))
            {
                Brake();
            }
            else
            {
                Walk();
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
}
