using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GameSceneGenerator : MonoBehaviour
{
    string bundleURL = "http://localhost:8000/shapes";
    string assetName = "sphere";

    void Start()
    {
        StartCoroutine(LoadAssetCoroutine());
    }

    IEnumerator LoadAssetCoroutine()
    {
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download asset bundle: {uwr.error}");
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);

                if (bundle == null)
                {
                    Debug.LogError("Failed to load AssetBundle!");
                    yield break;
                }

                AssetBundleRequest request = bundle.LoadAssetAsync<GameObject>(assetName);
                yield return request;

                if (request.asset == null)
                {
                    Debug.LogError($"Failed to load asset '{assetName}' from bundle!");
                }
                else
                {
                    GameObject loadedGameObject = Instantiate(request.asset as GameObject);
                    Debug.Log($"Successfully instantiated '{assetName}' from the asset bundle.");
                }

                bundle.Unload(false);
            }
        }
    }
}
