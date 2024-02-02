using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using TMPro;
using System;
using System.IO;

public class ArduinoCommunication : MonoBehaviour
{
    public TMP_InputField displayText1;
    public TMP_InputField displayText2;
    public TMP_InputField cancelText1;
    public TMP_InputField cancelText2;
    public Button startScanningButton; // should be attached to "next" button
    SerialPort serialPort;
    EpcDictionary epcDictionary;

    private bool shouldStopReading = false;

    void Start()
    {
        // Initialising the serial port
        serialPort = new SerialPort("COM6", 115200);
        serialPort.Open();

        epcDictionary = new EpcDictionary();

        // Add button click event listener
        startScanningButton.onClick.AddListener(StartScanning);

        // Start a background thread to listen to the serial port
        StartReading();
    }

    void OnDestroy()
    {
        // Close the serial port when destroying the script
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }

    void StartReading()
    {
        // Start a background thread to listen to the serial port
        System.Threading.Thread thread = new System.Threading.Thread(ReadSerialData);
        thread.Start();
    }

    void ReadSerialData()
    {
        while (true)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    string data = serialPort.ReadLine();
                    Debug.Log("Received: " + data);

                    // Check if the received data starts with "epc"
                    if (data.StartsWith("epc", StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log("EPC read");

                        // Use the entire string as the key in the dictionary
                        string key = data.Trim();
                        string value = epcDictionary.GetValue(key);

                        // Update the text fields if they are empty
                        if (string.IsNullOrEmpty(displayText1.text))
                        {
                            Debug.Log("EPC " + key + " recognized");
                            displayText1.text = value;
                        }
                        else if (string.IsNullOrEmpty(displayText2.text))
                        {
                            Debug.Log("EPC " + key + " recognized");
                            displayText2.text = value;
                        }
                        else
                        {
                            // Both text fields are filled; handle this case as needed
                            Debug.LogWarning("No additional input possible.");
                        }
                    }
                    if (data.StartsWith("cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.LogWarning("Tag disabled");
                    }
                }
            }
            catch (Exception e)
            {
                if (e is IOException || e is TimeoutException)
                {
                    // Handle specific exceptions related to the serial connection being interrupted
                    Debug.Log("Connection to Arduino interrupted. Check the Arduino connection and restart the game if needed.");
                }
                else
                {
                    // Log other exceptions
                    Debug.LogError("Error reading serial data: " + e.Message);
                }
            }
        }
    }

    void StartScanning()
    {
        // Button clicks send commands to the Arduino
        if (serialPort.IsOpen)
        {
            serialPort.Write("we are ready to rock");

            Debug.Log("Scanning has started.");
        }
    }

    // Called when the application is quitting
    void OnApplicationQuit()
    {
        shouldStopReading = true;
    }
}