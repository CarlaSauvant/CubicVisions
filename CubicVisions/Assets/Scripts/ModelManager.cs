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
        public string valueOld1;
        public string valueOld2;
        public string valueNew1;
        public string valueNew2;
    }

    // List to store placed models
    public List<ModelData> placedModels = new List<ModelData>();

    // Dictionary to store combination outputs based on model types
    public Dictionary<string, string> combinationOutputs = new Dictionary<string, string>();

    // Reference to the CreateGrid script
    public CreateGrid createGridScript; // Add this line

    public GameObject toBeSaved;

    public GameObject audioManager; // Reference to the AudioManager GameObject
    private EpcDictionary epcDictionary;
    private AudioSource placementSound; // Reference to the AudioSource component for placement sound
    
    void Start()
    {
        // Initialize combinationOutputs (you can customize this based on your combinations)
        combinationOutputs.Add("Mobility + Mobility", "NEBourhoodHub");
        combinationOutputs.Add("Mobility + Water", "BikeCleaningStation");
        combinationOutputs.Add("Water + Mobility", "WaterTreadmill");
        combinationOutputs.Add("Play + Play", "BetterPlayground");
        combinationOutputs.Add("Play + Water", "WaterPark");
        combinationOutputs.Add("Water + Play", "KiddiePool");
        combinationOutputs.Add("Mobility + Play", "Skate");
        combinationOutputs.Add("Play + Mobility", "AccessiblePlayground");
        combinationOutputs.Add("Mobility + Bench", "BikeChild");
        combinationOutputs.Add("Mobility + Bush", "GreenBicycleRack");
        combinationOutputs.Add("Mobility + Food", "TransportBike");
        combinationOutputs.Add("Bench + BasketballHoop", "PingPong");
        combinationOutputs.Add("Bench + Food", "PicknickTable");
        combinationOutputs.Add("Bush + Bush", "Tree");
        combinationOutputs.Add("Bush + Water", "Greenhouse");
        combinationOutputs.Add("Water + Food", "WaterFountain");
        combinationOutputs.Add("Play + Food", "PicknickAreaChildren");
        combinationOutputs.Add("BasketballHoop + Mobility", "Skate");
        combinationOutputs.Add("BasketballHoop + Bench", "PingPong");
        combinationOutputs.Add("BasketballHoop + Play", "ClimbingWall");
        combinationOutputs.Add("BasketballHoop + BasketballHoop", "Football");
        combinationOutputs.Add("Food + Mobility", "FoodTruck");
        combinationOutputs.Add("Food + Water", "WaterFountain");
        combinationOutputs.Add("Food + Play", "OutdoorKitchen");      

        // Add more combinations as needed...

        epcDictionary = new EpcDictionary();

        // Add these lines to get the AudioSource component from the AudioManager
        if (audioManager != null)
        {
            placementSound = audioManager.GetComponentInChildren<AudioSource>();
        }
    }

    // Place a model on a tile
    public void PlaceModel(string type, string id, string tileCoordinate, string valueOld1, string valueOld2, string valueNew1, string valueNew2)
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
            // Add a BoxCollider to the instantiated model and automatically adjust its size
            BoxCollider collider = instantiatedModel.AddComponent<BoxCollider>();
            AdjustColliderSize(instantiatedModel, collider);

            ModelData newModel = new ModelData
            {
                type = type,
                id = id,
                tileCoordinate = tileCoordinate,
                rotation = 0,
                valueOld1 = valueOld1,
                valueOld2 = valueOld2,
                valueNew1 = valueNew1,
                valueNew2 = valueNew2
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

            // Set rotation from the new model
            instantiatedModel.transform.rotation = Quaternion.Euler(0, newModel.rotation * 90, 0);

            // default rotation (fbx correction)
            instantiatedModel.transform.Rotate(270f, 0f, 0f);

            Debug.Log("Model placed: " + type + " (" + id + ") on tile " + tileCoordinate);

            // retrieve epcs from the dictionary
            if (string.IsNullOrEmpty(valueNew1) && string.IsNullOrEmpty(valueNew2))
            {
                // Search for valueOld1 in epcDictionary if valueNew1 is empty
                List<string> keysOld1 = epcDictionary.GetKeysByValue(valueOld1);
                if (keysOld1.Count > 0)
                {
                    Debug.Log($"Key retrieved: {string.Join(", ", keysOld1)}");
                }
                else
                {
                    Debug.LogWarning($"Key for valueOld1 ({valueOld1}) not found in the dictionary.");
                }

                // Search for valueOld2 in epcDictionary if valueNew2 is empty
                List<string> keysOld2 = epcDictionary.GetKeysByValue(valueOld2);
                if (keysOld2.Count > 0)
                {
                    Debug.Log($"Key retrieved: {string.Join(", ", keysOld2)}");
                }
                else
                {
                    Debug.LogWarning($"Key for valueOld2 ({valueOld2}) not found in the dictionary.");
                }
            }
            else
            {
                // Retrieve keys for valueNew1 if it's not empty
                if (!string.IsNullOrEmpty(valueNew1))
                {
                    List<string> keysNew1 = epcDictionary.GetKeysByValue(valueNew1);
                    if (keysNew1.Count > 0)
                    {
                        Debug.Log($"Key retrieved: {string.Join(", ", keysNew1)}");
                    }
                    else
                    {
                        Debug.LogWarning($"Key for valueNew1 ({valueNew1}) not found in the dictionary.");
                    }
                }

                // Retrieve keys for valueNew2 if it's not empty
                if (!string.IsNullOrEmpty(valueNew2))
                {
                    List<string> keysNew2 = epcDictionary.GetKeysByValue(valueNew2);
                    if (keysNew2.Count > 0)
                    {
                        Debug.Log($"Key retrieved: {string.Join(", ", keysNew2)}");
                    }
                    else
                    {
                        Debug.LogWarning($"Keys for valueNew2 ({valueNew2}) not found in the dictionary.");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Failed to instantiate model for type '" + type + "'.");
        }
    }

    // Adjust the size of the collider based on the model's bounds
    private void AdjustColliderSize(GameObject model, BoxCollider collider)
    {
        MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // Get the bounds of the mesh
            Bounds bounds = meshRenderer.bounds;

            // Set the size of the BoxCollider based on the model's bounds
            collider.size = bounds.size;
        }
        else
        {
            Debug.LogError("MeshRenderer component not found on the instantiated model.");
        }
    }

     // Remove a model from a tile
    public void RemoveModel(string id, string valueNew1, string valueNew2, string valueOld1, string valueOld2)
    {
        ModelData removedModel = placedModels.Find(model => model.id == id);

        if (removedModel != null)
        {
            // Find the GameObject associated with the model and destroy it
            GameObject modelObject = GameObject.Find(removedModel.id);

            if (modelObject != null)
            {
                Debug.Log("Found GameObject to destroy: " + modelObject.name); // Add this line

            // retrieve epcs from the dictionary
            if (string.IsNullOrEmpty(valueNew1) && string.IsNullOrEmpty(valueNew2))
            {
                // Search for valueOld1 in epcDictionary if valueNew1 is empty
                List<string> keysOld1 = epcDictionary.GetKeysByValue(valueOld1);
                if (keysOld1.Count > 0)
                {
                    Debug.Log($"Blocked key retrieved: {string.Join(", ", keysOld1)}");
                }
                else
                {
                    Debug.LogWarning($"Key for valueOld1 ({valueOld1}) not found in the dictionary.");
                }

                // Search for valueOld2 in epcDictionary if valueNew2 is empty
                List<string> keysOld2 = epcDictionary.GetKeysByValue(valueOld2);
                if (keysOld2.Count > 0)
                {
                    Debug.Log($"Blocked key retrieved: {string.Join(", ", keysOld2)}");
                }
                else
                {
                    Debug.LogWarning($"Key for valueOld2 ({valueOld2}) not found in the dictionary.");
                }
            }
            else
            {
                // Retrieve keys for valueNew1 if it's not empty
                if (!string.IsNullOrEmpty(valueNew1))
                {
                    List<string> keysNew1 = epcDictionary.GetKeysByValue(valueNew1);
                    if (keysNew1.Count > 0)
                    {
                        Debug.Log($"Blocked key retrieved: {string.Join(", ", keysNew1)}");
                    }
                    else
                    {
                        Debug.LogWarning($"Key for valueNew1 ({valueNew1}) not found in the dictionary.");
                    }
                }

                // Retrieve keys for valueNew2 if it's not empty
                if (!string.IsNullOrEmpty(valueNew2))
                {
                    List<string> keysNew2 = epcDictionary.GetKeysByValue(valueNew2);
                    if (keysNew2.Count > 0)
                    {
                        Debug.Log($"Blocked key retrieved: {string.Join(", ", keysNew2)}");
                    }
                    else
                    {
                        Debug.LogWarning($"Keys for valueNew2 ({valueNew2}) not found in the dictionary.");
                    }
                }
            }

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
    public string CombineModels(string type1, string id1, string type2, string id2, string valueNew1, string valueNew2)
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
