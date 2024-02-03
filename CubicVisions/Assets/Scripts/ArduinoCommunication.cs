using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using TMPro;
using System;
using System.IO;
using System.Collections;

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
        // Initializing the serial port
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

                    // Use the entire string as the key in the dictionary
                    string key = data;
                    string value = epcDictionary.GetValue(key);

                    // Check if the value is not "EPC not found" before updating text fields
                    if (!value.Equals("EPC not found", StringComparison.OrdinalIgnoreCase))
                    {
                        // Update the text fields if they are empty
                        if (string.IsNullOrEmpty(displayText1.text))
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateTextField(displayText1, value));
                        }
                        else if (string.IsNullOrEmpty(displayText2.text))
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateTextField(displayText2, value));
                        }
                        else
                        {
                            // Both text fields are filled; handle this case as needed
                            Debug.LogWarning("No additional input possible.");
                        }
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

    // Update the text field from the main thread
    void UpdateTextField(TMP_InputField textField, string value)
    {
        textField.text = value;
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
}
