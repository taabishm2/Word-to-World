using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class GripToSpeak : MonoBehaviour
{
    public GameSceneGenerator gameSceneGenerator;
    // public InputActionReference gripActionReference; // Assign in the inspector
    public XRRayInteractor rayInteractor;

    public TextMeshPro voiceText;
    public AudioClip startRecordingClip; // AudioClip for start recording
    public AudioClip stopRecordingClip; // AudioClip for stop recording
    
    private AudioSource audioSource; // AudioSource component
    private byte[] bytes;
    private AudioClip clip;
    private bool isRecording = false;

    public InputActionProperty gripAnimationAction;

    private void Awake() {
        Debug.Log("Ray Interactor: " + rayInteractor);
        
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null) {
            Debug.LogWarning("AudioSource component missing, adding one.");
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not already added
        }
    }

    // private void OnEnable()
    // {
    //     gripActionReference.action.performed += HandleGripPress;
    //     gripActionReference.action.canceled += HandleGripRelease;
    //     gripActionReference.action.Enable();
    // }

    // private void OnDisable()
    // {
    //     gripActionReference.action.performed -= HandleGripPress;
    //     gripActionReference.action.canceled -= HandleGripRelease;
    //     gripActionReference.action.Disable();
    // }

    void Update() {
        // Get trigger, grip float value.
        float gripValue = gripAnimationAction.action.ReadValue<float>();

        if (gripValue > 0.5 && !isRecording) {
            isRecording = true;
            StartRecording();
        } else if (gripValue < 0.5 && isRecording)
        {
            isRecording = false;
            StopRecording();
        }
    }

    // private void HandleGripPress(InputAction.CallbackContext context)
    // {
    //     if (!isRecording)
    //     {
    //         StartRecording();
    //     }
    // }

    // private void HandleGripRelease(InputAction.CallbackContext context)
    // {
    //     if (isRecording)
    //     {
    //         StopRecording();
    //     }
    // }

    private void StartRecording()
    {
        Debug.Log("Recording started");
        voiceText.color = Color.white;
        voiceText.text = "Recording...";
        audioSource.PlayOneShot(startRecordingClip); // Play start sound
        clip = Microphone.Start(null, false, 10, 44100);
        isRecording = true;
    }

    private void StopRecording()
    {
        Debug.Log("Recording stopped");

        RaycastHit hit;
        bool isHit = rayInteractor.TryGetCurrent3DRaycastHit(out hit);

        Vector3 hitPoint = Vector3.zero;
        if (isHit)
        {
            hitPoint = hit.point;
            Debug.Log("Hit point coordinates: " + hitPoint);
        }
        else
        {
            Debug.Log("The ray did not hit any object.");
        }

        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        audioSource.PlayOneShot(stopRecordingClip); // Play stop sound

        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = HelperRoutines.EncodeAsWAV(samples, clip.frequency, clip.channels);

        SendRecording(hitPoint);
        isRecording = false;
    }

    private void SendRecording(Vector3 hitPoint) {
        voiceText.color = Color.yellow;
        voiceText.text = "Sending...";

        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            voiceText.color = Color.white;
            voiceText.text = response;
            gameSceneGenerator.GenerateScene(response, hitPoint);
        }, error => {
            voiceText.color = Color.red;
            voiceText.text = error;
        });
    }
}
