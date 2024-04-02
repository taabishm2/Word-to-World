using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class EditorSceneLoader : Editor
{
    [MenuItem("W2W/Generate Scene Loader")]
    private const string jsonFilePath = "Assets/SerializedAssets.json";
    static async void LoadAssetFromJSON()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("SerializedAssets.json file not found.");
            return;
        }

        string json = File.ReadAllText(jsonFilePath);
        List<SerializedAsset> serializedAssets = JsonUtility.FromJson<List<SerializedAsset>>(json);

        foreach (SerializedAsset asset in serializedAssets)
        {
            GameObject obj = new GameObject(asset.assetName);
            obj.transform.position = asset.position;
            obj.transform.localScale = asset.scale;
            obj.transform.rotation = asset.rotation;
        }

        Debug.Log("Scene loaded from JSON.");
    }
}
public class SerializedAsset
{
    public string assetName;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;

    public SerializedAsset(string name, Vector3 pos, Vector3 scale, Quaternion rot)
    {
        assetName = name;
        position = pos;
        this.scale = scale;
        rotation = rot;
    }
}
