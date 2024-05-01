using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine.Networking;


public class Serializer : MonoBehaviour
{
    public Button serializeButton;
    public NetworkManager networkManager;
    public GameObject targetGameObject; 

    void Start()
    {
        if (serializeButton != null)
        {
            serializeButton.onClick.AddListener(TaskOnClick);
        } else {
            Debug.Log("Button is null");
        }
    }
    
    void TaskOnClick()
    {
        Debug.Log("Button Clicked");
        Debug.Log("Serializing " + targetGameObject.name + " with " + targetGameObject.transform.childCount + " children.");
        string serializedObject = SerializeGameObject(targetGameObject);
        Debug.Log("Serialized Object: " + serializedObject);
        StartCoroutine(networkManager.SendDataToServer(serializedObject));
    }

    public string SerializeGameObject(GameObject obj)
    {
        SerializableGameObject sgo = SerializableGameObject.Serialize(obj);
        return JsonUtility.ToJson(sgo);
    }

}
