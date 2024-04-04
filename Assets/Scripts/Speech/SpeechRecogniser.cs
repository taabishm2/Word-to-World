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
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
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
            StartCoroutine(SendRequest(voiceText.text));
        }, error => {
            voiceText.color = Color.red;
            voiceText.text = error;
            startButton.interactable = true;
        });
    }

    private void sendQuery(string inputText) {
        HuggingFaceAPI.Conversation("hello, how are you?", response => {
                string reply = conversation.GetLatestResponse();
                agentText.text = $"Bot: {reply}";
            }, error => {
                agentText.text = $"Error: {error}";
                agentText.color = Color.red;
            }, conversation);
    }

    IEnumerator SendRequest(string inputText)
    {
        // Define the URL
        string url = "https://api.openai.com/v1/chat/completions";

        // Define the JSON payload
        string jsonPayload = "{\"model\":\"gpt-3.5-turbo\",\"messages\":[{\"role\":\"user\",\"content\":"+ inputText + "\"}],\"max_tokens\":300}"; 


        // Create a new UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // Set the request body
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);

        // Set headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apikey);

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + request.error);
            agentText.text = request.error;
            agentText.color = Color.red;
        }
        else
        {
            if(request.downloadHandler == null) {
                Debug.Log("request is null");
            }
            // Get the response
            string response = request.downloadHandler.text;
            // agentText.text = response;
            Debug.Log("Response: " + response);
        }
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}
