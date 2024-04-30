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

    public string apikey;
    public TextMeshPro voiceText;
    private byte[] bytes;
    private AudioClip clip;
    private bool isRecording = false;

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
        clip = Microphone.Start(null, false, 10, 44100);
        isRecording = true;
    }

    private void StopRecording()
    {
        Debug.Log("Recording stopped");
        var position = Microphone.GetPosition(null);
        Microphone.End(null);

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
