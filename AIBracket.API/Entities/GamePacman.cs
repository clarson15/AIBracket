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
            Game.UpdateGame(User.Direction);
            UpdateUsers(); // Change this when tickrate changes
            if(Game.pacman.Lives == 0)
            {
                IsRunning = false;
                User.Socket.Disconnect();
            }
        }

        private void UpdateUsers()
        {
            var update = Game.score + " " + Game.pacman.Lives + " " + Game.pacman.Location.Xpos + " " + Game.pacman.Location.Ypos + " ";
            foreach(var ghost in Game.ghosts)
            {
                update += ghost.Location.Xpos + " ";
                update += ghost.Location.Ypos + " ";
                update += ghost.IsDead + " ";
                update += ghost.IsVulnerable + " ";
            }
            update += "\r\n\r\n";
            User.Socket.WriteData(update);
        }
    }
}
