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

public class GripToSpeak : MonoBehaviour
{
    public InputActionReference gripActionReference; // Assign in the inspector

    public TextMeshPro voiceText;
    public AudioClip startRecordingClip; // AudioClip for start recording
    public AudioClip stopRecordingClip; // AudioClip for stop recording
    
    private AudioSource audioSource; // AudioSource component

    private byte[] bytes;
    private AudioClip clip;
    private bool isRecording = false;

    private void Awake() {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null) {
            Debug.LogWarning("AudioSource component missing, adding one.");
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not already added
        }
    }

    private void OnEnable()
    {
        gripActionReference.action.performed += HandleGripPress;
        gripActionReference.action.canceled += HandleGripRelease;
        gripActionReference.action.Enable();
    }

    private void OnDisable()
    {
        gripActionReference.action.performed -= HandleGripPress;
        gripActionReference.action.canceled -= HandleGripRelease;
        gripActionReference.action.Disable();
    }

    private void HandleGripPress(InputAction.CallbackContext context)
    {
        if (!isRecording)
        {
            StartRecording();
        }
    }

    private void HandleGripRelease(InputAction.CallbackContext context)
    {
        if (isRecording)
        {
            StopRecording();
        }
    }

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
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        audioSource.PlayOneShot(stopRecordingClip); // Play stop sound

        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = HelperRoutines.EncodeAsWAV(samples, clip.frequency, clip.channels);

        SendRecording();
        isRecording = false;
    }

    private void SendRecording() {
        voiceText.color = Color.yellow;
        voiceText.text = "Sending...";

        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            voiceText.color = Color.white;
            voiceText.text = response;
        }, error => {
            voiceText.color = Color.red;
            voiceText.text = error;
        });
    }
}
