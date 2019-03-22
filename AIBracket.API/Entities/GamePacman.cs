using AIBracket.API.Sockets;
using AIBracket.GameLogic.Pacman.Game;
using AIBracket.GameLogic.Pacman.Pacman;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIBracket.API.Entities
{
    public class GamePacman
    {
        public PacmanGame Game { get; set; }
        public PacmanClient User { get; set; }
        public bool IsRunning { get; set; } = true;
        public bool IsStarted { get; set; } = false;
        public Guid Id { get; private set; } = Guid.NewGuid();
        public List<ISocket> Spectators { get; set; } = new List<ISocket>();

        public void GetUserInput()
        {
            if (!User.Socket.IsConnected)
            {
                IsRunning = false;
                return;
            }
            if(User.Socket.IsReady)
            {
                var command = User.Socket.ReadData();
                if(command != null)
                {
                    switch (command.ToUpper())
                    {
                        case "UP":
                            User.Direction = PacmanPacman.Direction.up;
                            break;
                        case "DOWN":
                            User.Direction = PacmanPacman.Direction.down;
                            break;
                        case "LEFT":
                            User.Direction = PacmanPacman.Direction.left;
                            break;
                        case "RIGHT":
                            User.Direction = PacmanPacman.Direction.right;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void UpdateGame()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                User.Socket.WriteData("0 " + Game.GetBoardString());
            }
            Game.UpdateGame(User.Direction);
            UpdateUsers(); // Change this when tickrate changes
            if(Game.Pacman.Lives == 0)
            {
                IsRunning = false;
                User.Socket.Disconnect();
            }
        }

        public void AddSpectator(ISocket spec)
        {
            spec.WriteData("0 " + Game.GetBoardString());
            Spectators.Add(spec);
        }

        private void UpdateUsers()
        {
            var update = "1 " + Game.Score + " " + Game.Pacman.Lives + " " + Game.Pacman.Location.Xpos + " " + Game.Pacman.Location.Ypos + " ";
            foreach(var ghost in Game.Ghosts)
            {
                update += ghost.Location.Xpos + " ";
                update += ghost.Location.Ypos + " ";
                update += ghost.IsDead + " ";
                update += ghost.IsVulnerable + " ";
            }
            update += "\r\n\r\n";
            User.Socket.WriteData(update);
            for(var i = 0; i < Spectators.Count; i++)
            {
                Spectators[i].WriteData(update);
                if (!Spectators[i].IsConnected)
                {
                    Spectators.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
