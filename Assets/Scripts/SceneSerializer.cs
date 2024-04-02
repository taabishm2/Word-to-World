using System.Collections;
using UnityEngine;
using System.IO;


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
public class SceneSerializer : MonoBehaviour {

    // TODO

    /**
    
    1.
    Create a Class (say SerializedAsset) to store properties of an object
    Fields:
        - asset name
        - asset position on scene
        - asset scale on scene
        - asset rotation on scene
        - etc

    Methods:
        - constructor to initialize the fields

    
    2.
    Create a function which accepts a list of 'SerializedAsset' objects (representing all objects in the scene)
    and serializes them to JSON -- this should be triggerred automatically when the user exits the game mode (if that
    isn't possible, then create an in-game button that triggers this function manually)

    3. 
    Create a function which reads a serialized JSON file and converts it to a list of 'SerializedAsset' objects
    Then, it should instantiate all objects in the scene based on the 'SerializedAsset' objects fields
    
    How to do this in the Unity Editor (not in game mode) - refer to the EditorSceneLoader.cs script 
    For scripts that insert objects in editor mode, scripts must be placed in the Editor folder
    
    Also, if a function in the Editor scripts has "[MenuItem("Test123/Generate my scene")]" in the beginning, Unity editor
    will have this function in the menu bar
    **/

    private const string jsonFilePath = "Assets/SerializedAssets.json";
    public static void serializeToJSON(){
        Debug.Log("Save button clicked.");
        List<SerializedAsset> serializedAssets = new List<SerializedAsset>();
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in objects)
        {
            SerializedAsset asset = new SerializedAsset(obj.name, obj.transform.position, obj.transform.localScale, obj.transform.rotation);
            serializedAssets.Add(asset);
        }
        string json = JsonUtility.ToJson(serializedAssets);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log("Scene serialized to JSON.");
    }

    public Button saveButton; 

    void Start()
    {
        Button btn = saveButton.GetComponent<Button>();
        btn.onClick.AddListener(serializeToJSON);
    }


}