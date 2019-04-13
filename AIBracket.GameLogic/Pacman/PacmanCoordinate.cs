using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.GameLogic.Pacman.Coordinate
{
    public class PacmanCoordinate
    {
        public decimal Xpos { get; set; }
        public decimal Ypos { get; set; }

        public int XRoundPos
        {
            get
            {
                return (int)(Xpos + 0.5m);
            }
        }

        public int YRoundPos
        {
            get
            {
                return (int)(Ypos + 0.5m);
            }
        }

        public PacmanCoordinate(decimal X = 0m, decimal Y = 0m)
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

        public string FloorToString()
        {
            return $"{XRoundPos} {YRoundPos}";
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

        public static PacmanCoordinate operator+(PacmanCoordinate lhs, PacmanCoordinate rhs)
        {
            return new PacmanCoordinate(lhs.Xpos + rhs.Xpos, lhs.Ypos + rhs.Ypos);
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
