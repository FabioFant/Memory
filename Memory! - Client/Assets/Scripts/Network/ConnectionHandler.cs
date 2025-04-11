using System.Net.Sockets;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.UI;
public class ConnectionHandler : MonoBehaviour
{
    Button button;

    IPEndPoint remoteEP;
    Socket socket;

    GameManager gameManager;
    (int, bool)[,] matrix;

    bool idTurn, turn;
    int row1, col1, row2, col2;

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

            idTurn = (bool)memorySettings["Turn"];
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    public async void StartConnection()
    {    
        button.gameObject.SetActive(false);
        try
        {
            gameManager.UpdateStatusText("Connecting to the server...");

            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            remoteEP = new IPEndPoint(ipAddress, 12345);
            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await Task.Run(ConnectToServer);
            gameManager.StartNewGame(matrix);
            SendData(new Dictionary<string, object> { { "Ready", true } });
            MainGame();
        }
        catch(Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    async void MainGame()
    {
        var start = ReceiveData();
        
        Debug.Log("Game started!");

        Dictionary<string, object> data = new Dictionary<string, object>();
        turn = true;

        try
        {
            do
            {
                if (turn == idTurn)
                {
                    gameManager.UpdateStatusText("Your turn!");
                    Debug.Log("Your turn.");
                    do
                    {
                        row1 = -1; col1 = -1; row2 = -1; col2 = -1;
                        await Task.Run(() =>
                        {
                            while (row1 == -1 || col1 == -1 || row2 == -1 || col2 == -1)
                            {
                                Task.Delay(100);
                            }
                        });

                        data = new Dictionary<string, object>
                        {
                            { "Row1", row1 },
                            { "Col1", col1 },
                            { "Row2", row2 },
                            { "Col2", col2 }
                        };
                        SendData(data);
                        data = ReceiveData();

                    } while (Convert.ToInt32(data["Error"]) != 0);

                    Debug.Log("Success.");

                    row1 = -1; col1 = -1; row2 = -1; col2 = -1;
                    gameManager.ActionOnCards(Convert.ToInt32(data["Row1"]), Convert.ToInt32(data["Col1"]),
                        Convert.ToInt32(data["Row2"]), Convert.ToInt32(data["Col2"]), (bool)data["Result"]);
                }
                else
                {
                    gameManager.UpdateStatusText("Waiting for the other player...");
                    Debug.Log("It's NOT your turn.");

                    await Task.Run(() => { data = ReceiveData(); });
                    gameManager.ActionOnCards(Convert.ToInt32(data["Row1"]), Convert.ToInt32(data["Col1"]),
                        Convert.ToInt32(data["Row2"]), Convert.ToInt32(data["Col2"]), (bool)data["Result"]);
                }

                int points1 = idTurn ? Convert.ToInt32(data["Points1"]) : Convert.ToInt32(data["Points2"]);
                int points2 = idTurn ? Convert.ToInt32(data["Points2"]) : Convert.ToInt32(data["Points1"]);

                gameManager.UpdateClientPoints(points1, points2);

                turn = (bool)data["Turn"];

            } while (Convert.ToInt32(data["GameOver"]) == 0);
            
            if(Convert.ToInt32(data["GameOver"]) == 1 && idTurn)
                gameManager.UpdateStatusText("You win!");
            else if(Convert.ToInt32(data["GameOver"]) == 1 && !idTurn)
                gameManager.UpdateStatusText("You lose!");
            else if(Convert.ToInt32(data["GameOver"]) == 2 && idTurn)
                gameManager.UpdateStatusText("You win!");
            else if (Convert.ToInt32(data["GameOver"]) == 2 && !idTurn)
                gameManager.UpdateStatusText("You lose!");
            else
                gameManager.UpdateStatusText("It's a draw!");

            socket.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    public void SetCard(int row, int col)
    {
        if (turn == idTurn || (row1 == -1 && row2 == -1 && col1 == -1 && col2 == -1))
        {
            if (row1 == -1 || col1 == -1)
            {
                row1 = row;
                col1 = col;

                Debug.Log($"First card selected: ({row1}, {col1})");
            }
            else
            {
                row2 = row;
                col2 = col;

                Debug.Log($"Second card selected: ({row2}, {col2})");
            }
        }
    }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartConnection);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    #region Send and Receive
    Dictionary<string, object> ReceiveData()
    {
        try
        {
            Debug.Log("Waiting to receive data...");

            byte[] bytes = new byte[4096];
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
