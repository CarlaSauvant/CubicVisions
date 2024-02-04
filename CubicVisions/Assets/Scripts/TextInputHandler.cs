using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TextInputHandler : MonoBehaviour
{
    // Reference to the ModelManager script
    public ModelManager modelManager;

    // Reference to the TMP_InputField
    public TMPro.TMP_InputField inputField1;
    public TMPro.TMP_InputField inputField2;
    public static string modelText;

    private Coroutine inputCoroutine;

    private bool isField1Triggered = false;
    private bool isField2Triggered = false;

    private string valueOld1;
    private string valueOld2;
    private string valueNew1;
    private string valueNew2;

    private void Update()
    {
        CheckForExclamation(inputField1, ref isField1Triggered);
        CheckForExclamation(inputField2, ref isField2Triggered);
    }

    private void CheckForExclamation(TMP_InputField inputField, ref bool isFieldTriggered)
    {
        string inputText = inputField.text;
        
        if (inputText.Contains("!") && !isFieldTriggered)
        {
            isFieldTriggered = true;
            OnSubmitInput();
        }
    }

    // Method to be called when the input is submitted
    public void OnSubmitInput()
    {
        Debug.Log("OnSubmitInput called");

        // Check if both fields have detected '!'
        if (isField1Triggered && isField2Triggered)
        {
            // Get the entered text from the input fields
            string inputText1 = inputField1.text;
            string inputText2 = inputField2.text;

            // Set the additional variables directly
            valueOld1 = inputText1;
            valueOld2 = inputText2;

            // Remove '!' from the input fields
            inputField1.text = RemoveExclamation(inputText1);
            inputField2.text = RemoveExclamation(inputText2);

            // Create the combined input string
            string combinedInput = CombineInputs(inputField1.text, inputField2.text);

            // Parse the input text to extract the coordinate and model information
            if (TryParseInput(combinedInput, out string coordinate, out string type1, out string id1, out string type2, out string id2))
            {
                // Check if it's a model+model case
                if (!IsValidCoordinate(type1) && !IsValidCoordinate(type2))
                {
                    // Set valueNew1 and valueNew2 from the input fields
                    valueNew1 = inputField1.text + "!";
                    valueNew2 = inputField2.text + "!";
                }

                // Place the model prefab on the corresponding tile
                PlaceAndCombineModels(coordinate, type1, id1, type2, id2);

                // Clear the input fields after placing the model
                inputField1.text = "";
                inputField2.text = "";
            }
            else
            {
                // Handle invalid input
                Debug.LogWarning("Invalid input format. Please enter a valid coordinate + model or model + model (e.g., A1 + Cube or Cube_01 + Sphere_03)");
            }

            // Reset trigger flags
            isField1Triggered = false;
            isField2Triggered = false;
        }
    }

    private string RemoveExclamation(string inputText)
    {
        // Remove '!' from the input text
        return inputText.Replace("!", "");
    }

    private IEnumerator WaitForOtherField()
    {
        yield return new WaitForSeconds(10f); // Wait for 10 seconds

        if (isField1Triggered && isField2Triggered)
        {
            // Both fields detected '!' within 10 seconds

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

                // Clear the input fields
                inputField1.text = "";
                inputField2.text = "";
            }
            else
            {
                // Handle invalid input
                Debug.LogWarning("Invalid input format. Please enter a valid coordinate + model or model + model (e.g., A1 + Cube or Cube_01 + Sphere_03)");
            }

            // Reset trigger flags
            isField1Triggered = false;
            isField2Triggered = false;
        }
        else
        {
            // Not both fields detected '!' within 10 seconds
            // Reset trigger flags
            isField1Triggered = false;
            isField2Triggered = false;

            // Display a warning
            Debug.LogWarning("Both fields must detect '!' within 10 seconds.");
        }
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
                modelText = parts[1].Trim();

                // Extract type1, cube_id1, and rotation1 from modelText
                int lastUnderscoreIndex = modelText.LastIndexOf('_');
                if (lastUnderscoreIndex != -1 && lastUnderscoreIndex < modelText.Length - 1)
                {
                    type1 = modelText.Substring(0, lastUnderscoreIndex);
                    string rotationAndBeyond = modelText.Substring(lastUnderscoreIndex + 1);

                    // Extract rotation from the end of the string
                    char rotationChar = rotationAndBeyond[rotationAndBeyond.Length - 1];

                    if (char.IsDigit(rotationChar))
                    {
                        int rotation = int.Parse(rotationChar.ToString());

                        if (rotation >= 1 && rotation <= 4)
                        {
                            id1 = modelText.Substring(0, modelText.Length - 1);
                            Debug.Log("Parsed coordinate: " + coordinate);
                            Debug.Log("Parsed type1: " + type1);
                            Debug.Log("Parsed id1: " + id1);

                            return true;
                        }
                        else
                        {
                            Debug.LogWarning("Invalid rotation format: " + rotationChar);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Invalid rotation format: " + rotationChar);
                    }
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
                            id1 = modelText.Substring(0, modelText.Length - 1);

                            Debug.Log("Parsed type1: " + type1);
                            Debug.Log("Parsed id1: " + id1);
                        }
                        else
                        {
                            type2 = modelTexts[i].Substring(0, lastUnderscoreIndex);
                            id2 = modelText.Substring(0, modelText.Length - 1);

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

        inputField1.text = "";
        inputField2.text = "";

        return false;
    }

    // Check if the given coordinate is valid
    public static bool IsValidCoordinate(string coordinate)
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
            modelManager.PlaceModel(type1, id1, coordinate, valueOld1, valueOld2, "", "");

            Debug.Log("Model placed: " + type1 + " (" + id1 + ") on tile " + coordinate);
        }
        else
        {
            // This is the scenario of combining models (model1 + model2)
            // Check if either the first or the second model is already placed on any tile
            ModelManager.ModelData model1Data = modelManager.placedModels.Find(modelData => modelData.id == id1);
            ModelManager.ModelData model2Data = modelManager.placedModels.Find(modelData => modelData.id == id2);

            if (model1Data != null || model2Data != null)
            {
                // Combine the models and get the output
                string combinedOutput = modelManager.CombineModels(type1, id1, type2, id2, valueNew1, valueNew2);

                if (combinedOutput != null)
                {
                    // Determine the tile coordinate for placing the combined model
                    string targetTileCoordinate = model1Data != null ? model1Data.tileCoordinate : model2Data.tileCoordinate;

                    // Retrieve the existing model data
                    ModelManager.ModelData existingModelData = model1Data ?? model2Data;

                    // Set the valueOld1 and valueOld2 based on the existing model data
                    valueOld1 = existingModelData.valueOld1;
                    valueOld2 = existingModelData.valueOld2;

                    // Place the combined model on the determined tile
                    modelManager.PlaceModel(combinedOutput, combinedOutput, targetTileCoordinate, valueOld1, valueOld2, valueNew1, valueNew2);

                    Debug.Log("Combined model placed: " + combinedOutput + " on tile " + targetTileCoordinate);

                    // Remove the original models from their tiles
                    if (model1Data != null) modelManager.RemoveModel(id1);
                    if (model2Data != null) modelManager.RemoveModel(id2);
                }
                else
                {
                    Debug.LogWarning("Combination not defined for " + type1 + " + " + type2);
                }
            }
            else
            {
                // If neither model1 nor model2 is found, show an error message
                Debug.LogError("Attempted to combine models, but neither model1 (" + type1 + " - " + id1 + ") nor model2 (" + type2 + " - " + id2 + ") is placed on any tile.");

                // Additional handling can be added here if needed
            }
        }
    }
}