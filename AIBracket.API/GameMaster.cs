using AIBracket.API.Entities;
using AIBracket.GameLogic.Pacman.Game;
using AIBracket.GameLogic.Pacman.Pacman;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AIBracket.API
{
    public static class GameMaster
    {
        private static List<GamePacman> games; // current games running (playing atm)
        private static List<IConnectedClient> waiting_players; // current connected players
        private static bool isRunning;
        private static Mutex mut = new Mutex();

        public static void Initialize() { // constructor for our games list and players
            games = new List<GamePacman>(); // <- create functions to add players and games
            waiting_players = new List<IConnectedClient>();
            isRunning = false;
        }

        public static void AddPlayer(IConnectedClient player) {
            if(player.Bot.Game == 1)
            {
                games.Add(new GamePacman
                {
                    Game = new PacmanGame(),
                    User = (PacmanClient)player
                });
                return;
            }
            mut.WaitOne();
            waiting_players.Add(player);
            mut.ReleaseMutex();
        }

        public static int GetPlayerCount() {
            return waiting_players.Count; // for creating a new game
        }

        public static void Run() {
            isRunning = true;
            var start_time = DateTime.UtcNow;
            while (isRunning) {
                var current_time = DateTime.UtcNow;
                var time_elapsed = current_time - start_time;
                foreach (var g in games)
                {
                    g.GetUserInput();
                }
                if (time_elapsed.TotalMilliseconds >= 1000){
                    foreach (var g in games)
                    {
                        g.UpdateGame();
                    }
                    start_time = current_time;
                }

                for(var i = 0; i < games.Count; i++)
                {
                    if (!games[i].IsRunning)
                    {
                        games[i].Game.PrintBoard();
                        games.RemoveAt(i);
                        i--;
                    }
                }

                mut.WaitOne();
                foreach(var client in waiting_players)
                {
                    if(client.Socket.IsReady)
                    {
                        var message = client.Socket.ReadData();
                        Console.WriteLine("Game master read: " + message);
                        client.Socket.WriteData("Hello world");

                    }
                }
                mut.ReleaseMutex();
            }
        }
    }
}
