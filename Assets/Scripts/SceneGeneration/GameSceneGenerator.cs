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

    void Start() {
        StartCoroutine(sceneBuilder.GetAssetCatalog(generateSceneUrl, initialPrompt, new Vector3(0,0,0), OnSuccess, OnError));
    }

    public void GenerateScene(string prompt, Vector3 hitPoint)
    {
        

        StartCoroutine(sceneBuilder.GetAssetCatalog(generateSceneUrl, prompt, hitPoint, OnSuccess, OnError));
    }

    private void OnSuccess(string response)
    {
        Debug.Log("Success: " + response);
    }

    private void OnError(string error)
    {
        Debug.LogError("ERROR IS: " + error);
    }
}