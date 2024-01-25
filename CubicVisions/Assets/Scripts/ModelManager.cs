using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    // Define a class to store model information
    [System.Serializable]
    public class ModelData
    {
        public string type; // e.g., "Cube", "Sphere", "Cylinder"
        public string id;   // e.g., "Cube_01", "Sphere_03"
        public string tileCoordinate;
    }

    // List to store placed models
    public List<ModelData> placedModels = new List<ModelData>();

    // Dictionary to store combination outputs based on model types
    public Dictionary<string, string> combinationOutputs = new Dictionary<string, string>();

    // Reference to the CreateGrid script
    public CreateGrid createGridScript; // Add this line

    public GameObject toBeSaved;

    void Start()
    {
        // Initialize combinationOutputs (you can customize this based on your combinations)
        combinationOutputs.Add("Cube + Sphere", "Block");
        combinationOutputs.Add("Sphere + Cube", "Block");
        combinationOutputs.Add("Sphere + Cylinder", "Capsule");
        combinationOutputs.Add("Cylinder + Sphere", "Capsule");
        combinationOutputs.Add("Cylinder + Cube", "Cylinder_flat");
        combinationOutputs.Add("Cube + Cylinder", "Cylinder_flat");

        combinationOutputs.Add("Mobility + Mobility", "NEBourhoodHub");
        combinationOutputs.Add("Mobility + Bush", "GreenBicycleRack");
        combinationOutputs.Add("Bench + Bush", "PicknickArea");
        combinationOutputs.Add("Bench + Sports", "PingPong");
        combinationOutputs.Add("Bench + Food", "GreenBicycleRack");
        combinationOutputs.Add("Bush + Bush", "Tree");
        combinationOutputs.Add("Bush + Water", "Greenhouse");
        combinationOutputs.Add("Water + Food", "WaterFountain");
        combinationOutputs.Add("Water + Water", "Pond");
        combinationOutputs.Add("Sports + Bench", "PingPong");
        combinationOutputs.Add("Sports + Play", "ClimbingWall");
        combinationOutputs.Add("Sports + Sports", "Footbal");
        combinationOutputs.Add("Food + Mobility", "FoodTruck");
        combinationOutputs.Add("Food + Bench", "PicknickTable");
        combinationOutputs.Add("Food + Water", "WaterFountain");
        combinationOutputs.Add("Food + Play", "OutdoorKitchen");

        // Add more combinations as needed...
    }

    // Place a model on a tile
    public void PlaceModel(string type, string id, string tileCoordinate)
    {
        // Check if the prefab for the model exists in Resources
        GameObject modelPrefab = Resources.Load<GameObject>("Prefabs/Toolkit/" + type);
        if (modelPrefab == null)
        {
            Debug.LogError("Prefab for model type '" + type + "' not found in Resources/Prefabs/Toolkit folder.");
            return;
        }

        // Check if the tile with the specified coordinate exists in the CreateGrid tile list
        CreateGrid.TileData targetTile = createGridScript.GetTileList().Find(tileData => tileData.name.Equals(tileCoordinate, System.StringComparison.OrdinalIgnoreCase));
        if (targetTile == null)
        {
            Debug.LogError("Tile with coordinate '" + tileCoordinate + "' not found in the tile list.");
            return;
        }

        // Instantiate the model prefab and place it on the tile's position
        GameObject instantiatedModel = Instantiate(modelPrefab, targetTile.transform.position, Quaternion.identity);
        instantiatedModel.name = id; // Set the name to match the model's id

        // Set the toBeSaved game object as the parent of the instantiated models.
        instantiatedModel.transform.SetParent(toBeSaved.transform);

        // Check if the instantiation was successful
        if (instantiatedModel != null)
        {
            ModelData newModel = new ModelData
            {
                type = type,
                id = id,
                tileCoordinate = tileCoordinate
            };

            placedModels.Add(newModel);

            Debug.Log("Model placed: " + type + " (" + id + ") on tile " + tileCoordinate);
        }
        else
        {
            Debug.LogError("Failed to instantiate model for type '" + type + "'.");
        }
    }

    // Remove a model from a tile
    public void RemoveModel(string id)
    {
        ModelData removedModel = placedModels.Find(model => model.id == id);

        if (removedModel != null)
        {
            // Find the GameObject associated with the model and destroy it
            GameObject modelObject = GameObject.Find(removedModel.id);

            if (modelObject != null)
            {
                Debug.Log("Found GameObject to destroy: " + modelObject.name); // Add this line
                Destroy(modelObject);
                Debug.Log("Model removed from the scene: " + removedModel.type + " (" + removedModel.id + ")");
            }
            else
            {
                Debug.LogError("Failed to find the GameObject associated with model id: " + id);
            }

            placedModels.Remove(removedModel); // Remove from the list
        }
        else
        {
            // Debug.LogError("Model with id " + id + " not found in placedModels list.");
        }
    }

    // Combine two models and get the output
    public string CombineModels(string type1, string id1, string type2, string id2)
    {
        // Define the valid logical combinations
        string logicalCombinationKey1 = type1 + " + " + type2;
        string logicalCombinationKey2 = type2 + " + " + type1;

        Debug.Log("Logical Combination Key 1: " + logicalCombinationKey1);
        Debug.Log("Logical Combination Key 2: " + logicalCombinationKey2);

        string outputType1, outputType2 = ""; // Initialize outputType2 with an empty string

        if (combinationOutputs.TryGetValue(logicalCombinationKey1, out outputType1) ||
            combinationOutputs.TryGetValue(logicalCombinationKey2, out outputType2))
        {
            string combinedId = "Combined_" + id1 + "_" + id2;
            string combinedPrefabPath = $"Assets/Resources/Prefabs/Toolkit/Combinations/{outputType1 ?? outputType2}";

            // Check if the prefab for the combined model exists in Resources/Combinations
            GameObject combinedModelPrefab = Resources.Load<GameObject>(combinedPrefabPath);
            if (combinedModelPrefab == null)
            {
                Debug.LogError($"Prefab for combined model type '{outputType1 ?? outputType2}' not found in {combinedPrefabPath} folder.");
                return null;
            }

            return outputType1 != null ? outputType1 : outputType2; // Use the first non-null output type
        }
        else
        {
            Debug.LogError("Logical Combination not found for keys: " + logicalCombinationKey1 + " and " + logicalCombinationKey2);
        }

        return null; // Combination not defined
    }
}