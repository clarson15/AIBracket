using AIBracket.Data.Entities;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AIBracket.API.Entities
{
    public class ConnectedClient
    {
        public TcpClient Socket;
        public AppUser User;
        public Bot Bot;
    }
}
