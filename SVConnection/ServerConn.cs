using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace My_food_manager.SVConnection
{
    public class ServerConn
    {
        private Socket socket;
        private readonly int port;
        private readonly string ip;

        public ServerConn(int port, string ip)
        {
            this.ip = ip;
            this.port = port;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(ip, port);
        }


        public string DataReceive()
        {
            byte[] buffer = new byte[8192];
            socket.Receive(buffer);
            while (socket.Available > 0)
            {
                byte[] secondData = new byte[socket.Available];
                socket.Receive(secondData);
                buffer = buffer.Concat(secondData).ToArray();
            }
            return Encoding.Unicode.GetString(buffer, 0, buffer.Length);
        }

        public void DataSend(string data)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(data);
            socket.Send(buffer);
        }
    }
}
