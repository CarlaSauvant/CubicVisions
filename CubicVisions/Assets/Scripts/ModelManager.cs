using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelManager : MonoBehaviour
{
    // Define a class to store model information
    [System.Serializable]
    public class ModelData
    {
        public string type; // e.g., "Cube", "Sphere", "Cylinder"
        public string id;   // e.g., "Cube_01", "Sphere_03"
        public string tileCoordinate;
        public int rotation;
    }

    // List to store placed models
    public List<ModelData> placedModels = new List<ModelData>();

    // Dictionary to store combination outputs based on model types
    public Dictionary<string, string> combinationOutputs = new Dictionary<string, string>();

    // Reference to the CreateGrid script
    public CreateGrid createGridScript; // Add this line

    public GameObject toBeSaved;

    public GameObject audioManager; // Reference to the AudioManager GameObject
    private AudioSource placementSound; // Reference to the AudioSource component for placement sound
    
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

        // Add these lines to get the AudioSource component from the AudioManager
        if (audioManager != null)
        {
            placementSound = audioManager.GetComponentInChildren<AudioSource>();
        }
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
                tileCoordinate = tileCoordinate,
                rotation = 0
            };

            placedModels.Add(newModel);

            // Extract rotation number from input text
            if (!string.IsNullOrEmpty(TextInputHandler.modelText) && char.IsDigit(TextInputHandler.modelText.Last()))
            {
                // Set rotation based on the extracted rotation number
                newModel.rotation = int.Parse(TextInputHandler.modelText.Last().ToString());
                instantiatedModel.transform.rotation = Quaternion.Euler(0, newModel.rotation * 90, 0);
            }

            // Play the placement sound
            if (placementSound != null)
            {
                placementSound.Play();
            }

            // default rotation (fbx correction)
            instantiatedModel.transform.Rotate(270f, 0f, 0f);

            Debug.Log("Model placed: " + type + " (" + id + ") on tile " + tileCoordinate);
        }
        else
        {
            Debug.LogError("Failed to instantiate model for type '" + type + "'.");
        }
    }

    // Place Water model with specific logic for neighboring Water models
    public void PlaceWaterModel(string type, string id, string tileCoordinate)
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

        // Check neighboring tiles for Water elements
        int waterNeighbourCount = CountWaterNeighbours(tileCoordinate);

        // Determine the specific Water model based on the number of Water neighbors
        string specificWaterModel = GetSpecificWaterModel(waterNeighbourCount);

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

            // Extract rotation number from input text
            if (!string.IsNullOrEmpty(TextInputHandler.modelText) && char.IsDigit(TextInputHandler.modelText.Last()))
            {
                // Set rotation based on the extracted rotation number
                newModel.rotation = int.Parse(TextInputHandler.modelText.Last().ToString());
                instantiatedModel.transform.rotation = Quaternion.Euler(0, newModel.rotation * 90, 0);
            }

            // Play the placement sound
            if (placementSound != null)
            {
                placementSound.Play();
            }

            // Default rotation (fbx correction)
            instantiatedModel.transform.Rotate(270f, 0f, 0f);

            Debug.Log("Model placed: " + type + " (" + id + ") on tile " + tileCoordinate +
                    " with " + waterNeighbourCount + " Water neighbors");

            // Additional logic for specific Water models
            if (specificWaterModel != null)
            {
                // Load the specific Water model prefab and replace the instantiated model
                GameObject specificWaterPrefab = Resources.Load<GameObject>("Prefabs/Toolkit/" + specificWaterModel);

                if (specificWaterPrefab != null)
                {
                    Destroy(instantiatedModel); // Destroy the generic Water model
                    instantiatedModel = Instantiate(specificWaterPrefab, targetTile.transform.position, Quaternion.identity);
                    instantiatedModel.name = id; // Set the name to match the model's id
                    instantiatedModel.transform.SetParent(toBeSaved.transform);

                    Debug.Log("Replaced with specific Water model: " + specificWaterModel);
                }
                else
                {
                    Debug.LogError("Prefab for specific Water model '" + specificWaterModel + "' not found.");
                }
            }
        }
        else
        {
            Debug.LogError("Failed to instantiate model for type '" + type + "'.");
        }
    }

    // Count Water neighbors for a given tile coordinate
    private int CountWaterNeighbours(string tileCoordinate)
    {
        // Define the coordinates of direct neighbors for a given tile
        string[] neighborOffsets = { "0,1", "1,0", "0,-1", "-1,0" };

        // Extract row and column indices from the given coordinate
        int row, col;
        ConvertCoordinateToIndices(tileCoordinate, out row, out col);

        // Count the number of Water neighbors
        int waterNeighbourCount = 0;

        foreach (var offset in neighborOffsets)
        {
            string[] offsetParts = offset.Split(',');
            int rowOffset = int.Parse(offsetParts[0]);
            int colOffset = int.Parse(offsetParts[1]);

            int neighborRow = row + rowOffset;
            int neighborCol = col + colOffset;

            // Convert indices back to coordinate
            string neighborCoordinate = ConvertIndicesToCoordinate(neighborRow, neighborCol);

            Debug.Log($"Checking neighbour: {neighborCoordinate}");

            // Check if the neighbor is a valid tile coordinate
            if (TextInputHandler.IsValidCoordinate(neighborCoordinate))
            {
                // Check if there is a Water model on the neighboring tile
                if (placedModels.Any(model => model.type.StartsWith("Water") && model.tileCoordinate.Equals(neighborCoordinate)))
                {
                    waterNeighbourCount++;
                }
                else
                {
                    Debug.Log("No water on the neighbouring tiles.");
                }
            }
        }

        return waterNeighbourCount;
    }

    // Get specific Water model based on the number of Water neighbors
    private string GetSpecificWaterModel(int waterNeighbourCount)
    {
        // Determine the specific Water model based on the number of Water neighbors
        switch (waterNeighbourCount)
        {
            case 0:
                return "Water";
            case 1:
                return "WaterOne";
            case 2:
                return "WaterTwo";
            case 3:
                return "WaterThree";
            case 4:
                return "WaterFour";
            default:
                return null; // Handle other cases as needed
        }
    }

    // Helper method to convert tile coordinate to indices
    private void ConvertCoordinateToIndices(string coordinate, out int row, out int col)
    {
        row = coordinate[0] - 'A';
        col = int.Parse(coordinate.Substring(1)) - 1;
    }

    // Helper method to convert indices to tile coordinate
    private string ConvertIndicesToCoordinate(int row, int col)
    {
        return $"{(char)('A' + row)}{col + 1}";
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
        // Check if it's a coordinate+model combination
        bool isCoordinateModelCombination = TextInputHandler.IsValidCoordinate(type1);

        string outputType = ""; // Initialize outputType with an empty string

        Debug.Log($"CombineModels called with type1: {type1}, id1: {id1}, type2: {type2}, id2: {id2}");

        if (isCoordinateModelCombination)
        {
            // Coordinate + Model combination
            outputType = $"{type1} + {type2}"; // Always place coordinate first, then model

            Debug.Log($"Combination keys present: {string.Join(", ", combinationOutputs.Keys)}");
        }
        else
        {
            // Model + Model combination
            string combinationKey1 = $"{type1} + {type2}";
            string combinationKey2 = $"{type2} + {type1}";

            // Check if either combination is valid
            bool isId1Exists = placedModels.Exists(model => model.id == id1);
            bool isId2Exists = placedModels.Exists(model => model.id == id2);

            if (isId1Exists && combinationOutputs.TryGetValue(combinationKey1, out outputType))
            {             
                // Combination found, return the output type
                return outputType;
            }
            else if (isId2Exists && combinationOutputs.TryGetValue(combinationKey2, out outputType))
            {
                // Combination found, return the output type
                return outputType;
            }

            Debug.LogError($"Combination not defined for {type1} + {type2}");
            return null; // Return null or handle the error accordingly
        }

        return outputType; // Return the actual outputType
    }
}