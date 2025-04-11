using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Memory____Server
{
    internal class Program
    {
        const int MAX_LENGHT = 7;
        const int MIN_LENGHT = 3;
        static Random rnd = new Random();
        static Memory memory;

        static Socket client1;
        static Socket client2;

        static void Main(string[] args)
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList.First();
            int port = 12345;

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            try
            {
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(2);

                Console.WriteLine($"Waiting for connection on port {port}...");

                client1 = listener.Accept();
                WriteMessage("1° CLIENT CONNECTED", ConsoleColor.DarkBlue);

                client2 = listener.Accept();
                WriteMessage("2° CLIENT CONNECTED", ConsoleColor.DarkBlue);

                int height = 0;
                int width = 0;
                do
                {
                    height = rnd.Next(MIN_LENGHT, MAX_LENGHT);
                    width = rnd.Next(MIN_LENGHT, MAX_LENGHT);
                } while (height * width % 2 != 0);
                memory = new Memory(width, height);

                var initMatrix = memory.GetMatrix();
                SendData(client1, new Dictionary<string, object> { { "Matrix", initMatrix }, { "Turn", true } });
                SendData(client2, new Dictionary<string, object> { { "Matrix", initMatrix }, { "Turn", false } });

                var result1 = ReceiveData(client1);
                var result2 = ReceiveData(client2);

                MainGame();
            }
            catch (Exception e)
            {
                WriteMessage(e.Message, ConsoleColor.Red);
            }
        }

        // server - both clients --> matrix: matrix, turn: bool ( true = client1, false = client2 )
        // both clients - server --> ready: bool

        // server - both clients --> start: bool
        // while ( !memory.IsFinished() )
        // client - server --> row1: int, col1: int, row2: int, col2: int
        // server - client --> error: int ( if error != 0 then return to previous step )
        // server - both clients --> row1: int, col1: int, row2: int, col2: int, result: bool, turn: bool, gameover: int

        // gameover:
        //  0: game is not over
        //  1: game is over ( player 1 wins )
        //  2: game is over ( player 2 wins )

        // error:
        //  0: no error
        //  1: one or both of the two cells has already been selected or the same cell has been selected twice
        //  2: the indexes are out of bounds ( code error )

        static void MainGame()
        {
            // Client
            Socket source = client1;
            bool turn = true;

            Dictionary<string, object> rd; // Rows and cols received

            // Additional data to send
            int error;
            int gameover;
            bool result = false;

            int p1 = 0, p2 = 0; // Points

            try
            {
                // Send start data to both clients
                var data = new Dictionary<string, object>
                {
                    { "Start", true }
                };
                SendData(client1, data);
                SendData(client2, data);

                while (!memory.IsFinished())
                {
                    do // Receive data from the client and verify it
                    {
                        rd = ReceiveData(source);
                        error = 0;
                        try
                        {
                            result = memory.Pick((int)rd["Row1"], (int)rd["Col1"], (int)rd["Row2"], (int)rd["Col2"]);
                        }
                        catch (InvalidOperationException e)
                        {
                            error = 1;
                            WriteMessage(e.Message, ConsoleColor.DarkRed);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            error = 2;
                            WriteMessage(e.Message, ConsoleColor.DarkRed);
                        }
                        if (error != 0)
                        {
                            SendData(source, new Dictionary<string, object> { { "Error", error } });
                        }
                    } while (error != 0);

                    // Define the end of the game
                    if (memory.IsFinished())
                    {
                        gameover = p1 > p2 ? 1 : 2;
                    }
                    else
                    {
                        gameover = 0;
                    }

                    turn = !turn;

                    // Response to both clients
                    data = new Dictionary<string, object>
                    {
                        { "Result", result },
                        { "Row1", (int)rd["Row1"] },
                        { "Col1", (int)rd["Col1"] },
                        { "Row2", (int)rd["Row2"] },
                        { "Col2", (int)rd["Col2"] },
                        { "Turn", turn },
                        { "GameOver", gameover }
                    };
                    SendData(client1, data);
                    SendData(client2, data);

                    source = turn ? client1 : client2;
                }
            }
            catch (Exception e)
            {
                WriteMessage(e.Message, ConsoleColor.Red);
            }

        }

        #region Send and Receive
        static void SendData(Socket destination, Dictionary<string, object> data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                destination.Send(bytes);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Data sent to {destination.RemoteEndPoint}: {json}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static Dictionary<string, object> ReceiveData(Socket source)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Waiting for data from {source.RemoteEndPoint}...");
                Console.ForegroundColor = ConsoleColor.White;

                byte[] bytes = new byte[1024];
                source.Receive(bytes);
                string json = Encoding.UTF8.GetString(bytes);

                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Data received from {source.RemoteEndPoint}: {json}");
                Console.ForegroundColor = ConsoleColor.White;

                return result;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }
        }
        #endregion

        #region Methods for Ports
        static bool IsPortAvailable(int port)
        {
            try
            {
                TcpListener tcpListener = new TcpListener(IPAddress.Loopback, port);
                tcpListener.Start();
                tcpListener.Stop();

                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        static int FindFreePort()
        {
            int port = 10000;
            while (!IsPortAvailable(port)) { port++; }
            return port;
        }
        #endregion

        #region Utils
        static void WriteMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        #endregion
    }
}
