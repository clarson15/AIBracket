using AIBracket.Data.Entities;
using AIBracket.GameLogic.Pacman.Pacman;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AIBracket.API.Entities
{
    public class PacmanClient : ConnectedClient
    {
        public PacmanPacman.Direction Direction { get ; set; }
    }
}
