using System.Net.Sockets;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using System.Threading;
using System.Threading.Tasks;

public class ConncetionHandler : MonoBehaviour
{
    IPEndPoint remoteEP;
    Socket socket;
    GameManager gameManager;

    public void StartConnection()
    {
        try
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            remoteEP = new IPEndPoint(ipAddress, 12345);
            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Task.Run(ConnectToServer);
        }
        catch(Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
    Dictionary<string, string> ReceiveData()
    {
        try
        {
            Debug.Log("Waiting to receive data...");

            byte[] bytes = new byte[1024];
            socket.Receive(bytes);
            string json = Encoding.UTF8.GetString(bytes);

            fsData data = fsJsonParser.Parse(json);
            fsSerializer serializer = new fsSerializer();
            Dictionary<string, string> result = new Dictionary<string, string>();
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

    void ConnectToServer()
    {
        try
        {
            Debug.Log("Connecting to the server...");
            socket.Connect(remoteEP);
            Debug.Log("Socket connected to " + socket.RemoteEndPoint.ToString());

            Dictionary<string, string> memorySettings = ReceiveData();
            gameManager
            
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
}
