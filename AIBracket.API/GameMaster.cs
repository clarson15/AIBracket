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
        private static Mutex clientMutex = new Mutex(), spectatorMutex = new Mutex(), gameMutex = new Mutex();

        public static void Initialize() { // constructor for our games list and players
            games = new List<GamePacman>(); // <- create functions to add players and games
            waiting_players = new List<IConnectedClient>();
            isRunning = false;
            spectators = new List<ISocket>();
        }

        public static bool WatchGame(ISocket spectator, string guid)
        {
            gameMutex.WaitOne();
            var game = games.FirstOrDefault(x => x.Id.ToString() == guid);
            if (game != null)
            {
                game.Spectators.Add(spectator);
                gameMutex.ReleaseMutex();
                return true;
            }
            gameMutex.ReleaseMutex();
            return false;
        }

        public static void AddSpectator(ISocket spectator)
        {
            spectatorMutex.WaitOne();
            spectators.Add(spectator);
            spectatorMutex.ReleaseMutex();
        }

        public static void AddPlayer(IConnectedClient player) {
            if(player.Bot.Game == 1)
            {
                gameMutex.WaitOne();
                games.Add(new GamePacman
                {
                    Game = new PacmanGame(),
                    User = (PacmanClient)player
                });
                gameMutex.ReleaseMutex();
                return;
            }
            clientMutex.WaitOne();
            waiting_players.Add(player);
            clientMutex.ReleaseMutex();
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
                gameMutex.WaitOne();
                foreach (var g in games)
                {
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
                gameMutex.ReleaseMutex();
                clientMutex.WaitOne();
                foreach(var client in waiting_players)
                {
                    if(client.Socket.IsReady)
                    {
                        var message = client.Socket.ReadData();
                        Console.WriteLine("Game master read: " + message);
                        client.Socket.WriteData("Hello world");

                    }
                }
                clientMutex.ReleaseMutex();
                spectatorMutex.WaitOne();
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
                        gameMutex.WaitOne();
                        var message = spectators[i].ReadData().Trim();
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
                                game.Spectators.Add(spectators[i]);
                                spectators.RemoveAt(i);
                                i--;
                            }
                        }
                        gameMutex.ReleaseMutex();
                    }
                }
                spectatorMutex.ReleaseMutex();
                Thread.Sleep(1);
            }
        }
    }
}
