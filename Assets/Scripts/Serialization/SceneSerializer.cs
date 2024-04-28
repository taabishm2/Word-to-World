
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;


public class SceneSerializer : MonoBehaviour {
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
    private const string jsonFilePath = "Assets/SerializedAssets.json";
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

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
    public static IEnumerator<object> onButtonClick(){
        Debug.Log("Save button clicked.");
        List<SerializedAsset> serializedAssets = new List<SerializedAsset>();
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        // need to save objects in the parent folder
        foreach (GameObject obj in objects)
        {
            SerializedAsset asset = new SerializedAsset(obj.name, obj.transform.position, obj.transform.localScale, obj.transform.rotation);
            serializedAssets.Add(asset);
        }
        string json = JsonHelper.ToJson(serializedAssets.ToArray());
        string url = "http://127.0.0.1:5555/process_data";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, json);
        request.SetRequestHeader("Content-Type", "application/json");
         // Prepare buffer for response.
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }



}