using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;

public class GameObjectLoaderEditor : EditorWindow
{
    private static string fetchSceneUrl = URLS.w2w_server_url + "/fetch";
    private static string bundleUrl = URLS.w2w_server_url + "/bundle";

    [MenuItem("W2W/Load Saved Scene")]
    public static void LoadScene()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(CallAPI());
    }

    private static IEnumerator CallAPI()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(fetchSceneUrl))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                APIResponse response = JsonUtility.FromJson<APIResponse>(webRequest.downloadHandler.text);
                yield return EditorCoroutineUtility.StartCoroutineOwnerless(SpawnAssets(response.assets));
            }
        }
    }

    private static IEnumerator SpawnAssets(AssetData[] assets)
    {
        GameObject parentObject = GameObject.Find("AssetsParent");
        if (parentObject == null)
        {
            Debug.LogError("AssetsParent GameObject not found in the scene!");
            yield return null;
        }

        foreach (AssetData asset in assets)
        {
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(LoadAndInstantiateAsset(asset, parentObject));
        }
    }

    private static IEnumerator LoadAndInstantiateAsset(AssetData asset, GameObject parent)
    {
        AssetBundle assetBundle = null;
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download asset bundle: {uwr.error} {bundleUrl}");
            }
            else
            {
                assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
                if (assetBundle == null)
                {
                    Debug.LogError("Failed to load AssetBundle!");
                    yield break;
                }

                GameObject prefab = assetBundle.LoadAsset<GameObject>(asset.name);
                if (prefab == null)
                {
                    Debug.LogError($"Asset {asset.name} not found in AssetBundle.");
                    assetBundle.Unload(true);
                    yield break;
                }

                GameObject instance = GameObject.Instantiate(prefab, parent.transform);

                Renderer renderer = instance.GetComponent<Renderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    // Set the Universal Render Pipeline Lit shader
                    Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
                    if (urpLitShader != null)
                    {
                        renderer.sharedMaterial.shader = urpLitShader;
                    }
                }

                instance.transform.localPosition = new Vector3(asset.position.x, asset.position.y, asset.position.z);
                instance.transform.localRotation = Quaternion.Euler(asset.rotation.x, asset.rotation.y, asset.rotation.z);
                instance.transform.localScale = new Vector3(asset.scale.x, asset.scale.y, asset.scale.z);

                assetBundle.Unload(false);  // Unload the AssetBundle but keep the loaded objects
            }
        }
    }

    [System.Serializable]
    public class APIResponse
    {
        public AssetData[] assets;
    }

    [System.Serializable]
    public class AssetData
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

}
