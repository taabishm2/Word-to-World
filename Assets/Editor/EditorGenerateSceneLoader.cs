using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
public GameObject parentObject;
public class EditorGenerateSceneLoader : Editor
{
    public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
    
    private const string jsonFilePath = "Assets/SerializedAssets.json";
    [MenuItem("W2W/Generate Scene Loader")]
    static void LoadAssetFromJSON()
    {
        StartCoroutine(LoadAssets())
    }

    static IEnumerator<object> LoadAssets(){
        string url = "http://127.0.0.1:5555/serve_save";
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            APIResponse response = JsonUtility.FromJson<APIResponse>(jsonResponse);
            Debug.Log("Response: " + request.downloadHandler.text);
            yield return StartCoroutine(SpawnAssets(response.assets));
        }

    }
        private IEnumerator SpawnAssets(AssetData[] assets) {
        // Debug.Log("Spawning number of objects: " + assets.Length);
    
        foreach (AssetData asset in assets) {
            // Use a lambda expression to create a delegate with fixed position and rotation.
            System.Action<GameObject> callback = (loadedGameObject) => {
                OnAssetLoaded(loadedGameObject, asset.position, asset.rotation, asset.scale);
            };

            // Debug.Log("Spawning object from " + asset.bundle_url + " name " + asset.name);

            //TODO: Parallelize this if possible
            // TODO: Parameterize the URL
            yield return StartCoroutine(LoadAssetCoroutine(asset.name, callback));
        }

        onSuccess?.Invoke($"{assets.Length} assets successfully initialized.");
    }

    public IEnumerator LoadAssetCoroutine(string assetName)
        AssetBundle asset_bundle = null;

        // Check if AssetBundle is already loaded.
        
        // Get all loaded AssetBundles
        AssetBundle[] loadedAssetBundles = AssetBundle.GetAllLoadedAssetBundles().ToArray();

         // Iterate through the loaded AssetBundles and check if the URL matches

        asset_bundle = bundle; 
         

        if (asset_bundle == null) {
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to download asset bundle: {uwr.error} {bundleURL}");
                }
                else
                {
                    asset_bundle = DownloadHandlerAssetBundle.GetContent(uwr);

                    if (asset_bundle == null)
                    {
                        Debug.LogError("Failed to load AssetBundle!");
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
        }
        else
        {
            
            Debug.Log($"Successfully instantiated '{assetName}' from the asset bundle.");
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

[System.Serializable]
    public class AssetData
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

     public class APIResponse
    {
        public AssetData[] assets;
    }

