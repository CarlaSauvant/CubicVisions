using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteModels : MonoBehaviour
{
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
                    modelManager.RemoveModel(clickedModelData.id);
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