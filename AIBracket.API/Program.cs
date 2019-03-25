﻿using System;
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
using AIBracket.API.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Cryptography;

namespace AIBracket.API
{

    class TcpHelper {
        private static AIBracketContext context = new AIBracketContext();
        private static TcpListener Listener { get; set; }
        private static bool Accept { get; set; } = false;
        private static List<TcpClient> clients = new List<TcpClient>();
        private static List<WebSocket> websockets = new List<WebSocket>();

        public static void StartServer(int port) {
            IPAddress address = IPAddress.Any;
            GameMaster.Initialize(); 
            Task.Run(() => GameMaster.Run());
            Listener = new TcpListener(address, port);
            Listener.Start();
            Accept = true;

            Console.WriteLine($"Server started. Listening to TCP clients on port {port}");
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
                    var buffer = new byte[client.Available];
                    client.GetStream().Read(buffer, 0, client.Available);

                    var message = Encoding.ASCII.GetString(buffer);
                    if (message.StartsWith("GET "))
                    {
                        if (!message.Contains("websocket"))
                        {
                            Console.WriteLine("Random GET request.");
                            clientsToRemove.Add(client);
                            continue;
                        }
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
                        websockets.Add(new WebSocket(client));
                        clientsToRemove.Add(client);
                        continue;
                    }
                    else
                    {
                        if (message.StartsWith("BOT ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var bot = VerifyBot(message.Substring(4), client, false);
                            if(bot != null)
                            {
                                Console.WriteLine("Bot connected.");
                                GameMaster.AddPlayer(bot);
                                clientsToRemove.Add(client);
                                continue;
                            }
                        }
                        else if(message.StartsWith("WATCH ", StringComparison.InvariantCultureIgnoreCase)){
                            var target = message.Substring(6);
                            if (target == "GAMEMASTER")
                            {
                                Console.WriteLine("gamemaster spectator");
                                client.GetStream().Write(Encoding.ASCII.GetBytes("OK"));
                                GameMaster.AddSpectator(new BotSocket(client));
                                clientsToRemove.Add(client);
                                continue;
                            }
                            else
                            {
                                if(GameMaster.WatchGame(new BotSocket(client), target))
                                {
                                    clientsToRemove.Add(client);
                                    continue;
                                }
                                else
                                {
                                    client.GetStream().Write(Encoding.ASCII.GetBytes("Game does not exist"));
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            var certificate = new X509Certificate2("Cert.pfx");
                            using (RSA rsa = certificate.GetRSAPrivateKey())
                            {
                                var ssl = new SslStream(client.GetStream(), false);
                                var udata = rsa.Decrypt(buffer, RSAEncryptionPadding.OaepSHA1);
                                var newmessage = Encoding.ASCII.GetString(udata);
                                Console.WriteLine(newmessage);
                                ssl.AuthenticateAsServer(certificate, false, false);
                            }
                            Console.WriteLine("Unknown client.");
                            clientsToRemove.Add(client);
                            continue;
                        }
                    }
                }
            }
            foreach(var client in clientsToRemove)
            {
                clients.Remove(client);
            }
            for(var i = 0; i < websockets.Count(); i++)
            {
                var client = websockets[i];
                if (!client.IsConnected)
                {
                    websockets.RemoveAt(i);
                    i--;
                    continue;
                }
                if (client.IsReady)
                {
                    var message = client.ReadData();
                    Console.WriteLine(message);
                    if(message.StartsWith("BOT ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var bot = VerifyBot(message.Substring(4), client.Socket, true);
                        if(bot != null)
                        {
                            GameMaster.AddPlayer(bot);
                            websockets.RemoveAt(i);
                            i--;
                        }
                    }
                    else if(message.StartsWith("WATCH ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var targets = message.Substring(6).Split(' ');
                        if (targets.Count() == 1)
                        {
                            var target = targets[0];
                            if (target == "GAMEMASTER")
                            {
                                GameMaster.AddSpectator(client);
                                websockets.Remove(client);
                                continue;
                            }
                            else
                            {
                                if (GameMaster.WatchGame(client, target))
                                {
                                    websockets.Remove(client);
                                    continue;
                                }
                                else
                                {
                                    client.WriteData("Game does not exist");
                                    websockets.Remove(client);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            var user = context.Users.Where(x => x.SpectatorId == targets[1]).FirstOrDefault();
                            if (user == null)
                            {
                                client.WriteData("SpectatorID doesn't exist");
                                websockets.Remove(client);
                                continue;
                            }
                            client.Name = user.UserName;
                            if(targets[0] == "GAMEMASTER")
                            {
                                GameMaster.AddSpectator(client);
                                websockets.Remove(client);
                                continue;
                            }
                            if(GameMaster.WatchGame(client, targets[0]))
                            {
                                websockets.Remove(client);
                                continue;
                            }
                        }
                    }
                }
            }
        }

        private static IConnectedClient VerifyBot(string input, TcpClient client, bool isWebSocket)
        {
            var bot = context.Bots.Where(x => x.PrivateKey == input.Trim()).FirstOrDefault();
            if (bot != null)
            {
                var user = context.Users.Where(x => x.Id == bot.IdentityId).FirstOrDefault();
                if (user != null)
                {
                    switch (bot.Game)
                    {
                        case 0:
                            break;
                        case 1:
                            if (isWebSocket)
                            {
                                return new PacmanClient
                                {
                                    User = user,
                                    Bot = bot,
                                    Socket = new WebSocket(client, bot.Name)
                                };
                            }
                            else
                            {
                                return new PacmanClient
                                {
                                    User = user,
                                    Bot = bot,
                                    Socket = new BotSocket(client, bot.Name)
                                };
                            }
                        default:
                            break;
                    }
                }
            }
            return null;
        }

        public static void Main(string[] args) {
            StartServer(8000);
            Listen();
        }
    }
}  
