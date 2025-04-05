using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Memory____Server
{
    internal class Program
    {
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
            catch(SocketException)
            {
                return false;
            }   
        }

        static int FindFreePort()
        {
            int port = 10000;
            while(!IsPortAvailable(port)) { port++; }
            return port;
        }
        #endregion

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
            int port = FindFreePort();

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            try
            {
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(2);

                Console.WriteLine($"Waiting for connection on port {port}...\n");

                Console.ForegroundColor = ConsoleColor.DarkBlue;
                client1 = listener.Accept();
                Console.WriteLine("1° CLIENT CONNECTED");

                client2 = listener.Accept();
                Console.WriteLine("2° CLIENT CONNECTED");
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine();

                int height = rnd.Next(MIN_LENGHT, MAX_LENGHT);
                int width = rnd.Next(MIN_LENGHT, MAX_LENGHT);
                memory = new Memory(width, height);

                var data = new Dictionary<string, object>
                {
                    { "Height", memory.Height },
                    { "Width", memory.Width }
                };

                SendData(client1, data);
                SendData(client2, data);
            }
            catch(Exception e) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }

            static void SendData(Socket destination, Dictionary<string, object> data)
            {
                try
                {
                    string json = JsonSerializer.Serialize(data);
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    destination.Send(bytes);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
