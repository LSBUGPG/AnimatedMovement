using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minimumSpeed = 0.001f;
    [SerializeField] private float impulseFraction = 0.5f;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private InputActionReference move;
    private float desiredX;

    private int walk = Animator.StringToHash("walk");
    private int braking = Animator.StringToHash("braking");
    private int stopped = Animator.StringToHash("stopped");
    private int startBraking = Animator.StringToHash("start-braking");
    private int touchingGround = 0;

    private void OnEnable()
    {
        move.action.performed += OnMove;
        move.action.canceled += OnMove;
    }

    private void OnDisable()
    {
        move.action.performed -= OnMove;
        move.action.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Vector3 movement = new Vector3(input.x, 0f, 0f);
        desiredX = movement.x * speed;
    }

    private bool FaceDirection(float x)
    {
        if (x == 0f)
        {
            return false;
        }

        spriteRenderer.flipX = x < 0f;
        return true;
    }

    private void Walk()
    {
        animator.SetBool(braking, false);
        animator.Play(walk, -1);
    }

    private void Stop()
    {
        animator.SetBool(braking, false);
    }

    private void Brake()
    {
        if (animator.GetBool(braking))
        {
            return;
        }

        animator.SetBool(braking, true);
        animator.Play(startBraking, -1);
    }

    private void FixedUpdate()
    {
        float currentX = body.linearVelocity.x;
        if (Mathf.Abs(currentX) < minimumSpeed)
        {
            currentX = 0f;
        }

        if (!FaceDirection(currentX))
        {
            FaceDirection(desiredX);
        }

        if (desiredX == 0f && currentX == 0)
        {
            Stop();
        }
        else if (SameDirection(currentX, desiredX))
        {
            Walk();
        }
        else
        {
            Brake();
        }
    }

    private bool SameDirection(float current, float desired)
    {
        return current == 0 || Math.Sign(current) * Math.Sign(desired) == 1;
    }

    public void MovementImpulse()
    {
        if (touchingGround > 0)
        {
            if (body.linearVelocity.x != desiredX)
            {
                float deltaVelocity = desiredX - body.linearVelocity.x;
                float acceleration = deltaVelocity * impulseFraction;
                body.AddForce(Vector2.right * acceleration * body.mass, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && collision.transform.position.y > transform.position.y)
        {
            touchingGround++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && collision.transform.position.y > transform.position.y)
        {
            Invoke("CoyoteTime", 0.1f);
        }
    }

    private void CoyoteTime()
    {
        touchingGround--;
    }
}
