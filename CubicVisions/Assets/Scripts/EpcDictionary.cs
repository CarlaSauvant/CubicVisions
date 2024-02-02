using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EpcDictionary
{
    public Dictionary<string, string> epcDictionary;

    public EpcDictionary()
    {
        epcDictionary = new Dictionary<string, string>
        {
            {"epc[E2 80 11 60 60 00 02 13 54 C5 2A 4B ]", "C3"},
            
            {"epc[E2 80 11 60 60 00 02 13 54 C5 68 FC ]", "Play_11"},
            {"epc[E2 80 11 60 60 00 02 13 54 C5 9A 2C ]", "Play_12"},
            {"epc[E2 80 11 60 60 00 02 13 54 C5 68 BC ]", "Play_13"},
            // more entries can be added
        };
    }

    // Get the value associated with the key
    public string GetValue(string key)
    {
        if (epcDictionary.TryGetValue(key, out var value))
        {
            return value;
        }
        else
        {
            // Handle the case when the key is not found, for example, return an error message or an empty string
            UnityEngine.Debug.LogError("EPC not found in EpcDictionary: " + key);
            return "EPC Not Found";
        }
    }
}