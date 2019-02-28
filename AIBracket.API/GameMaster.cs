using AIBracket.API.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AIBracket.API
{
    public static class GameMaster
    {
        private static List<Object> games; // current games running (playing atm)
        private static List<ConnectedClient> players; // current connected players
        private static Boolean isRunning; 

        public static void Initialize() { // constructor for our games list and players
            games = new List<Object>(); // <- create functions to add players and games
            players = new List<ConnectedClient>();
            isRunning = false;
        }

        public static void AddGame(Object game) { // adds a new thread with the game object into our thread list
            games.Add(game);
        }

        public static void AddPlayer(ConnectedClient player) {
            players.Add(player);
        }

        public static int GetPlayerCount() {
            return players.Count; // for creating a new game
        }

        public static void InputCommand(string command)
        {
             // this will parse commands
        }

        public static async void Run() {
            isRunning = true;
            while (isRunning) {

                /*
                1. Gets player input, if any
                2. Update games with input from players
                3. Update the players with current game state, loop back to 1
                (^^^this will happen ever second -> thread.sleep() to acheive this)
                4. adds a new game for new players if needed
                */
                foreach(var client in players)
                {
                    if(client.Socket.Available > 0)
                    {
                        byte[] buffer = new byte[1024];
                        client.Socket.GetStream().Read(buffer, 0, buffer.Length);
                        var message = Encoding.ASCII.GetString(buffer);
                        Console.WriteLine("Game master read: " + message);
                    }
                }
                Thread.Sleep(1000);
            }
            // restart? print out debug stuff?
        }
    }
}
