using UnityEngine;
using System.IO;

public class Screenshot : MonoBehaviour
{
    public GameObject uiObject; // Reference to the UI GameObject
    public GameObject selectionGrid; // Reference to the selection grid

    // Your custom button click method
    public void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine());
    }

    private System.Collections.IEnumerator CaptureScreenshotCoroutine()
    {
        // Hide UI elements if needed
        ToggleUIElements(false);

        // Wait for the next frame to ensure UI elements are hidden
        yield return null;

        // Capture the screenshot
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string screenshotName = "CubicVisions_" + timestamp + ".png";
        string screenshotPath = Path.Combine(Application.dataPath, "..", screenshotName);

        ScreenCapture.CaptureScreenshot(screenshotPath);

        // Wait for the screenshot to be saved
        yield return new WaitForSeconds(0.5f); // Adjust the delay if needed

        // Show UI elements again
        ToggleUIElements(true);
    }

    private void ToggleUIElements(bool show)
    {
        // Check if the UI GameObject is assigned
        if (uiObject != null && selectionGrid != null)
        {
            // Set the UI GameObject and its children to be active or inactive based on the 'show' parameter
            uiObject.SetActive(show);
            selectionGrid.SetActive(show);
        }
        else
        {
            Debug.LogWarning("UI GameObject and selection grid are not assigned. Please assign them in the inspector.");
        }
    }
}