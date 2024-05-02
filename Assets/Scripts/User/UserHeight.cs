using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHeightAdjusterContinuous : MonoBehaviour
{
    public InputActionReference increaseAction;
    public InputActionReference decreaseAction;
    public float heightSpeed = 0.5f;  // Speed of height increase per second

    private void OnEnable()
    {
        increaseAction.action.Enable();
        decreaseAction.action.Enable();
    }

    private void OnDisable()
    {
        increaseAction.action.Disable();
        decreaseAction.action.Disable();
    }

    void Update()
    {
        if (increaseAction.action.ReadValue<float>() > 0.1f) // Check if the grab button is pressed
        {
            Vector3 newPosition = transform.position;
            newPosition.y += heightSpeed * Time.deltaTime; // Increase height based on time to make it smooth and frame-rate independent
            transform.position = newPosition;
        }

        if (decreaseAction.action.ReadValue<float>() > 0.1f) // Check if the grab button is pressed
        {
            Vector3 newPosition = transform.position;
            newPosition.y -= heightSpeed * Time.deltaTime; // Increase height based on time to make it smooth and frame-rate independent
            transform.position = newPosition;
        }
    }
}
