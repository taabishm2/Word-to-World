using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class EditorSceneLoader : Editor
{
    [MenuItem("W2W/Load Scene into Editor")]
    static async void LoadAssetFromBundleAsync()
    {
        string assetBundleUrl = "http://localhost:8000/shapes";
        string assetName = "sphere";

        using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundleUrl))
        {
            // Send the request and await its completion
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Delay(100); // Adjust delay as necessary
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load AssetBundle from {assetBundleUrl}");
                return;
            }

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            if (bundle == null)
            {
                Debug.LogError("Failed to download AssetBundle.");
                return;
            }

            GameObject asset = bundle.LoadAsset<GameObject>(assetName);
            if (asset == null)
            {
                Debug.LogError($"Asset '{assetName}' not found in bundle.");
                bundle.Unload(false);
                return;
            }

            GameObject instance = GameObject.Instantiate(asset);
            if (instance == null)
            {
                Debug.LogError("Failed to instantiate asset!");
                bundle.Unload(false);
                return;
            }

            // Set the specific location here
            instance.transform.position = new Vector3(0, 0, 0); // Change to your desired location
            Undo.RegisterCreatedObjectUndo(instance, "Create instance from asset bundle");
            Selection.activeGameObject = instance;

            bundle.Unload(false); // Unload the bundle after use
            Debug.Log($"Asset '{assetName}' successfully instantiated.");
        }
    }
}
