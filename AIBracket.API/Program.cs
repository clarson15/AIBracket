﻿using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;
using AIBracket.Data;
using AIBracket.API.Entities;
using System.Threading;
using System.Collections.Generic;
using AIBracket.API.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using AIBracket.Data.Entities;

namespace AIBracket.API
{

    class TcpHelper {
        private static AIBracketContext context = new AIBracketContext();
        private static TcpListener Listener { get; set; }
        private static TcpListener SslListener { get; set; }
        private static bool Accept { get; set; } = false;
        private static X509Certificate2 cert;
        private static Task GameMasterThread;
        private static List<ISocket> clients = new List<ISocket>();

        public static void StartServer(int port, int sslport) {
            IPAddress address = IPAddress.Any;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                cert = new X509Certificate2("Cert.pfx", "password");
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": No certificate found.");
            }
            GameMaster.Initialize(); 
            GameMasterThread = Task.Run(() => GameMaster.Run());
            Listener = new TcpListener(address, port);
            SslListener = new TcpListener(address, sslport);
            Listener.Start();
            SslListener.Start();
            Accept = true;

            Console.WriteLine($"Server started. Listening to TCP clients on port {port}, listening to SSL TCP clients on port {sslport}");
        }

        public static void ConnectClient(IAsyncResult ar)
        {
            var listener = (TcpListener) ar.AsyncState;
            var client = listener.EndAcceptTcpClient(ar);
            client.NoDelay = true;
            Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Client connected.");
            clients.Add(new InsecureSocket(client));
            listener.BeginAcceptTcpClient(ConnectClient, listener);
        }

        public static void SslConnectClient(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;
            var client = listener.EndAcceptTcpClient(ar);
            client.NoDelay = true;
            try
            {
                var sstream = new SslStream(client.GetStream(), false, (sender, cert, chain, err) => true);
                sstream.AuthenticateAsServer(cert, false, SslProtocols.Tls12, false);
                clients.Add(new SecureSocket(sstream));
                client.NoDelay = true;
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Secure client connected.");
            }
            catch(Exception e)
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Error connecting secure client: " + e.Message);
            }
            listener.BeginAcceptTcpClient(SslConnectClient, listener);
        }

        public static void Listen()
        {
            if (Listener != null && Accept)
            {
                Listener.BeginAcceptTcpClient(ConnectClient, Listener);
            }
            if(SslListener != null && Accept)
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Listening for SSL connections");
                SslListener.BeginAcceptTcpClient(SslConnectClient, SslListener);
            }
            while (true)
            {
                DiscoverIntentions();
                Thread.Sleep(10);
                if(GameMasterThread.Status != TaskStatus.Running)
                {
                    GameMasterThread.Start();
                    Console.WriteLine("Restarting GameMaster main thread");
                }
            }
        }
        
        public static void DiscoverIntentions()
        {
            for(var i = 0; i < clients.Count; i++)
            {
                if (!clients[i].IsConnected)
                {
                    clients.RemoveAt(i);
                    i--;
                    continue;
                }
                if (clients[i].IsReady)
                {
                    var message = clients[i].ReadData();
                    if (message.StartsWith("BOT ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var bot = VerifyBot(message.Substring(4), clients[i]);
                        if (bot != null)
                        {
                            if (GameMaster.IsPlayerConnected(bot))
                            {
                                bot.Socket.WriteData("Bot is already connected.");
                            }
                            else
                            {
                                GameMaster.AddPlayer(bot);
                            }
                            clients.RemoveAt(i);
                            i--;
                            continue;
                        }
                        else
                        {
                            clients[i].WriteData("Incorrect Bot ID");
                            clients.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    else if (message.StartsWith("WATCH ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var target = message.Substring(6);
                        if (target == "GAMEMASTER")
                        {
                            GameMaster.AddSpectator(clients[i]);
                            clients.RemoveAt(i);
                            i--;
                            continue;
                        }
                        else
                        {
                            var targets = target.Split(' ');
                            if (targets.Length > 2)
                            {
                                var user = context.Users.FirstOrDefault(x => x.SpectatorId == targets[2]);
                                if (user != null)
                                {
                                    clients[i].Name = user.UserName;
                                }
                            }
                            if (targets[0] == "GAME")
                            {
                                if (GameMaster.WatchGame(clients[i], targets[1]))
                                {
                                    clients.RemoveAt(i);
                                    i--;
                                    continue;
                                }
                                else
                                {
                                    clients[i].WriteData("Game does not exist");
                                    continue;
                                }
                            }
                            else if(targets[0] == "BOT")
                            {
                                if(context.Bots.Count(x => x.Id.ToString() == targets[1]) == 1)
                                {
                                    GameMaster.WatchBot(clients[i], targets[1]);
                                    clients.RemoveAt(i);
                                    i--;
                                    continue;
                                }
                                else
                                {
                                    clients[i].WriteData("Bot does not exist");
                                    continue;
                                }
                            }
                        }
                    }
                    else if (message.StartsWith("DEBUG", StringComparison.InvariantCultureIgnoreCase))
                    {
                        clients[i].WriteData(GameMaster.GetDebugInfo());
                        clients.RemoveAt(i);
                        i--;
                        continue;
                    }
                    else
                    {
                        clients[i].WriteData("Unknown command.");
                        clients.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
        }

        private static IConnectedClient VerifyBot(string input, ISocket client)
        {
            var bot = context.Bots.Where(x => x.PrivateKey == input.Trim()).FirstOrDefault();
            if (bot != null)
            {
                client.Name = bot.Name;
                var user = context.Users.Where(x => x.Id == bot.IdentityId).FirstOrDefault();
                if (user != null)
                {
                    switch (bot.Game)
                    {
                        case 0:
                            break;
                        case 1:
                            return new PacmanClient
                            {
                                User = user,
                                Bot = bot,
                                Socket = client
                            };
                        default:
                            break;
                    }
                }
            }
            return null;
        }

        public static void Main(string[] args) {
            StartServer(8000, 8005);
            Listen();
        }
    }
}  
