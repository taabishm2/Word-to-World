
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

    public GameObject parentObject;
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
    public void onButtonClick(){
        Debug.Log("Save button clicked.");
        List<SerializedAsset> serializedAssets = new List<SerializedAsset>();

         foreach (Transform child in parentObject.transform)
            {
                SerializedAsset asset = new SerializedAsset(child.gameObject.name, child.position, child.localScale, child.rotation);
                serializedAssets.Add(asset); 
            }
        
        string json = JsonHelper.ToJson(serializedAssets.ToArray());
        Debug.Log($"assets serialized {json}");
        StartCoroutine(SaveCoroutine(json));
    }

    private IEnumerator<object> SaveCoroutine(string json){
        string url = "http://127.0.0.1:5555/get_prompt";
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