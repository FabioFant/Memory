using System.Net.Sockets;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;

public class CreateConnection : MonoBehaviour
{
    Socket socket;
    public void StartConnection()
    {
        try
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 12345);
            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Debug.Log("Connecting to the server...");
                socket.Connect(remoteEP);
                Debug.Log("Socket connected to " + socket.RemoteEndPoint.ToString());

                Dictionary<string, object> memorySettings = ReceiveData();
                Memory memory = new Memory((int)memorySettings["Height"], (int)memorySettings["Width"]);
            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e.Message);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    public Dictionary<string, object> ReceiveData()
    {
        try
        {
            Debug.Log("Waiting to receive data...");

            byte[] bytes = new byte[1024];
            socket.Receive(bytes);
            string json = Encoding.UTF8.GetString(bytes);

            fsData data = fsJsonParser.Parse(json);
            fsSerializer serializer = new fsSerializer();
            Dictionary<string, object> result = new Dictionary<string, object>();
            serializer.TryDeserialize(data, ref result).AssertSuccess();

            Debug.Log("Received data: " + json);
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
