using UnityEngine;

public class LoadBundle : MonoBehaviour
{
    // Path to the AssetBundle file
    public string assetBundlePath = "Assets/Build AssetBundles/abc";
    // Name of the object to be loaded from the AssetBundle
    public string objectToLoad = "armchair";

    void Start()
    {
        LoadAsset(assetBundlePath);
    }

    void LoadAsset(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("AssetBundle path is not set");
            return;
        }

        // Load the AssetBundle from file
        AssetBundle loadedBundle = AssetBundle.LoadFromFile(path);
        if (loadedBundle == null)
        {
            Debug.LogError("Failed to load AB from path: " + path);
            return;
        }

        // Load the object from the AssetBundle
        GameObject prefab = loadedBundle.LoadAsset<GameObject>(objectToLoad);
        if (prefab == null)
        {
            Debug.LogError("Failed to load object from AssetBundle: " + objectToLoad);
            loadedBundle.Unload(false);
            return;
        }

        // Instantiate the object at the origin of the scene
        Instantiate(prefab, Vector3.zero, Quaternion.identity);

        // Unload the AssetBundle (do not unload the loaded assets)
        loadedBundle.Unload(false);
    }
}
