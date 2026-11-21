using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyDetect : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private string key;
    private InputAction keyPress;

    private void OnEnable()
    {
        keyPress = new InputAction($"{key} key", binding: $"<Keyboard>/{key}");
        keyPress.performed += Highlight;
        keyPress.canceled += Restore;
        keyPress.Enable();
    }

    private void Restore(InputAction.CallbackContext context)
    {
        image.color = Color.white;
    }

    private void Highlight(InputAction.CallbackContext context)
    {
        image.color = Color.green;
    }

    private void OnDisable()
    {
        keyPress.Disable();
        keyPress.performed -= Highlight;
        keyPress.canceled -= Restore;
        keyPress.Dispose();
        keyPress = null;
    }
}
