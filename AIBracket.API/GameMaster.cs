using System;
using System.Collections.Generic;
using System.Text;

namespace AIBracket.API
{
    public static class GameMaster
    {
        private static List<Object> games; // current games running (playing atm)
        private static List<Object> players; // current connected players
        private static Boolean isRunning; 

        public static void Initialize() { // constructor for our games list and players
            games = new List<Object>(); // <- create functions to add players and games
            players = new List<Object>();
            isRunning = false;
        }

        public static void AddGame(Object game) { // adds a new thread with the game object into our thread list
            games.Add(game);
        }

        public static void AddPlayer(Object player) {
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
            }
            // restart? print out debug stuff?
        }

        public static 
    }
}
