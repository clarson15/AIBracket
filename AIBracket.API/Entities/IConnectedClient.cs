using AIBracket.Data.Entities;
using AIBracket.GameLogic.Pacman.Pacman;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AIBracket.API.Entities
{
    public interface IConnectedClient
    {
        TcpClient Socket { get; set; }
        AppUser User { get; set; }
        Bot Bot { get; set; }
        bool IsWebSocket { get; set; }
        string ReadData();
        bool SendData(string message);
    }
}
