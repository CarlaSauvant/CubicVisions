using UnityEngine;

public class DisappearButton : MonoBehaviour
{
    public GameObject button;
    
    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(true);
    }

    public void ButtonDisappear()
    {
        button.SetActive(false);
    }

    public void ButtonAppear()
    {
        button.SetActive(true);
    }

    public void ToggleObject()
    {
        if (button != null)
        {
            if (button.activeSelf)
            {
                button.SetActive(false);
            }

            else
            {
                button.SetActive(true);
            }
        }

        else
        {
            Debug.LogError("Object reference is null. Please assign the object in the Ispector.");
        }
    }

    // Getter method to check if the button is active
    public bool IsButtonActive()
    {
        if (button != null)
        {
            return button.activeSelf;
        }

        else
        {
            return button.activeSelf;
        }
    }
}
