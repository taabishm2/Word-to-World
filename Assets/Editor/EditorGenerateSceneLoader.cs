using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

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
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("SerializedAssets.json file not found.");
            return;
        }

        string json = File.ReadAllText(jsonFilePath);
        Debug.Log(json);
        SerializedAsset[] serializedAssets = JsonHelper.FromJson<SerializedAsset>(json);
        Debug.Log(serializedAssets[0].assetName);
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
[System.Serializable]
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
