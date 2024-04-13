using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;


public class TakeScreenShot : MonoBehaviour
{
    // If running locally, get ip address of the system using `ipconfig getifaddr en0`.
    public string serverUrl = "http://127.0.0.1:9000/";
    public string imageUrl = "http://127.0.0.1:9000/receive_image";

    [SerializeField] public Button feedBackButton;

    [SerializeField] public TextMeshProUGUI text;

    public Camera snapshotCamera;

    // Start is called before the first frame update
    void Start()
    {
        feedBackButton.onClick.AddListener(GetFeedback);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void GetFeedback() {
        StartCoroutine(TakeScreenshotAndQuery("what's in the image?"));
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
