using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class NetworkManager : MonoBehaviour
{

    public string serverUrl = URLS.w2w_server_url + "/save";

    public IEnumerator SendDataToServer(string json)
    {
        var request = new UnityWebRequest(serverUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

}
