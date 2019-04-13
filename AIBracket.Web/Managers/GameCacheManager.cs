using AIBracket.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AIBracket.Web.Managers
{
    public static class GameCacheManager
    {
        private static string FeaturedGame { get; set; }
        private static object lockObj { get; set; } = new object();
        private static TcpClient Client { get; set; }
        private static bool isReady { get; set; } = false;
        private static byte[] receiveBuffer = new byte[1024];
        public static List<GameSummaryModel> Games { get; set; }

        public static void Start()
        {
            Thread.Sleep(1000);
            Client = new TcpClient("127.0.0.1", 8000);
            Games = new List<GameSummaryModel>();
            Client.GetStream().Write(Encoding.ASCII.GetBytes("WATCH GAMEMASTER"));
            Client.GetStream().BeginRead(receiveBuffer, 0, 1024, new AsyncCallback(ReadCallback), Client);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            TcpClient stream = (TcpClient)ar.AsyncState;
            var bytesRead = -1;
            try
            {
                bytesRead = stream.GetStream().EndRead(ar);
                var newarray = new byte[bytesRead];
                Buffer.BlockCopy(receiveBuffer, 0, newarray, 0, bytesRead);
                stream.GetStream().BeginRead(receiveBuffer, 0, 1024, new AsyncCallback(ReadCallback), stream);
                var message = Encoding.ASCII.GetString(newarray).Trim();
                lock (lockObj)
                {
                    UpdateData(message);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error receiving GameMaster data: " + e.Message);
            }
        }

        private static void UpdateData(string message)
        {
            var elements = message.Split(' ');
            switch (elements[0])
            {
                case "0":
                    if(elements.Length < 5)
                    {
                        break;
                    }
                    var newGame = new GameSummaryModel
                    {
                        GameId = elements[1],
                        GameType = int.Parse(elements[2]),
                        BotId = elements[3],
                        Score = int.Parse(elements[4])
                    };
                    Games.Add(newGame);
                    break;
                case "1":
                    if(elements.Length < 3)
                    {
                        break;
                    }
                    var gameToRemove = Games.FirstOrDefault(x => x.GameId == elements[1]);
                    if (gameToRemove != null)
                    {
                        Games.Remove(gameToRemove);
                    }
                    break;
                case "2":
                    for(int i = 1; (i + 3) < elements.Length; i+=4)
                    {
                        var newGamel = new GameSummaryModel
                        {
                            GameId = elements[i],
                            GameType = int.Parse(elements[i + 1]),
                            BotId = elements[i + 2],
                            Score = int.Parse(elements[i + 3])
                        };
                        Games.Add(newGamel);
                    }
                    break;
                case "3":
                    if(elements.Length < 5)
                    {
                        break;
                    }
                    var game = Games.FirstOrDefault(x => x.GameId == elements[1]);
                    if(game != null)
                    {
                        Games.Remove(game);
                        game.GameId = elements[2];
                        game.GameType = int.Parse(elements[3]);
                        game.Score = int.Parse(elements[4]);
                        Games.Add(game);
                    }
                    break;
                case "4":
                    for(var i = 1; i+1 < elements.Length; i += 2)
                    {
                        var newgame = Games.FirstOrDefault(x => x.GameId == elements[i]);
                        if(newgame != null)
                        {
                            newgame.Score = int.Parse(elements[i + 1]);
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Received OP code " + elements[0]);
                    break;
            }
            var fg = Games.FirstOrDefault(x => x.Score == Games.Max(y => y.Score));
            if(fg != null)
            {
                FeaturedGame = fg.GameId;
            }
            else
            {
                FeaturedGame = null;
            }
        }

        public static bool IsBotActive(string Id)
        {
            bool ret;
            lock (lockObj)
            {
                ret = Games.Count(x => x.BotId == Id) > 0;
            }
            return ret;
        }

        public static string GetFeaturedGameId()
        {
            string featured;
            lock (lockObj)
            {
                featured = FeaturedGame;
            }
            return featured;
        }
    }
}
