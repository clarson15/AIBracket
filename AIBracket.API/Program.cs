using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;
using AIBracket.Data;
using AIBracket.Data.Entities;
using AIBracket.API.Entities;
using System.Threading;
using System.Collections.Generic;

namespace AIBracket.API
{

    class TcpHelper {
        private static TcpListener Listener { get; set; }
        private static bool Accept { get; set; } = false;
        private static Mutex mut = new Mutex();
        private static List<TcpClient> clients = new List<TcpClient>();

        public static void StartServer(int port) {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            GameMaster.Initialize(); 
            Task.Run(() => GameMaster.Run());
            Listener = new TcpListener(address, port);
            Listener.Start();
            Accept = true;

            Console.WriteLine($"Server started. Listening to TCP clients at 127.0.0.1:{port}");
        }

        public static void ConnectClient(IAsyncResult ar)
        {
            var listener = (TcpListener) ar.AsyncState;
            var client = listener.EndAcceptTcpClient(ar);
            client.NoDelay = true;
            Console.WriteLine("Client connected.");
            clients.Add(client);
            Listener.BeginAcceptTcpClient(ConnectClient, Listener);
        }

        public static void Listen()
        {
            if (Listener != null && Accept)
            {
                Listener.BeginAcceptTcpClient(ConnectClient, Listener);
                Console.WriteLine("Waiting for clients...");
                while (true)
                {
                    DiscoverIntentions();
                    Thread.Sleep(10);
                }
            }
        }

        public static void DiscoverIntentions()
        {
            var clientsToRemove = new List<TcpClient>();
            foreach(var client in clients)
            {
                if (!client.Connected)
                {
                    clientsToRemove.Add(client);
                    continue;
                }
                if(client.Available > 0)
                {
                    Console.WriteLine("Client is ready");
                    var buffer = new byte[client.Available];
                    client.GetStream().Read(buffer, 0, client.Available);

                    var message = Encoding.ASCII.GetString(buffer);
                    using (var context = new AIBracketContext())
                    {
                        if (new System.Text.RegularExpressions.Regex("^GET").IsMatch(message))
                        {
                            const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

                            byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                                + "Connection: Upgrade" + eol
                                + "Upgrade: websocket" + eol
                                + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                                    System.Security.Cryptography.SHA1.Create().ComputeHash(
                                        Encoding.UTF8.GetBytes(
                                            new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(message).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                        )
                                    )
                                ) + eol
                                + eol);

                            client.GetStream().Write(response, 0, response.Length);
                            Console.WriteLine("Websocket connected.");
                            GameMaster.AddPlayer(new PacmanClient
                            {
                                Socket = client,
                                User = new AppUser(),
                                Bot = new Bot(),
                                IsWebSocket = true
                            });
                        }
                        else
                        {
                            var bot = context.Bots.Where(x => x.PrivateKey == message.Trim()).FirstOrDefault();
                            if (bot != null)
                            {
                                var user = context.Users.Where(x => x.Id == bot.IdentityId).FirstOrDefault();
                                if (user != null)
                                {
                                    Console.WriteLine("Success. Bot " + bot.Name + " connected from User " + user.UserName);
                                    GameMaster.AddPlayer(new PacmanClient
                                    {
                                        Socket = client,
                                        User = user,
                                        Bot = bot,
                                        IsWebSocket = false
                                    });
                                }
                                else
                                {
                                    Console.WriteLine("User not found for bot. This should never happen.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Bot not found with secret \"" + message + "\"");
                                message = "Invalid secret";
                                client.GetStream().Write(Encoding.ASCII.GetBytes(message));
                                client.Close();
                            }
                        }
                    }
                }
            }
            foreach(var client in clientsToRemove)
            {
                clients.Remove(client);
            }
        }

        public static void Main(string[] args) {
            StartServer(8000);
            Listen();
        }
    }
}  
