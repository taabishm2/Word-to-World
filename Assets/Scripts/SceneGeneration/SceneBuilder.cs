using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SceneBuilder : MonoBehaviour
{
    public GameObject parentObject; // Assigned in the scene editor.

    [Serializable]
    public class SceneGenerationRequest
    {
        public string prompt;
        public Vector3 hit_point;
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

    void Start()
    {
        parentObject.transform.position = Vector3.zero;
        parentObject.transform.rotation = Quaternion.identity;
    }

    private static string preparePayload(string prompt, Vector3 hitPoint)
    {
        SceneGenerationRequest request = new SceneGenerationRequest();
        request.prompt = prompt;
        request.hit_point = hitPoint;

        // Serialize the object to JSON
        return JsonUtility.ToJson(request);
    }

    public IEnumerator GetAssetCatalog(string generateSceneUrl, string prompt, Vector3 hitPoint,
        System.Action<string> onSuccess, System.Action<string> onError)
    {
        string jsonPayload = preparePayload(prompt, hitPoint);

        using (UnityWebRequest uwr = UnityWebRequest.Put(generateSceneUrl, jsonPayload))
        {
            uwr.SetRequestHeader("Content-Type", "application/json");
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(uwr.error);
                Debug.LogError("PUT request error: " + uwr.error);
            }
            else
            {
                string jsonResponse = uwr.downloadHandler.text;
                APIResponse response = JsonUtility.FromJson<APIResponse>(jsonResponse);

                yield return StartCoroutine(SpawnAssets(response.assets, onSuccess, onError));
            }
        }
    }

    private IEnumerator SpawnAssets(AssetData[] assets, System.Action<string> onSuccess, System.Action<string> onError)
    {

        foreach (AssetData asset in assets)
        {
            // Use a lambda expression to create a delegate with fixed position and rotation.
            System.Action<GameObject> callback = (loadedGameObject) =>
            {
                OnAssetLoaded(loadedGameObject, asset.position, asset.rotation, asset.scale);
            };

            yield return StartCoroutine(LoadAssetCoroutine(URLS.w2w_server_url + "/bundle", asset.name, callback, onError));
        }

        onSuccess?.Invoke($"{assets.Length} assets successfully initialized.");
    }

    public IEnumerator LoadAssetCoroutine(string bundleURL, string assetName, System.Action<GameObject> onSuccess, System.Action<string> onError)
    {
        AssetBundle asset_bundle = null;

        // Check if AssetBundle is already loaded.
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

        instance.transform.position = position; // Change to your desired location
        instance.transform.eulerAngles = rotation; // Change to your desired location
        instance.transform.localScale = scale; // Change to your desired scale
    }
}