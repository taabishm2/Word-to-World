using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class DestroyOnRayCasterPrimary : MonoBehaviour
{
    private InputActionReference primaryButtonAction;
    private XRBaseInteractable interactable;
    private bool isHovered = false;

    private AudioSource audioSource; // AudioSource component

    public AudioClip clip;

    public void SetAudioClip(AudioClip fadeclip) {
        clip = fadeclip;
    }

    void Start() {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null) {
            Debug.LogWarning("AudioSource component missing, adding one.");
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not already added
        }
    }

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

    IEnumerator PlaySoundAndDestroy()
    {
        audioSource.PlayOneShot(clip); // Play the audio clip
        yield return new WaitForSeconds(clip.length); // Wait for the clip to finish
        Destroy(gameObject); // Destroy the GameObject after the clip has finished
    }

    private void Update()
    {
        if (isHovered && primaryButtonAction.action.triggered)
        {
            StartCoroutine(PlaySoundAndDestroy());
        }
    }
}
