using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public static class URLS {
    public static string w2w_server_url = "http://192.168.1.248:5555";
}

// Define a class structure to represent the JSON response
public class Choice
{
    public int Index { get; set; }
    public MessageObject Message { get; set; }
    public object Logprobs { get; set; }
    public string FinishReason { get; set; }
}

public class MessageObject
{
    public string Role { get; set; }
    public string Content { get; set; }
}

public class ResponseObject
{
    public string Id { get; set; }
    public string Object { get; set; }
    public long Created { get; set; }
    public string Model { get; set; }
    public Choice[] Choices { get; set; }
    public UsageObject Usage { get; set; }
    public string SystemFingerprint { get; set; }
}
public class UsageObject
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

public class HelperRoutines : MonoBehaviour
{
    
    public static IEnumerator SendLLMTextRequest(string inputText, string apikey, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // Define the URL
        string url = "https://api.openai.com/v1/chat/completions";

        // Define the JSON payload for GPT-3.5 turbo.
        string jsonPayload = "{\"model\":\"gpt-3.5-turbo\",\"messages\":[{\"role\":\"user\",\"content\":\""+ inputText + "\"}],\"max_tokens\":300}"; 


        // Create a new UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // Set the request body
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);

        // Prepare buffer for response.
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apikey);

        // Send the request and wait for completion.
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to get LLM response: {request.error}");
            onError?.Invoke(request.error);
        }
        else
        {
            string response = request.downloadHandler.text;

            // Deserialize the JSON string into ResponseObject
            ResponseObject responseObject = JsonConvert.DeserializeObject<ResponseObject>(response);
            // Access the message content
            string messageContent = responseObject.Choices[0].Message.Content;


            Debug.Log("Response: " + messageContent);
            onSuccess?.Invoke(messageContent);
        }
    }

    public static byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
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
