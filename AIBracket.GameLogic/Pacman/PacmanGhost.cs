using AIBracket.GameLogic.Pacman.Coordinate;
using AIBracket.GameLogic.Pacman.Pacman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.GameLogic.Pacman.Ghost
{
    public class PacmanGhost
    {
        public enum Ghost { Blue, Pink, Red, Orange };
        public bool IsDead, IsVulnerable;
        public PacmanCoordinate Location;
        public PacmanPacman.Direction Facing { get; set; }

        public PacmanGhost()
        {
            Facing = PacmanPacman.Direction.start;
            IsDead = true;
            IsVulnerable = false;
            Location = new PacmanCoordinate(13, 11);
        }

        public PacmanCoordinate GetPosition()
        {
            return Location;
        }

        
        public void Move()
        {
            switch (Facing)
            {
                case PacmanPacman.Direction.start:
                    break;
                case PacmanPacman.Direction.up:
                    Location.Ypos--;
                    break;
                case PacmanPacman.Direction.down:
                    Location.Ypos++;
                    break;
                case PacmanPacman.Direction.left:
                    Location.Xpos--;
                    break;
                case PacmanPacman.Direction.right:
                    Location.Xpos++;
                    break;
                default:
                    Console.Error.WriteLine("Error: entered PacmanEntity.move switch default");
                    break;
            }
        }
    }
}
