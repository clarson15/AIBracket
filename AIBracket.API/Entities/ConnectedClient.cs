using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using AIBracket.Data.Entities;

namespace AIBracket.API.Entities
{
    public class ConnectedClient : IConnectedClient
    {
        public TcpClient Socket { get ; set ; }
        public AppUser User { get ; set ; }
        public Bot Bot { get ; set ; }
        public bool IsWebSocket { get ; set ; }

        public string ReadData()
        {
            if (Socket.Available == 0)
            {
                return null;
            }
            var data = new byte[Socket.Available];
            Socket.GetStream().Read(data, 0, Socket.Available);
            return Encoding.ASCII.GetString(data);
        }

        public bool SendData(string message)
        {
            if (!Socket.Connected)
            {
                return false;
            }
            var bytes = Encoding.ASCII.GetBytes(message);
            Socket.GetStream().Write(bytes);
            return true;
        }
    }
}
