﻿using AIBracket.API.Sockets;
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
            Game.UpdateGame(User.Direction);
            UpdateUsers();
            if(Game.Pacman.Lives == 0)
            {
                IsRunning = false;
                User.Socket.Disconnect();
            }
        }

        public void AddSpectator(ISocket spec)
        {
            var ret = "";
            ret += (int)PacmanGame.EventType.BoardReset + " " + Game.GetBoardString() + "*";
            ret += (int)PacmanGame.EventType.PacmanLives + " " + Game.Pacman.Lives + "*";
            foreach(var g in Game.Ghosts)
            {
                ret += (int)PacmanGame.EventType.GhostUpdate + $" {g.GetPosition().ToString()} {g.IsDead} {g.IsVulnerable}*";
            }
            ret += (int)PacmanGame.EventType.PacmanUpdate + " " + Game.Pacman.GetPosition().ToString() + "*";
            ret += (int)PacmanGame.EventType.Score + " " + Game.Score;
            spec.WriteData(ret);
            if (spec.IsConnected)
            {
                Spectators.Add(spec);
            }
        }

        private void UpdateUsers()
        {
            var data = "";
            for(var i = 0; i < Game.CurrentGameEvent.Count; i++)
            {
                var e = Game.CurrentGameEvent[i];
                data += ((int)e.Key + " " + e.Value);
                if(i < Game.CurrentGameEvent.Count - 1)
                {
                    data += "*";
                }
            }
            Game.CurrentGameEvent.Clear();

            for (var i = 0; i < Spectators.Count; i++)
            {
                Spectators[i].WriteData(data);
                if (!Spectators[i].IsConnected)
                {
                    Spectators.RemoveAt(i);
                    i--;
                }
            }
            User.Socket.WriteData(data);
            if (!User.Socket.IsConnected)
            {
                IsRunning = false;
            }
        }
    }
}
