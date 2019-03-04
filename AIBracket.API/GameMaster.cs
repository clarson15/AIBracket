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
            if (!player.IsWebSocket)
            {
                if(player.Bot.Game == 1)
                {
                    games.Add(new GamePacman
                    {
                        Game = new PacmanGame(),
                        User = (PacmanClient)player
                    });
                    return;
                }
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
                    if(client.Socket.Available > 0)
                    {
                        byte[] buffer = new byte[client.Socket.Available];
                        client.Socket.GetStream().Read(buffer, 0, client.Socket.Available);
                        string message;
                        if (client.IsWebSocket)
                        {
                            message = GetDecodedData(buffer, buffer.Length);
                        }
                        else
                        {
                            message = Encoding.ASCII.GetString(buffer);
                        }
                        Console.WriteLine("Game master read: " + message);
                        client.Socket.GetStream().Write( EncodeMessageToSend("Hello World") );

                    }
                }
                mut.ReleaseMutex();
            }
        }

        private static string GetDecodedData(byte[] buffer, int length)
        {
            byte b = buffer[1];
            int dataLength = 0;
            int totalLength = 0;
            int keyIndex = 0;

            if (b - 128 <= 125)
            {
                dataLength = b - 128;
                keyIndex = 2;
                totalLength = dataLength + 6;
            }

            if (b - 128 == 126)
            {
                dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
                keyIndex = 4;
                totalLength = dataLength + 8;
            }

            if (b - 128 == 127)
            {
                dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
                keyIndex = 10;
                totalLength = dataLength + 14;
            }

            if (totalLength > length)
                throw new Exception("The buffer length is small than the data length");

            byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

            int dataIndex = keyIndex + 4;
            int count = 0;
            for (int i = dataIndex; i < totalLength; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
                count++;
            }

            return Encoding.ASCII.GetString(buffer, dataIndex, dataLength);
        }

        private static byte[] EncodeMessageToSend(string message)
        {
            byte[] response;
            byte[] bytesRaw = Encoding.UTF8.GetBytes(message);
            byte[] frame = new byte[10];

            int indexStartRawData = -1;
            int length = bytesRaw.Length;

            frame[0] = (byte)129;
            if (length <= 125)
            {
                frame[1] = (byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (byte)126;
                frame[2] = (byte)((length >> 8) & 255);
                frame[3] = (byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (byte)127;
                frame[2] = (byte)((length >> 56) & 255);
                frame[3] = (byte)((length >> 48) & 255);
                frame[4] = (byte)((length >> 40) & 255);
                frame[5] = (byte)((length >> 32) & 255);
                frame[6] = (byte)((length >> 24) & 255);
                frame[7] = (byte)((length >> 16) & 255);
                frame[8] = (byte)((length >> 8) & 255);
                frame[9] = (byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new byte[indexStartRawData + length];

            int i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }
    }
}
