using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SceneBuilder : MonoBehaviour
{
    public string bundleURL = "http://127.0.0.1:8000/fbxassetbundle";

    [Serializable]
    public class SceneGenerationRequest
    {
        public string prompt;
        public Vector3 user_location;
        public Vector3 user_rotation;
    }

    [System.Serializable]
    public class AssetData
    {
        public string bundle_url;
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public class APIResponse
    {
        public AssetData[] assets;
    }

    public GameObject parentObject; // Assigned in the scene editor.

    void Start()
    {
        // Create or find the parent object
        parentObject = new GameObject("AssetsParent");
        // Set the custom transform for the parent object
        parentObject.transform.position = Vector3.zero; // Set the desired position
        parentObject.transform.rotation = Quaternion.identity; // Set the desired rotation

    }

    private static string preparePayload(string prompt)
    {
        // Create a SceneGenerationRequest object
        SceneGenerationRequest request = new SceneGenerationRequest();

        request.prompt = prompt;
        // request.user_location = 0;
        // request.user_rotation = 0;

        // Serialize the object to JSON
        string json = JsonUtility.ToJson(request);

        // Now you can send 'json' as part of your request payload
        // Debug.Log(json);


        return json;
    }

    public IEnumerator GetAssetCatalog(string generateSceneUrl, string prompt, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string jsonPayload = preparePayload(prompt);

        // Debug.Log("Sending request to: " + generateSceneUrl);
        using (UnityWebRequest uwr = UnityWebRequest.Put(generateSceneUrl, jsonPayload))
        {
            // Set the content type
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            // Check for errors
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(uwr.error);

                Debug.LogError("PUT request error: " + uwr.error);
            }
            else
            {
                string jsonResponse = uwr.downloadHandler.text;

                // Log the response
                // Debug.Log("PUT request successful: " + jsonResponse);


                // Deserialize the JSON response
                APIResponse response = JsonUtility.FromJson<APIResponse>(jsonResponse);

                // Spawn assets.
                yield return StartCoroutine(SpawnAssets(response.assets, onSuccess, onError));
            }
        }
    }

    private IEnumerator SpawnAssets(AssetData[] assets, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // Debug.Log("Spawning number of objects: " + assets.Length);

        foreach (AssetData asset in assets)
        {
            // Use a lambda expression to create a delegate with fixed position and rotation.
            System.Action<GameObject> callback = (loadedGameObject) =>
            {
                OnAssetLoaded(loadedGameObject, asset.position, asset.rotation, asset.scale);
            };

            // Debug.Log("Spawning object from " + asset.bundle_url + " name " + asset.name);

            //TODO: Parallelize this if possible
            // TODO: Parameterize the URL
            yield return StartCoroutine(LoadAssetCoroutine(bundleURL, asset.name, callback, onError));
        }

        onSuccess?.Invoke($"{assets.Length} assets successfully initialized.");
    }

    public IEnumerator LoadAssetCoroutine(string bundleURL, string assetName, System.Action<GameObject> onSuccess, System.Action<string> onError)
    {
        AssetBundle asset_bundle = null;

        // Check if AssetBundle is already loaded.

        // Get all loaded AssetBundles
        AssetBundle[] loadedAssetBundles = AssetBundle.GetAllLoadedAssetBundles().ToArray();

        // Iterate through the loaded AssetBundles and check if the URL matches
        foreach (AssetBundle bundle in loadedAssetBundles)
        {
            if (bundle.name == bundleURL)
            {
                asset_bundle = bundle;
            }
        }

        if (asset_bundle == null)
        {
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to download asset bundle: {uwr.error} {bundleURL}");
                    onError?.Invoke(uwr.error);
                }
                else
                {
                    asset_bundle = DownloadHandlerAssetBundle.GetContent(uwr);

                    if (asset_bundle == null)
                    {
                        Debug.LogError("Failed to load AssetBundle!");
                        onError?.Invoke("AssetBundle is NULL!");
                        yield break; // Terminate.
                    }
                }
            }
        }

        AssetBundleRequest request = asset_bundle.LoadAssetAsync<GameObject>(assetName);
        yield return request;

        if (request.asset == null)
        {
            Debug.LogError($"Failed to load asset '{assetName}' from bundle!");
            onError?.Invoke($"Failed to load asset '{assetName}' from bundle!");
        }
        else
        {
            onSuccess?.Invoke(request.asset as GameObject);
            // Debug.Log($"Successfully instantiated '{assetName}' from the asset bundle.");
        }

        asset_bundle.Unload(false);
    }

    private void OnAssetLoaded(GameObject loadedGameObject, Vector3 position, Vector3 rotation, Vector3 scale)

    {
        GameObject instance = Instantiate(loadedGameObject, parentObject.transform);

        // Apply the material to the instantiated object's renderer component
        Renderer renderer = instance.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            // Set the Universal Render Pipeline Lit shader
            Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLitShader != null)
            {
                renderer.material.shader = urpLitShader;
            }
        }

        Debug.Log("Pstition:" + position + ", Rotation:" + rotation + ", Scale:" + scale);
        instance.transform.position = position; // Change to your desired location
        instance.transform.eulerAngles = rotation; // Change to your desired location
        instance.transform.localScale = scale; // Change to your desired scale

        // Debug.Log("Successfully instantiated from the asset bundle.");
    }
}