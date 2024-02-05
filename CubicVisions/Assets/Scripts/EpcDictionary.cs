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
            // tile coordinates
            {"BA A1 00 00 ", "A1!"},
            {"BA A2 00 00 ", "A2!"},
            {"BA A3 00 00 ", "A3!"},
            {"BA A4 00 00 ", "A4!"},
            {"BA A5 00 00 ", "A5!"},

            {"BA B1 00 00 ", "B1!"},
            {"BA B2 00 00 ", "B2!"},
            {"BA B3 00 00 ", "B3!"},
            {"BA B4 00 00 ", "B4!"},
            {"BA B5 00 00 ", "B5!"},

            {"BA C1 00 00 ", "C1!"},
            {"BA C2 00 00 ", "C2!"},
            {"BA C3 00 00 ", "C3!"},
            {"BA C4 00 00 ", "C4!"},
            {"BA C5 00 00 ", "C5!"},

            {"BA D1 00 00 ", "D1!"},
            {"BA D2 00 00 ", "D2!"},
            {"BA D3 00 00 ", "D3!"},
            {"BA D4 00 00 ", "D4!"},
            {"BA D5 00 00 ", "D5!"},

            {"BA E1 00 00 ", "E1!"},
            {"BA E2 00 00 ", "E2!"},
            {"BA E3 00 00 ", "E3!"},
            {"BA E4 00 00 ", "E4!"},
            {"BA E5 00 00 ", "E5!"},
            
            // mobility cube #1
            {"CB 01 01 00 ", "Mobility_11!"},
            {"CB 01 02 00 ", "Mobility_12!"},
            {"CB 01 03 00 ", "Mobility_13!"},
            {"CB 01 04 00 ", "Mobility_14!"},
            {"CB 01 05 00 ", "Mobility_15!"},

            // mobility cube #2
            {"CB 02 01 00 ", "Mobility_21!"},
            {"CB 02 02 00 ", "Mobility_22!"},
            {"CB 02 03 00 ", "Mobility_23!"},
            {"CB 02 04 00 ", "Mobility_24!"},
            {"CB 02 05 00 ", "Mobility_25!"},

            // play cube #1
            {"E2 80 11 60 60 00 02 13 54 C5 D8 D9 ", "Play_11!"},
            {"E2 80 11 60 60 00 02 13 54 C5 D8 C9 ", "Play_12!"},
            {"E2 80 11 60 60 00 02 13 54 C5 D8 8A ", "Play_13!"},
            {"E2 80 11 60 60 00 02 13 54 C5 D8 F9 ", "Play_14!"},
            {"CB 03 05 00 ", "Play_15!"},

            // play cube #2
            {"CB 04 01 00 ", "Play_21!"},
            {"CB 04 02 00 ", "Play_22!"},
            {"CB 04 03 00 ", "Play_23!"},
            {"CB 04 04 00 ", "Play_24!"},
            {"CB 04 05 00 ", "Play_25!"},


            // water cube #1
            {"CB 05 01 00 ", "Water_11!"},
            {"CB 05 02 00 ", "Water_12!"},
            {"CB 05 03 00 ", "Water_13!"},
            {"CB 05 04 00 ", "Water_14!"},
            {"CB 05 05 00 ", "Water_15!"},
            
            // more entries can be added

            /* EPC LIST FOR FULLY FUNCTIONING PROTOTYPE WITH CANCEL METHOD
            // tile coordinates
            {"epc[BA A1 00 00 ]", "A1"},
            {"epc[BA A2 00 00 ]", "A2"},
            {"epc[BA A3 00 00 ]", "A3"},
            {"epc[BA A4 00 00 ]", "A4"},
            {"epc[BA A5 00 00 ]", "A5"},

            {"epc[BA B1 00 00 ]", "B1"},
            {"epc[BA B2 00 00 ]", "B2"},
            {"epc[BA B3 00 00 ]", "B3"},
            {"epc[BA B4 00 00 ]", "B4"},
            {"epc[BA B5 00 00 ]", "B5"},

            {"epc[BA C1 00 00 ]", "C1"},
            {"epc[BA C2 00 00 ]", "C2"},
            {"epc[BA C3 00 00 ]", "C3"},
            {"epc[BA C4 00 00 ]", "C4"},
            {"epc[BA C5 00 00 ]", "C5"},

            {"epc[BA D1 00 00 ]", "D1"},
            {"epc[BA D2 00 00 ]", "D2"},
            {"epc[BA D3 00 00 ]", "D3"},
            {"epc[BA D4 00 00 ]", "D4"},
            {"epc[BA D5 00 00 ]", "D5"},

            {"epc[BA E1 00 00 ]", "E1"},
            {"epc[BA E2 00 00 ]", "E2"},
            {"epc[BA E3 00 00 ]", "E3"},
            {"epc[BA E4 00 00 ]", "E4"},
            {"epc[BA E5 00 00 ]", "E5"},
            
            // mobility cube #1
            {"epc[CB 01 01 00 ]", "Mobility_11"},
            {"epc[CB 01 02 00 ]", "Mobility_12"},
            {"epc[CB 01 03 00 ]", "Mobility_13"},
            {"epc[CB 01 04 00 ]", "Mobility_14"},
            {"epc[CB 01 05 00 ]", "Mobility_15"},

            // mobility cube #2
            {"epc[CB 02 01 00 ]", "Mobility_21"},
            {"epc[CB 02 02 00 ]", "Mobility_22"},
            {"epc[CB 02 03 00 ]", "Mobility_23"},
            {"epc[CB 02 04 00 ]", "Mobility_24"},
            {"epc[CB 02 05 00 ]", "Mobility_25"},

            // play cube #1
            {"epc[CB 03 01 00 ]", "Play_11"},
            {"epc[CB 03 02 00 ]", "Play_12"},
            {"epc[CB 03 03 00 ]", "Play_13"},
            {"epc[CB 03 04 00 ]", "Play_14"},
            {"epc[CB 03 05 00 ]", "Play_15"},

            // play cube #2
            {"epc[CB 04 01 00 ]", "Play_21"},
            {"epc[CB 04 02 00 ]", "Play_22"},
            {"epc[CB 04 03 00 ]", "Play_23"},
            {"epc[CB 04 04 00 ]", "Play_24"},
            {"epc[CB 04 05 00 ]", "Play_25"},


            // water cube #1
            {"epc[CB 05 01 00 ]", "Water_11"},
            {"epc[CB 05 02 00 ]", "Water_12"},
            {"epc[CB 05 03 00 ]", "Water_13"},
            {"epc[CB 05 04 00 ]", "Water_14"},
            {"epc[CB 05 05 00 ]", "Water_15"},
            
            // more entries can be added
            */
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

    // Get the key associated with the value
    public string GetKey(string value)
    {
        foreach (var pair in epcDictionary)
        {
            if (pair.Value == value)
            {
                return pair.Key;
            }
        }

        // Handle the case when the value is not found, for example, return an error message or an empty string
        UnityEngine.Debug.LogError("Tag name not found in EpcDictionary: " + value);
        return "Tag name Not Found";
    }

    // Get all keys associated with a specific value
    public List<string> GetKeysByValue(string value)
    {
        List<string> keys = new List<string>();

        foreach (var pair in epcDictionary)
        {
            if (pair.Value == value)
            {
                keys.Add(pair.Key);
            }
        }

        // Return the list of keys
        return keys;
    }
}