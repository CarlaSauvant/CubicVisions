using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

public class ArduinoCommunication : MonoBehaviour
{
    public TMP_InputField displayText1;
    public TMP_InputField displayText2;
    public Button startScanningButton; // should be attached to "next" button
    SerialPort serialPort;
    List<string> dataList = new List<string>();
    List<string> dataList1 = new List<string>();
    List<string> dataList2 = new List<string>();

    void Start()
    {
        // 初始化串口
        serialPort = new SerialPort("COM5", 115200);
        serialPort.Open();

        // 添加按钮点击事件监听器
        startScanningButton.onClick.AddListener(StartScanning);

        // 启动后台线程监听串口
        StartReading();
    }

    void Update()
    {
        // 处理接收到的数据
        ProcessReceivedData();
    }

    void OnDestroy()
    {
        // 在销毁脚本时关闭串口
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }

    void StartReading()
    {
        // 启动后台线程监听串口
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

                    // 将数据添加到总列表
                    dataList.Add(data);

                    // 根据总列表长度判断奇偶，分别加入不同的列表
                    if (dataList.Count % 2 == 0)
                    {
                        dataList2.Add(data);
                    }
                    else
                    {
                        dataList1.Add(data);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading serial data: " + e.Message);
            }
        }
    }

    void StartScanning()
    {
        // 按钮点击后，向Arduino发送指令
        if (serialPort.IsOpen)
        {
            serialPort.Write("we are ready to rock");
        }
    }

    void ProcessReceivedData()
    {
        // 在主线程中处理接收到的数据
        if (dataList1.Count > 0)
        {
            string data = dataList1[dataList1.Count - 1];
            Debug.Log("Processing: " + data);

            // 更新显示文本
            displayText1.text = data;
        }

        if (dataList2.Count > 0)
        {
            string data = dataList2[dataList2.Count - 1];
            Debug.Log("Processing: " + data);

            // 更新显示文本
            displayText2.text = data;
        }
    }
}
