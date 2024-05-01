using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class SerializableGameObject
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public List<SerializableGameObject> children;

    public static SerializableGameObject Serialize(GameObject obj)
    {
        SerializableGameObject sgo = new SerializableGameObject
        {
            name = obj.name,
            position = obj.transform.position,
            rotation = obj.transform.rotation,
            scale = obj.transform.localScale,
            children = new List<SerializableGameObject>()
        };

        // Ensure to iterate correctly over all children
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            GameObject child = obj.transform.GetChild(i).gameObject;
            sgo.children.Add(Serialize(child));
        }

        return sgo;
    }
    
}
