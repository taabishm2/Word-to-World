using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameSceneGenerator : MonoBehaviour
{
    [SerializeField] private Button generateScene;

    [SerializeField] private TextMeshProUGUI errorText;
    public string bundleURL = "http://127.0.0.1:8000/fbxassetbundle";

    public string generateSceneUrl = "http://127.0.0.1:5555/create-scene";

    public Transform userOrigin;

    public SceneBuilder sceneBuilder;

    public string InitialPrompt = "a few houses and trees with a sunny day in a countryside";

    /* Functionalities */
    // 1. Generate scene based on user input.
    // 2. Enable user feedback to improve upon a generated scene.
    // 3. Provide save functionality to users.

    void Start()
    {
        // generateScene.onClick.AddListener(GenerateScene);

        // sceneBuilder.SomeMethod();
        UserInput(InitialPrompt);
        // StartCoroutine(sceneBuilder.LoadAssetCoroutine(bundleURL, assetName, OnAssetLoaded, OnError));
    }

    // private void GenerateScene() {
    //     errorText.text = "Fetching assets...";
    //     StartCoroutine(AssetLoader.LoadAssetCoroutine(bundleURL, assetName, OnAssetLoaded, OnError));
    // }

    private void OnAssetLoaded(GameObject loadedGameObject)

    {
        GameObject instance = Instantiate(loadedGameObject);
        // Debug.Log($"Successfully instantiated from the asset bundle.");
        instance.transform.position = new Vector3(0,0,0); // Change to your desired location
        instance.transform.eulerAngles = new Vector3(0,0,0); // Change to your desired location
    }

    public void UserInput(string prompt) {
        // Get Assets catalog for user provided prompt.
        // Debug.Log("User input: " + prompt);
        // Debug.Log("Scene Builder: " + sceneBuilder);
        // Debug.Log("Generate Scene URL: " + generateSceneUrl);
        // Debug.Log("User Origin: " + userOrigin);
        
        
        StartCoroutine(sceneBuilder.GetAssetCatalog(generateSceneUrl, prompt, OnSuccess, OnError));
    }

    private void OnSuccess(string response) {
        // Debug.Log("Success: " + response);
    }

    private void OnError(string error)
    {
        Debug.LogError("ERROR IS: " + error);
        // errorText.text = "this is the error " + error;
    }
}