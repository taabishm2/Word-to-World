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
    public string bundleURL = "http://localhost:8000/shapes";
    string assetName = "sphere";

    public string generateSceneUrl = "http://localhost:8000/create_scene";

    public Transform userOrigin;

    public SceneBuilder sceneBuilder;

    /* Functionalities */
    // 1. Generate scene based on user input.
    // 2. Enable user feedback to improve upon a generated scene.
    // 3. Provide save functionality to users.

    void Start()
    {
        generateScene.onClick.AddListener(GenerateScene);

        UserInput("generate 2 spheres");
        // StartCoroutine(sceneBuilder.LoadAssetCoroutine(bundleURL, assetName, OnAssetLoaded, OnError));
    }

    private void GenerateScene() {
        errorText.text = "Fetching assets...";
        StartCoroutine(AssetLoader.LoadAssetCoroutine(bundleURL, assetName, OnAssetLoaded, OnError));
    }

    private void OnAssetLoaded(GameObject loadedGameObject)

    {
        GameObject instance = Instantiate(loadedGameObject);
        Debug.Log($"Successfully instantiated from the asset bundle.");
        instance.transform.position = new Vector3(0,0,0); // Change to your desired location
        instance.transform.eulerAngles = new Vector3(0,0,0); // Change to your desired location
    }

    public void UserInput(string prompt) {
        // Get Assets catalog for user provided prompt.
        StartCoroutine(sceneBuilder.GetAssetCatalog(generateSceneUrl, prompt, userOrigin, OnSuccess, OnError));
    }

    private void OnSuccess(string response) {
        errorText.text += response;
    }

    private void OnError(string error)
    {
        Debug.LogError(error);
        errorText.text = "this is the error " + error;
    }
}
