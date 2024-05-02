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

    private Transform userOrigin;

    void Start() {
        StartCoroutine(sceneBuilder.GetAssetCatalog(URLS.w2w_server_url + "/create-scene", "Generate a scene with a building and trees", new Vector3(0,0,0), OnSuccess, OnError));
    }

    public void GenerateScene(string prompt, Vector3 hitPoint)
    {
        

        StartCoroutine(sceneBuilder.GetAssetCatalog(URLS.w2w_server_url + "/create-scene", prompt, hitPoint, OnSuccess, OnError));
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