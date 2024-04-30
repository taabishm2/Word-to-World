using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameSceneGenerator : MonoBehaviour
{

    public SceneBuilder sceneBuilder;
    public string generateSceneUrl = "http://127.0.0.1:5555/create-scene";
    public string initialPrompt = "a few houses and trees with a sunny day in a countryside";

    private Transform userOrigin;

    // void Start()
    // {
    //     UserInput(initialPrompt);
    // }

    // private void GenerateScene() {
    //     errorText.text = "Fetching assets...";
    //     StartCoroutine(AssetLoader.LoadAssetCoroutine(bundleURL, assetName, OnAssetLoaded, OnError));
    // }

    private void OnAssetLoaded(GameObject loadedGameObject)

    {
        GameObject instance = Instantiate(loadedGameObject);
        // Debug.Log($"Successfully instantiated from the asset bundle.");
        instance.transform.position = new Vector3(0, 0, 0); // Change to your desired location
        instance.transform.eulerAngles = new Vector3(0, 0, 0); // Change to your desired location
    }

    public void GenerateScene(string prompt)
    {
        // Get Assets catalog for user provided prompt.
        Debug.Log("User input: " + prompt);
        Debug.Log("Scene Builder: " + sceneBuilder);
        Debug.Log("Generate Scene URL: " + generateSceneUrl);
        Debug.Log("User Origin: " + userOrigin);

        StartCoroutine(sceneBuilder.GetAssetCatalog(generateSceneUrl, prompt, OnSuccess, OnError));
    }

    private void OnSuccess(string response)
    {
        Debug.Log("Success: " + response);
    }

    private void OnError(string error)
    {
        Debug.LogError("ERROR IS: " + error);
        // errorText.text = "this is the error " + error;
    }
}