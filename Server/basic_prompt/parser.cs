using UnityEngine;
using System;

public class parser : MonoBehaviour
{
    [Serializable]
    public class Asset
    {
        public string type;
        public string size;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    [Serializable]
    public class AssetList
    {
        public Asset[] assets;
    }

    public GameObject cubePrefab;
    public GameObject circlePrefab;
    public GameObject trianglePrefab;

    private void Start()
    {
        // Load the JSON from the file
        TextAsset textAsset = Resources.Load<TextAsset>("IceCreamAssets");
        string json = textAsset.text;

        CreateIceCreamFromJson(json);
    }

    void CreateIceCreamFromJson(string jsonString)
    {
        AssetList assetList = JsonUtility.FromJson<AssetList>(jsonString);

        foreach (Asset asset in assetList.assets)
        {
            GameObject prefab = null;

            switch (asset.type)
            {
                case "Cube":
                    prefab = cubePrefab;
                    break;
                case "Circle":
                    prefab = circlePrefab;
                    break;
                case "Triangle":
                    prefab = trianglePrefab;
                    break;
            }

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, asset.position, Quaternion.Euler(asset.rotation));
                obj.transform.localScale = asset.scale;
            }
        }
    }
}
