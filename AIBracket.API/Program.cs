using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using AIBracket.GameLogic;
namespace AIBracket.API
{

    class TcpHelper {
        private static TcpListener listener { get; set; }
        private static bool accept { get; set; } = false;

        public static void StartServer(int port) {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            GameMaster.Initialize(); // create game master
            GameMaster.Run();
            /* 
            initialize game master: <- this will handle all of our games and solo queue
                1.setup up the game types
                2.add connected player to match queue
                3.when 2 players or more connected start a thread with a game in it between the 2 players
                5.wait until the game finish and get the final results. 
                6.put players back into match queue and start back at step 3
                7.take match data and replay data and push it to the database
                8.END THREAD
             */
            listener = new TcpListener(address, port);
            listener.Start();
            accept = true;

            Console.WriteLine($"Server started. Listening to TCP clients at 127.0.0.1:{port}");
        }
        public static void Listen()
        {
            if (listener != null && accept)
            {

                // Continue listening.  
                while (true)
                {
                    Console.WriteLine("Waiting for client...");
                    var clientTask = listener.AcceptTcpClientAsync(); // Get the client  

                    if (clientTask.Result != null)
                    {
                        // create thread here
                        Console.WriteLine("Client connected. Waiting for data.");
                        var client = clientTask.Result;
                        string message = "";

                        while (message != null && !message.StartsWith("quit"))
                        {
                            byte[] data = Encoding.ASCII.GetBytes("Send next data: [enter 'quit' to terminate] ");
                            client.GetStream().Write(data, 0, data.Length);

                            byte[] buffer = new byte[1024];
                            client.GetStream().Read(buffer, 0, buffer.Length);

                            message = Encoding.ASCII.GetString(buffer);
                            Console.WriteLine(message); 
                        }
                        Console.WriteLine("Closing connection.");
                        client.GetStream().Dispose();
                    }
                }
            }
        }
        public static void Main(string[] args) {
            StartServer(8000);


        }
    }
}  
