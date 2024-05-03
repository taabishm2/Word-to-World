using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkManager : MonoBehaviour
{

    public TextMeshProUGUI voiceText;

    public IEnumerator SendDataToServer(string json)
    {
        voiceText.color = Color.yellow;
        voiceText.text += "\nSaving Scene on Server...";
        var request = new UnityWebRequest(URLS.w2w_server_url + "/save", "POST");
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
