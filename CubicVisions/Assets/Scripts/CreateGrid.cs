using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGrid : MonoBehaviour
{
    // Define a class to store tile information
    [System.Serializable]
    public class TileData
    {
        public string name;
        public Transform transform;
    }

    // List to store tile information
    public List<TileData> tileList = new List<TileData>();

    // Flag to check if the tile list has been populated
    private bool isTileListPopulated = false;

    void Start()
    {
        // Assuming you have a UI button attached to the script
        Button enterButton = GetComponent<Button>();

        // Attach a method to the button click event
        if (enterButton != null)
        {
            enterButton.onClick.AddListener(MakeList);
        }
    }

    public void MakeList()
    {
        // Check if the tile list has already been populated
        if (!isTileListPopulated)
        {
            // Populate the tile list
            PopulateTileList();

            // Set the flag to true to indicate that the list has been populated
            isTileListPopulated = true;
        }
    }

    void PopulateTileList()
    {
        // Loop through each child of the GameObject
        foreach (Transform tile in transform)
        {
            // Check if the tile is already in the list
            if (!IsTileInList(tile))
            {
                // Create a new TileData instance and add it to the list
                TileData tileData = new TileData
                {
                    name = tile.name,
                    transform = tile
                };

                tileList.Add(tileData);

                // Display the information in the console (for testing purposes)
                Debug.Log("Tile name: " + tileData.name + ", Position: " + tileData.transform.position);
            }
        }
    }

    bool IsTileInList(Transform tile)
    {
        // Check if the tile is already in the list based on its name
        return tileList.Exists(tileData => tileData.name == tile.name);
    }

    // Public method to get the tile list
    public List<TileData> GetTileList()
    {
        return tileList;
    }
}
