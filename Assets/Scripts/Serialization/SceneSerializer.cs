using System.Collections;
using UnityEngine;

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



}
