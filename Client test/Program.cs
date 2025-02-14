using System.Net;
using System.Net.Sockets;

namespace Client_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 5001);
            StreamWriter writer = new StreamWriter(tcpClient.GetStream()) { AutoFlush = true };
            StreamReader reader = new StreamReader(tcpClient.GetStream());

            Thread readThread = new Thread(() =>
            {
                while (tcpClient.Connected)
                {
                    try
                    {
                        string response = reader.ReadLine();
                        if (response != null)
                        {
                            Console.WriteLine(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            });
            readThread.Start();

            while (tcpClient.Connected)
            {
                string s = Console.ReadLine();
                if (s == "Exit")
                    break;
                Console.Write("==>");
                writer.WriteLine(s);
            }

            tcpClient.Close();
        }
    }
}