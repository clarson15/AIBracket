using AIBracket.API.Entities;
using AIBracket.API.Sockets;
using AIBracket.GameLogic.Pacman.Game;
using AIBracket.GameLogic.Pacman.Pacman;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace AIBracket.API
{
    public static class GameMaster
    {
        private static List<GamePacman> games; // current games running (playing atm)
        private static List<IConnectedClient> waiting_players; // current connected players
        private static List<ISocket> spectators; 
        private static bool isRunning;

        public static void Initialize() { // constructor for our games list and players
            games = new List<GamePacman>(); // <- create functions to add players and games
            waiting_players = new List<IConnectedClient>();
            isRunning = false;
            spectators = new List<ISocket>();
        }

        public static bool WatchGame(ISocket spectator, string guid)
        {
            var game = games.FirstOrDefault(x => x.Id.ToString() == guid);
            if (game != null)
            {
                game.AddSpectator(spectator);
                return true;
            }
            return false;
        }

        public static void AddSpectator(ISocket spectator)
        {
            spectators.Add(spectator);
        }

        public static void AddPlayer(IConnectedClient player) {
            if(player.Bot.Game == 1)
            {
                games.Add(new GamePacman
                {
                    Game = new PacmanGame(),
                    User = (PacmanClient)player
                });
                Console.WriteLine("Added pacman game");
                return;
            }
            waiting_players.Add(player);
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
                    Console.WriteLine("Getting user input");
                    g.GetUserInput();
                }
                if (time_elapsed.TotalMilliseconds >= 500){
                    foreach (var g in games)
                    {
                        g.UpdateGame();
                        // g.Game.PrintBoard();
                    }
                    start_time = current_time;
                }

                for(var i = 0; i < games.Count; i++)
                {
                    if (!games[i].IsRunning)
                    {
                        games[i].Game.PrintBoard();
                        spectators.AddRange(games[i].Spectators);
                        games.RemoveAt(i);
                        i--;
                    }
                }
                foreach(var client in waiting_players)
                {
                    if(client.Socket.IsReady)
                    {
                        var message = client.Socket.ReadData();
                        Console.WriteLine("Game master read: " + message);
                        client.Socket.WriteData("Hello world");

                    }
                }
                for(var i = 0; i < spectators.Count; i++)
                {
                    if (!spectators[i].IsConnected)
                    {
                        spectators.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (spectators[i].IsReady)
                    {
                        var message = spectators[i].ReadData().Trim();
                        Console.WriteLine("Spectator said " + message);
                        if (message == "LIST GAMES")
                        {
                            spectators[i].WriteData("Pacman: " + games.Count);
                        }
                        else if (message == "LIST PACMAN")
                        {
                            var buffer = "";
                            foreach (var game in games)
                            {
                                buffer += game.Id.ToString() + " " + game.Game.Score + "\n";
                            }
                            spectators[i].WriteData(buffer);
                        }
                        else if(message.StartsWith("WATCH "))
                        {
                            var guid = message.Substring(6);
                            var game = games.FirstOrDefault(x => x.Id.ToString() == guid);
                            if(game == null)
                            {
                                spectators[i].WriteData("Failed to find game");
                            }
                            else
                            {
                                game.AddSpectator(spectators[i]);
                                spectators.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
