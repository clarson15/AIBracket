using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AIBracket.API.Sockets
{
    public class BotSocket : ISocket
    {
        private TcpClient _socket;

        public bool IsReady => _socket.Connected && _socket.Available > 0;

        public bool IsConnected => _socket.Connected;

        public BotSocket(TcpClient client)
        {
            _socket = client;
        }

        public void Disconnect()
        {
            _socket.Close();
        }

        public string ReadData()
        {
            if (IsReady)
            {
                var readAmount = _socket.Available;
                if (readAmount > 1024)
                {
                    Console.WriteLine("Data larger than 1KB: " + readAmount + " bytes");
                    Disconnect();
                    return "";
                }
                var data = new byte[readAmount];
                try
                {
                    _socket.GetStream().Read(data, 0, readAmount);
                    return Encoding.ASCII.GetString(data).Trim();
                }
                catch(Exception e)
                {
                    _socket.Close();
                    Console.WriteLine(e.Message);
                    return "";
                }
            }
            return "";
        }

        public void WriteData(string data)
        {
            if (IsConnected)
            {
                var bytes = Encoding.ASCII.GetBytes(data);
                try
                {
                    _socket.GetStream().Write(bytes);
                }
                catch(Exception e)
                {
                    _socket.Close();
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
