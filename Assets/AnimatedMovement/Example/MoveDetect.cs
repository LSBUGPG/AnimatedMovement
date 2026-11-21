using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MoveDetect : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        move.action.performed += Move;
        move.action.canceled += Move;
    }

    private void Move(InputAction.CallbackContext context)
    {
        slider.value = context.ReadValue<Vector2>().x;
    }

    private void OnDisable()
    {
        move.action.performed -= Move;
        move.action.canceled -= Move;
    }
}
