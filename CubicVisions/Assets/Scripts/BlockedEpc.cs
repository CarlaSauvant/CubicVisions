using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockedEpc : MonoBehaviour
{
    [System.Serializable]
    public class StoredEpcs
    {
        public string key;
    }

    // List to store all retrieved keys from log messages
    public List<StoredEpcs> retrievedKeys = new List<StoredEpcs>();

    void OnEnable()
    {
        // Subscribe to the log message event
        Application.logMessageReceived += HandleEpcs;
    }

    void OnDisable()
    {
        // Unsubscribe from the log message event to avoid memory leaks
        Application.logMessageReceived -= HandleEpcs;
    }

    private void HandleEpcs(string logMessage, string stackTrace, LogType type)
    {
        // Log the raw logMessage for debugging
        Debug.Log($"Received log message: {logMessage}");

        // Check if the log message contains "Key retrieved: "
        if (logMessage.Contains("Key retrieved: "))
        {
            // Extract the key from the log message
            string key = logMessage.Replace("Key retrieved: ", "");

            // Add the key to the list of retrieved keys
            StoredEpcs keyData = new StoredEpcs { key = key };
            retrievedKeys.Add(keyData);
        }

        // Check if the log message contains "Blocked Key retrieved: "
        if (logMessage.Contains("Blocked key retrieved: "))
        {
            // Extract the blocked key from the log message
            string blockedKey = logMessage.Replace("Blocked key retrieved: ", "");

            // Log the blocked key for debugging
            Debug.Log($"Blocked key to remove: {blockedKey}");

            // Check if the blocked key is in the list of retrieved keys
            StoredEpcs blockedKeyData = retrievedKeys.Find(data => data.key == blockedKey);

            if (blockedKeyData != null)
            {
                // Remove the blocked key from the list
                retrievedKeys.Remove(blockedKeyData);
                Debug.Log($"Blocked key removed: {blockedKey}");

                // Log the current contents of the list for further inspection
                Debug.Log($"Current contents of the list: {string.Join(", ", retrievedKeys.Select(item => item.key))}");
            }
            else
            {
                Debug.LogWarning($"Blocked key not found in the list: {blockedKey}");
            }
        }
    }
}