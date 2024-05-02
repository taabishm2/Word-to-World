using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DynamicGrabInteractor : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to the hover events
        GetComponent<XRBaseInteractor>().hoverEntered.AddListener(HandleHoverEntered);
        GetComponent<XRBaseInteractor>().hoverExited.AddListener(HandleHoverExited);
    }

    private void OnDisable()
    {
        // Unsubscribe from the hover events
        GetComponent<XRBaseInteractor>().hoverEntered.RemoveListener(HandleHoverEntered);
        GetComponent<XRBaseInteractor>().hoverExited.RemoveListener(HandleHoverExited);
    }

    private void HandleHoverEntered(HoverEnterEventArgs args)
    {
        // Check if the object already has a XRGrabInteractable component
        XRGrabInteractable grabInteractable = args.interactable.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            // Add XRGrabInteractable if not already attached
            grabInteractable = args.interactable.gameObject.AddComponent<XRGrabInteractable>();
            // Optionally configure the grab interactable here
            ConfigureGrabInteractable(grabInteractable);
        }
    }

    private void HandleHoverExited(HoverExitEventArgs args)
    {
        // Optionally remove the XRGrabInteractable if you want to clean up
        XRGrabInteractable grabInteractable = args.interactable.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Perform any cleanup if necessary before removal
            Destroy(grabInteractable);
        }
    }

    private void ConfigureGrabInteractable(XRGrabInteractable interactable)
    {
        // Set any properties specific to your needs
        interactable.interactionLayerMask = LayerMask.GetMask("Interactable");
        // You can set other properties like attachTransform, movementType, etc.
    }
}

