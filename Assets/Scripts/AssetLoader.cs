using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetLoader : MonoBehaviour
{
    public static IEnumerator LoadAssetCoroutine(string bundleURL, string assetName, System.Action<GameObject> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL))
        {
            yield return uwr.SendWebRequest();
           
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download asset bundle: {uwr.error}");
                onError?.Invoke(uwr.error);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);

                if (bundle == null)
                {
                    Debug.LogError("Failed to load AssetBundle!");
                    onError?.Invoke("AssetBundle is NULL!");
                    yield break;
                }

                AssetBundleRequest request = bundle.LoadAssetAsync<GameObject>(assetName);
                yield return request;

                if (request.asset == null)
                {
                    Debug.LogError($"Failed to load asset '{assetName}' from bundle!");
                    onError?.Invoke($"Failed to load asset '{assetName}' from bundle!");
                }
                else
                {
                    onSuccess?.Invoke(request.asset as GameObject);
                    Debug.Log($"Successfully instantiated '{assetName}' from the asset bundle.");
                }

                bundle.Unload(false);
            }
        }
    }
}
