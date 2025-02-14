using System.Net;
using System.Net.Sockets;

namespace TCP_Server_Client_Demo
{
    internal class Program
    {
        private static List<(TcpClient client, string id)> clients = new List<(TcpClient, string)>();
        private static int clientCounter = 0;

        public static void ProcessClientRequests(object? client)
        {
            TcpClient tcpClient = (TcpClient)client;
            string clientId = "Client " + (++clientCounter);
            clients.Add((tcpClient, clientId));
            StreamReader reader = new StreamReader(tcpClient.GetStream());
            StreamWriter writer = new StreamWriter(tcpClient.GetStream()) { AutoFlush = true };
            string message;

            try
            {
                writer.WriteLine($"Welcome {clientId}");
                while ((message = reader.ReadLine()) != null)
                {
                    Console.WriteLine($"From {clientId} => " + message);
                    if (message.Equals("Exit"))
                        break;

                    foreach (var clientInstance in clients)
                    {
                        if (clientInstance.client != tcpClient)
                        {
                            StreamWriter clientWriter = new StreamWriter(clientInstance.client.GetStream()) { AutoFlush = true };
                            clientWriter.WriteLine($"{clientId} => " + message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                clients.Remove((tcpClient, clientId));
                reader.Close();
                writer.Close();
                tcpClient.Close();
                Console.WriteLine("Connection closed with client: " + clientId);
            }
        }

        static void Main(string[] args)
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5001);
                listener.Start();
                Console.WriteLine("Server has started...");
                while (true)
                {
                    Console.WriteLine("Waiting for a client...");
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected.");
                    Thread t = new Thread(() => ProcessClientRequests(client));
                    t.Start();
                }
            }
            catch (Exception? ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                listener?.Stop();
            }
        }
    }
}