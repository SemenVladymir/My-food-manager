using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.BDConnection
{
    public class Connection
    {
        private Socket socket;
        private int port;
        private IPEndPoint endPoint;
        private Socket client;

        public Connection(int port, IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            this.port = port;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Bind(endPoint);
            this.socket.Listen();
        }

        public void Run()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Server start...");
            client =  socket.Accept();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Client connect... {client.RemoteEndPoint}");
        }

        public string DataReceive()
        {
            byte[] firstData = new byte[8192];
            int bytes = client.Receive(firstData);
            byte[] buffer;
            if (client.Available > 0)
            {
                byte[] secondData = new byte[client.Available];
                client.Receive(secondData);
                buffer = firstData.Concat(secondData).ToArray();
            }
            else
                buffer = firstData;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{buffer.Length} bytes received from the client");
            return Encoding.Unicode.GetString(buffer, 0, buffer.Length);
        }

        public void DataSend (string data)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(data);
            client.Send(buffer);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{buffer.Length} bytes sent from the server to a client");
        }


    }
}
