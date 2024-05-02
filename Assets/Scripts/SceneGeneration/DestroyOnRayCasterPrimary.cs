using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class DestroyOnRayCasterPrimary : MonoBehaviour
{
    private InputActionReference primaryButtonAction;
    private XRBaseInteractable interactable;
    private bool isHovered = false;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(HandleHoverEntered);
        interactable.hoverExited.AddListener(HandleHoverExited);
    }

    public void SetPrimaryButtonAction(InputActionReference actionReference)
    {
        primaryButtonAction = actionReference;
        primaryButtonAction.action.Enable();
    }

    private void OnDestroy()
    {
        interactable.hoverEntered.RemoveListener(HandleHoverEntered);
        interactable.hoverExited.RemoveListener(HandleHoverExited);
        // if (primaryButtonAction != null)
        // {
        //     primaryButtonAction.action.Disable();
        // }
    }

    private void HandleHoverEntered(HoverEnterEventArgs arg)
    {
        isHovered = true;
    }

    private void HandleHoverExited(HoverExitEventArgs arg)
    {
        isHovered = false;
    }

    private void Update()
    {
        if (isHovered && primaryButtonAction.action.triggered)
        {
            Destroy(gameObject);
        }
    }
}
