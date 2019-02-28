using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.GameLogic.Pacman.Coordinate
{
    public class PacmanCoordinate
    {
        public int Xpos { get; set; }
        public int Ypos { get; set; }

        public PacmanCoordinate(int X = 0, int Y = 0)
        {
            this.Xpos = X;
            this.Ypos = Y;
        }
    }
}
