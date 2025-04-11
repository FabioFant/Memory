using System.Net.Sockets;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
public class ConncetionHandler : MonoBehaviour
{
    IPEndPoint remoteEP;
    Socket socket;

    GameManager gameManager;
    (int, bool)[,] matrix;

    public async void StartConnection()
    {    
        try
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            remoteEP = new IPEndPoint(ipAddress, 12345);
            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await Task.Run(ConnectToServer);
            gameManager.StartNewGame(matrix);
            SendData(new Dictionary<string, object> { { "Ready", true } });
        }
        catch(Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    void ConnectToServer()
    {
        try
        {
            Debug.Log("Connecting to the server...");
            socket.Connect(remoteEP);
            Debug.Log("Socket connected to " + socket.RemoteEndPoint.ToString());

            Dictionary<string, object> memorySettings = ReceiveData();
            string matrixJson = JsonConvert.SerializeObject(memorySettings["Matrix"]);
            matrix = JsonConvert.DeserializeObject<(int, bool)[,]>(matrixJson);
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

    #region Send and Receive
    Dictionary<string, object> ReceiveData()
    {
        try
        {
            Debug.Log("Waiting to receive data...");

            byte[] bytes = new byte[1024];
            socket.Receive(bytes);
            string json = Encoding.UTF8.GetString(bytes);

            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            Debug.Log("Received data: " + json);
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
            return null;
        }
    }

    void SendData(Dictionary<string, object> data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            socket.Send(bytes);

            Debug.Log("Sent data: " + json);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
    #endregion
}
