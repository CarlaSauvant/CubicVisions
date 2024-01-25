using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class TextInputHandler : MonoBehaviour
{
    // Reference to the ModelManager script
    public ModelManager modelManager;

    // Reference to the TMP_InputField
    public TMPro.TMP_InputField inputField1;
    public TMPro.TMP_InputField inputField2;

    // Method to be called when the input is submitted
    public void OnSubmitInput()
    {
        Debug.Log("OnSubmitInput called");

        // Get the entered text from the input fields
        string inputText1 = inputField1.text;
        string inputText2 = inputField2.text;

        // Create the combined input string
        string combinedInput = CombineInputs(inputText1, inputText2);

        // Parse the input text to extract the coordinate and model information
        if (TryParseInput(combinedInput, out string coordinate, out string type1, out string id1, out string type2, out string id2))
        {
            // Place the model prefab on the corresponding tile
            PlaceAndCombineModels(coordinate, type1, id1, type2, id2);
        }
        else
        {
            // Handle invalid input
            Debug.LogWarning("Invalid input format. Please enter a valid coordinate + model or model + model (e.g., A1 + Cube or Cube_01 + Sphere_03)");
        }

        // Clear the input fields
        inputField1.text = "";
        inputField2.text = "";
    }

    private string CombineInputs(string inputText1, string inputText2)
    {
        // Check if one of the inputs is a valid coordinate
        if (IsValidCoordinate(inputText1))
        {
            // If inputText1 is a valid coordinate, it comes first in the combination
            return $"{inputText1} + {inputText2}";
        }
        else if (IsValidCoordinate(inputText2))
        {
            // If inputText2 is a valid coordinate, it comes first in the combination
            return $"{inputText2} + {inputText1}";
        }
        else
        {
            // If neither is a valid coordinate, assume model+model and combine in any order
            return $"{inputText1} + {inputText2}";
        }
    }

    private bool TryParseInput(string inputText, out string coordinate, out string type1, out string id1, out string type2, out string id2)
    {
        coordinate = "";
        type1 = "";
        id1 = "";
        type2 = "";
        id2 = "";

        Debug.Log("Input text: " + inputText);

        // Split the input text by '+'
        string[] parts = inputText.Split('+');

        if (parts.Length == 2)
        {
            // Check if the first part is a valid coordinate
            if (IsValidCoordinate(parts[0].Trim()))
            {
                // Format: "coordinate + model"
                coordinate = parts[0].Trim();
                string modelText = parts[1].Trim();

                // Extract type1 and id1 from modelText
                int lastUnderscoreIndex = modelText.LastIndexOf('_');
                if (lastUnderscoreIndex != -1 && lastUnderscoreIndex < modelText.Length - 1)
                {
                    type1 = modelText.Substring(0, lastUnderscoreIndex);
                    id1 = modelText; // Change this line to include the whole model text

                    Debug.Log("Parsed coordinate: " + coordinate);
                    Debug.Log("Parsed type1: " + type1);
                    Debug.Log("Parsed id1: " + id1);

                    return true;
                }
                else
                {
                    Debug.LogWarning("Invalid model format: " + modelText);
                }
            }
            else
            {
                // Format: "model + model"
                string[] modelTexts = parts.Select(part => part.Trim()).ToArray();

                // Extract type and id from modelTexts
                for (int i = 0; i < modelTexts.Length; i++)
                {
                    int lastUnderscoreIndex = modelTexts[i].LastIndexOf('_');
                    if (lastUnderscoreIndex != -1 && lastUnderscoreIndex < modelTexts[i].Length - 1)
                    {
                        if (i == 0)
                        {
                            type1 = modelTexts[i].Substring(0, lastUnderscoreIndex);
                            id1 = modelTexts[i];

                            Debug.Log("Parsed type1: " + type1);
                            Debug.Log("Parsed id1: " + id1);
                        }
                        else
                        {
                            type2 = modelTexts[i].Substring(0, lastUnderscoreIndex);
                            id2 = modelTexts[i];

                            Debug.Log("Parsed type2: " + type2);
                            Debug.Log("Parsed id2: " + id2);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Invalid model format: " + modelTexts[i]);
                    }
                }

                return true;
            }
        }

        return false;
    }

    // Check if the given coordinate is valid
    private bool IsValidCoordinate(string coordinate)
    {
        HashSet<string> validTileCoordinates = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A10",
            "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10",
            "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10",
            "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10",
            "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "E10",
            "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10",
            "G1", "G2", "G3", "G4", "G5", "G6", "G7", "G8", "G9", "G10",
            "H1", "H2", "H3", "H4", "H5", "H6", "H7", "H8", "H9", "H10",
            "J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9", "J10",
            "K1", "K2", "K3", "K4", "K5", "K6", "K7", "K8", "K9", "K10"
            // Add the rest of the valid tile coordinates...
        };

        return validTileCoordinates.Contains(coordinate);
    }

    // Place a model on a tile and handle combination
    private void PlaceAndCombineModels(string coordinate, string type1, string id1, string type2, string id2)
    {
        // Define a set of valid tile coordinates
        HashSet<string> validTileCoordinates = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A10",
            "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10",
            "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10",
            "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10",
            "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "E10",
            "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10",
            "G1", "G2", "G3", "G4", "G5", "G6", "G7", "G8", "G9", "G10",
            "H1", "H2", "H3", "H4", "H5", "H6", "H7", "H8", "H9", "H10",
            "J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9", "J10",
            "K1", "K2", "K3", "K4", "K5", "K6", "K7", "K8", "K9", "K10"
            // Add the rest of the valid tile coordinates...
        };

        // Check if the input is in the form "coordinate + model1"
        if (validTileCoordinates.Contains(coordinate))
        {
            // This is the scenario of placing a model on a specified coordinate
            modelManager.PlaceModel(type1, id1, coordinate);

            Debug.Log("Model placed: " + type1 + " (" + id1 + ") on tile " + coordinate);
        }
        else
        {
            // This is the scenario of combining models (model1 + model2)
            // Check if the first model is already placed on any tile
            ModelManager.ModelData model1Data = modelManager.placedModels.Find(modelData => modelData.id == id1);

            if (model1Data != null)
            {
                // Combine the models and get the output
                string combinedOutput = modelManager.CombineModels(type1, id1, type2, id2);

                if (combinedOutput != null)
                {
                    // Place the combined model on the same tile
                    modelManager.PlaceModel(combinedOutput, combinedOutput, model1Data.tileCoordinate);

                    Debug.Log("Combined model placed: " + combinedOutput + " on tile " + model1Data.tileCoordinate);
                }
                else
                {
                    Debug.LogWarning("Combination not defined for " + type1 + " + " + type2);
                }

                // Remove the original models from their tiles
                modelManager.RemoveModel(id1);
                modelManager.RemoveModel(id2);
            }
            else
            {
                // If model1 is not found, show an error message
                Debug.LogError("Attempted to combine models, but model1 (" + type1 + " - " + id1 + ") is not placed on any tile.");

                // Additional handling can be added here if needed
            }
        }
    }
}