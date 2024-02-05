using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGrid : MonoBehaviour
{
    [System.Serializable]
    public class TileData
    {
        public string name;
        public Transform transform;
    }

    public List<TileData> tileList = new List<TileData>();
    private bool isTileListPopulated = false;

    void Start()
    {
        // Assuming you have a UI button attached to the script
        Button enterButton = GetComponent<Button>();

        if (enterButton != null)
        {
            // Attach the MakeList method to the button click event
            enterButton.onClick.AddListener(MakeList);
        }
        else
        {
            // If there's no button, populate the tile list directly
            MakeList();
        }
    }

    // Public method to get the tile list
    public List<TileData> GetTileList()
    {
        // Ensure the tile list is populated before returning it
        MakeList();
        return tileList;
    }

    void MakeList()
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
        foreach (Transform tile in transform)
        {
            if (!IsTileInList(tile))
            {
                TileData tileData = new TileData
                {
                    name = tile.name,
                    transform = tile
                };

                tileList.Add(tileData);
                Debug.Log("Tile name: " + tileData.name + ", Position: " + tileData.transform.position);
            }
        }
    }

    bool IsTileInList(Transform tile)
    {
        return tileList.Exists(tileData => tileData.name == tile.name);
    }
}