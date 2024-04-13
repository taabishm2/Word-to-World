using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Networking;
using UnityEngine.UI;


public class TakeScreenShot : MonoBehaviour
{
    // If running locally, get ip address of the system using `ipconfig getifaddr en0`.
    public string serverUrl = "http://10.5.3.74:9000/";
    public string imageUrl = "http://10.5.3.74:9000/receive_image";

    [SerializeField] public TextMeshProUGUI text;

    public InputActionProperty rightGripAction;

    public Camera snapshotCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get grip float values.
        float rightGripValue = rightGripAction.action.ReadValue<float>();

        if (rightGripValue > 0.5f) {
            Debug.Log("I am sending hello.");
            StartCoroutine(sendHello());
            // StartCoroutine(TakeScreenshotAndSend());
        }
    }

    IEnumerator sendHello()
    {
        Debug.Log("Inside enumerator.");
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                text.text = "Response from server:";
                Debug.Log("Response from server:");
                Debug.Log(request.downloadHandler.text);
            }
            else
            {
                text.text = "Failed to request API. Error: " + request.error;
                Debug.LogError("Failed to request API. Error: " + request.error);
            }
        }
    }

    IEnumerator TakeScreenshotAndQuery(string query)
    {
        yield return new WaitForEndOfFrame();

        // Capture the screenshot
        Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();

        // Convert the texture to bytes
        byte[] screenshotBytes = screenshotTexture.EncodeToPNG();
        
        // Create a UnityWebRequest
        using (UnityWebRequest www = new UnityWebRequest(imageUrl, "POST"))
        {
            // Set the request method and upload handler
            www.method = "POST";
            www.uploadHandler = new UploadHandlerRaw(screenshotBytes);
            www.uploadHandler.contentType = "image/png";

            // Send the request
            yield return www.SendWebRequest();

            // Check for errors
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error uploading screenshot: " + www.error);
            }
            else
            {
                Debug.Log("Screenshot uploaded successfully");
            }
        }
    }
}
