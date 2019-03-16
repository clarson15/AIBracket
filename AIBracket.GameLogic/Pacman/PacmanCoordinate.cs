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

        public PacmanCoordinate(PacmanCoordinate p)
        {
            this.Xpos = p.Xpos;
            this.Ypos = p.Ypos;
        }

        public static bool operator==(PacmanCoordinate lhs, PacmanCoordinate rhs)
        {
            return lhs.Xpos == rhs.Xpos && lhs.Ypos == rhs.Ypos;
        }

        public static bool operator!=(PacmanCoordinate lhs, PacmanCoordinate rhs)
        {
            return lhs.Xpos != rhs.Xpos && lhs.Ypos != rhs.Ypos;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
