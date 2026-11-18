using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minimumSpeed = 0.001f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private InputActionReference move;
    private float desiredX;

    private int walk = Animator.StringToHash("walk");
    private int braking = Animator.StringToHash("braking");
    private int stopped = Animator.StringToHash("stopped");
    private int startBraking = Animator.StringToHash("start-braking");

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

    private void FaceDirection(float x)
    {
        if (x == 0)
        {
            return;
        }

        spriteRenderer.flipX = x < 0f;
    }

    private void Walk()
    {
        animator.SetBool(braking, false);
        animator.Play(walk, -1);
    }

    private void Stop()
    {
        animator.SetBool(braking, false);
        animator.Play(stopped, -1);
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
            currentX = 0;
        }

        if (desiredX == 0f && currentX == 0f)
        {
            Stop();
        }
        else if (SameDirection(currentX, desiredX))
        {
            FaceDirection(desiredX);
            Walk();
        }
        else
        {
            Brake();
        }

        float difference = desiredX - currentX;
        body.AddForce(Vector2.right * difference * acceleration * body.mass, ForceMode2D.Force);
    }

    private bool SameDirection(float current, float desired)
    {
        return Math.Sign(current) * Math.Sign(desired) >= 0;
    }
}
