using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    public float transparencyPercentage = 100f;  // Editable in the inspector between 2% and 100%
    public GameObject targetObject;  // Assign the GameObject you want to make transparent
    public DisappearButton uiButton; // Reference to the script controlling UI button

    void Update()
    {
        // Check if the UI button is active
        bool isUIButtonActive = uiButton != null && uiButton.IsButtonActive();

        // Set the transparency of the target object
        SetObjectTransparency(isUIButtonActive ? transparencyPercentage : 100f);
    }

    void SetObjectTransparency(float percentage)
    {
        // Ensure targetObject and its renderer are assigned
        if (targetObject != null)
        {
            Renderer renderer = targetObject.GetComponent<Renderer>();

            // Ensure the renderer exists and has materials
            if (renderer != null && renderer.materials != null)
            {
                // Loop through all materials of the object
                foreach (Material material in renderer.materials)
                {
                    // Calculate alpha value based on percentage
                    float alpha = percentage / 100f;
                    Color color = material.color;
                    color.a = alpha;
                    material.color = color;
                }
            }
            else
            {
                Debug.LogWarning("Renderer or materials not found on the target object.");
            }
        }
        else
        {
            Debug.LogWarning("Target object not assigned.");
        }
    }
}