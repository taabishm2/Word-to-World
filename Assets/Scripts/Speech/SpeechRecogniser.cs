using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;
using UnityEngine.Audio;
using UnityEngine.Networking;


public class SpeechRecogniser : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI voiceText;
    [SerializeField] private TextMeshProUGUI agentText;

    private Conversation conversation = new Conversation();

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    public string apikey;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;

        // Test LLM agent.
        // StartCoroutine(HelperRoutines.SendLLMTextRequest("How to make an omelette?", apikey, OnAgentResponseReceived, OnError));
    }

    private void Update() {
        if (recording && Microphone.GetPosition(null) >= clip.samples) {
            StopRecording();
        }
    }

    private void StartRecording() {
        voiceText.color = Color.white;
        voiceText.text = "Recording...";
        startButton.interactable = false;
        stopButton.interactable = true;
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording() {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = HelperRoutines.EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void SendRecording() {
        voiceText.color = Color.yellow;
        voiceText.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            voiceText.color = Color.white;
            voiceText.text = response;
            startButton.interactable = true;

            // Send recongised text to 
            StartCoroutine(HelperRoutines.SendLLMTextRequest(response, apikey, OnAgentResponseReceived, OnError));
        }, error => {
            voiceText.color = Color.red;
            voiceText.text = error;
            startButton.interactable = true;
        });
    }

    private void OnAgentResponseReceived(string response) {
        agentText.text = response;
    }

    private void OnError(string error) {
        agentText.text = error;
    }
}
