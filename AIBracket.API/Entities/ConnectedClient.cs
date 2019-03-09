using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using AIBracket.API.Sockets;
using AIBracket.Data.Entities;

namespace AIBracket.API.Entities
{
    public class ConnectedClient : IConnectedClient
    {
        public ISocket Socket { get ; set ; }
        public AppUser User { get ; set ; }
        public Bot Bot { get ; set ; }
    }
}
