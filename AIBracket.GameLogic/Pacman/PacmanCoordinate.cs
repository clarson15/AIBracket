using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.GameLogic.Pacman.Coordinate
{
    public class PacmanCoordinate
    {
        public decimal Xpos
        {
            get
            {
                return Math.Round(xpos, 1);
            }
            set
            {
                xpos = value;
            }
        }
        public decimal Ypos
        {
            get
            {
                return Math.Round(ypos, 1);
            }
            set
            {
                ypos = value;
            }
        }
        private decimal xpos;
        private decimal ypos;

        public PacmanCoordinate(decimal X = 0, decimal Y = 0)
        {
            Xpos = X;
            Ypos = Y;
        }

        public PacmanCoordinate(PacmanCoordinate p)
        {
            Xpos = p.Xpos;
            Ypos = p.Ypos;
        }

        public override string ToString()
        {
            return $"{Xpos} {Ypos}";
        }

        public static bool operator==(PacmanCoordinate lhs, PacmanCoordinate rhs)
        {
            return lhs.Xpos == rhs.Xpos && lhs.Ypos == rhs.Ypos;
        }

        public static bool operator!=(PacmanCoordinate lhs, PacmanCoordinate rhs)
        {
            return lhs.Xpos != rhs.Xpos && lhs.Ypos != rhs.Ypos;
        }

        public static PacmanCoordinate operator-(PacmanCoordinate lhs, PacmanCoordinate rhs)
        {
            return new PacmanCoordinate(lhs.Xpos - rhs.Xpos, lhs.Ypos - rhs.Ypos);
        }

        public bool Collide(PacmanCoordinate rhs)
        {
            return (Xpos < rhs.Xpos + 1 && Xpos + 1 > rhs.Xpos && Ypos < rhs.Ypos + 1 && Ypos + 1 > rhs.Ypos);
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
