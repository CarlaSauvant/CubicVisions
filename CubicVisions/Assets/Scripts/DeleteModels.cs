using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteModels : MonoBehaviour
{
    public TMP_InputField inputField1;
    public TMP_InputField inputField2;
    private ModelManager modelManager;

    void Start()
    {
        // Attempt to get the ModelManager component on the same GameObject
        modelManager = GetComponent<ModelManager>();

        // Check if the ModelManager component was found
        if (modelManager == null)
        {
            Debug.LogError("ModelManager component not found. Make sure it's attached to the same GameObject.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Assuming left mouse button is used
        {
            DeleteOneModel();
        }
    }

    private void DeleteOneModel()
    {
        // Check if the modelManager is null before using it
        if (modelManager == null)
        {
            Debug.LogError("ModelManager is null. Make sure it's assigned in the Unity Editor or attached to the same GameObject.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Object hit with raycast.");

            // Check if the clicked object is a model
            GameObject clickedModel = hit.collider.gameObject;

            // Check if the clicked model is in the PlacedModels list
            ModelManager.ModelData clickedModelData = modelManager.placedModels.Find(model => model.id == clickedModel.name);

            if (clickedModelData != null)
            {
                // Remove the model from the scene and the list only if it's a child of the toBeSaved game object
                if (clickedModel.transform.parent == modelManager.toBeSaved.transform)
                {
                    // Check if it's a combinationModel or basicModel
                    if (!string.IsNullOrEmpty(clickedModelData.valueNew1) && !string.IsNullOrEmpty(clickedModelData.valueNew2))
                    {
                        // Case combinationModel: valueNew1 and valueNew2 are filled in and not empty
                        // Retrieve valueOld1 and valueOld2 before deleting the model
                        string valueOld1 = clickedModelData.valueOld1;
                        string valueOld2 = clickedModelData.valueOld2;

                        // Remove the model from the scene and the list
                        modelManager.RemoveModel(clickedModelData.id);

                        // Update the TextMeshPro text fields with valueOld1 and valueOld2
                        if (inputField1 != null)
                        {
                            inputField1.text = valueOld1;
                        }

                        if (inputField2 != null)
                        {
                            inputField2.text = valueOld2;
                        }
                    }
                    else
                    {
                        // Case basicModel: valueNew1 and valueNew2 are empty
                        // Remove the model from the scene and the list
                        modelManager.RemoveModel(clickedModelData.id);
                    }
                }
                else
                {
                    Debug.LogWarning("Clicked model is not a child of the toBeSaved game object.");
                }
            }
        }
        else
        {
            Debug.LogError("No object found to destroy with Raycast.");
        }
    }
}