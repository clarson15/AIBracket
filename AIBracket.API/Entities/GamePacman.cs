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
            if (!User.Socket.Connected)
            {
                IsRunning = false;
                return;
            }
            if(User.Socket.Available > 0)
            {
                var command = User.ReadData();
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
        }

        private void UpdateUsers()
        {
            var update = "update";
            if (!User.SendData(update))
            {
                IsRunning = false;
            }
        }
    }
}
