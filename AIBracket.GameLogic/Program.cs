using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIBracket.GameLogic.Pacman.Game;
using AIBracket.GameLogic.Pacman.Pacman;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIBracket.GameLogic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();
            var game = new PacmanGame();
            var random = new Random();
            var randoms = new PacmanPacman.Direction[5];
            var resetDirectionsCounter = 0;
            for (int i = 0; i < 5; i++)
            {
                randoms[i] = (PacmanPacman.Direction)random.Next(1, 5);
            }
            while (true)
            {
                if (resetDirectionsCounter > 3)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        randoms[i] = (PacmanPacman.Direction)random.Next(1, 5);
                    }
                    resetDirectionsCounter = 0;
                }
                game.UpdateGame(randoms);
                game.PrintBoard();
                resetDirectionsCounter++;
            }
        }
    }
}
