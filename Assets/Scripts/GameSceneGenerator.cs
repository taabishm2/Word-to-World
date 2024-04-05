using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class GameSceneGenerator : MonoBehaviour
{

    [SerializeField] private Button generateScene;

    [SerializeField] private TextMeshProUGUI errorText;
    public string bundleURL = "http://localhost:8000/shapes";
    string assetName = "sphere";

    /* Algorithm */
    // 0. Invoke script when "Describe Scene" button is clicked
    // 1. Get audio input from the microphone
    // 2. Transcribe audio input to text
    // 3. Send transcription to the W2W server (generateScene API)
    // 4. Receive JSON response from the server (containing asset bundle URL, asset name, and other scene details)
    // 5. Iterate over the JSON response and instantiate the scene objects (assets) in Unity
    // 6. Apply provided scene details (e.g., object positions, rotations, scales) to the instantiated objects
    // 7. On exiting game, serialize ALL scene details to JSON for populating in editor mode
    // 8. Before exiting game mode and entering editor mode, populate the scene in editor mode

    void Start()
    {
        generateScene.onClick.AddListener(GenerateScene);
        StartCoroutine(AssetLoader.LoadAssetCoroutine(bundleURL, assetName, OnAssetLoaded, OnError));
    }

    private void GenerateScene() {
        errorText.text = "Fetching assets...";
        StartCoroutine(AssetLoader.LoadAssetCoroutine(bundleURL, assetName, OnAssetLoaded, OnError));
    }

    private void OnAssetLoaded(GameObject loadedGameObject)
    {
        GameObject instance = Instantiate(loadedGameObject);
        errorText.text = $"Successfully instantiated '{assetName}' from the asset bundle.";   
        Debug.Log($"Successfully instantiated '{assetName}' from the asset bundle.");

        // Set the specific location here
        int xPos = UnityEngine.Random.Range(0, 3); // Generates a random integer between -5 and 5 (inclusive)
        int yPos = UnityEngine.Random.Range(0, 3); // Generates a random integer between -5 and 5 (inclusive)

        instance.transform.position = new Vector3(xPos, yPos, 2); // Change to your desired location
    }

    private void OnError(string error)
    {
        Debug.LogError(error);
        errorText.text = "this is the error " + error;
    }
}
