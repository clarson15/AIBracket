using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIBracket.Web.Managers
{
    public static class GameCacheManager
    {
        private static DateTime LastUpdated { get; set; }
        private static string FeaturedGame { get; set; }
        private static TcpClient Client { get; set; }
        private static bool isReady { get; set; } = false;

        static GameCacheManager()
        {
            LastUpdated = DateTime.UnixEpoch;
            Client = new TcpClient("127.0.0.1", 8000);
            Client.GetStream().Write(Encoding.ASCII.GetBytes("WATCH GAMEMASTER"));
            var timeout = DateTime.Now.AddSeconds(2);
            while (Client.Available == 0)
            {
                if(DateTime.Now - timeout > TimeSpan.FromSeconds(0))
                {
                    LastUpdated = DateTime.Now;
                    return;
                }
                Thread.Sleep(0);
            }
            isReady = true;
            var ready = Client.Available;
            var bytes = new byte[ready];
            Client.GetStream().Read(bytes, 0, ready);
            UpdateData();
        }

        private static void UpdateData()
        {
            if (!isReady)
            {
                return;
            }
            Client.GetStream().Write(Encoding.ASCII.GetBytes("LIST PACMAN")); 
            var timeout = DateTime.Now.AddSeconds(2);
            while(Client.Available == 0)
            {
                if(DateTime.Now - timeout > TimeSpan.FromSeconds(0))
                {
                    LastUpdated = DateTime.Now;
                    return;
                }
                Thread.Sleep(0);
            }
            var ready = Client.Available;
            var bytes = new byte[ready];
            Client.GetStream().Read(bytes, 0, ready);
            var message = Encoding.ASCII.GetString(bytes);
            var games = message.Split('\n');
            var parsedGames = new List<KeyValuePair<string, int>>();
            foreach(var game in games)
            {
                if (string.IsNullOrWhiteSpace(game))
                {
                    break;
                }
                var data = game.Split(' ');
                if(data.Count() != 2)
                {
                    return;
                }
                var newGame = new KeyValuePair<string, int>(data[0], int.Parse(data[1]));
                parsedGames.Add(newGame);
            }
            if(games.Count() == 0)
            {
                return;
            }
            FeaturedGame = parsedGames.First(y => parsedGames.Max(x => x.Value) == y.Value).Key;
            LastUpdated = DateTime.Now;
        }

        public static string GetFeaturedGameId()
        {
            if(DateTime.Now - LastUpdated > TimeSpan.FromSeconds(10))
            {
                UpdateData();
            }
            return FeaturedGame;
        }
    }
}
