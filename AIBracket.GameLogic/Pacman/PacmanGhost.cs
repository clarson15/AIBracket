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
            Location = new PacmanCoordinate(13, 10);
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

        /// <summary>
        /// Finds a random direction for the ghost prioritizing not going backwards
        /// </summary>
        /// <param name="d">Direction or Facing</param>
        /// <param name="pos">Location</param>
        /// <returns>New Direction</returns>
        public PacmanPacman.Direction DetermineGhostMoveEasy(List<PacmanPacman.Direction> possible)
        {
            var random = new Random();
            if (possible.Count > 1)
            {
                for (int i = 0; i < possible.Count; i++)
                {
                    if (possible[i] == PacmanPacman.InverseDirection(Facing)) 
                    {
                        possible.RemoveAt(i);
                        break;
                    }
                }
                return possible[random.Next(0, possible.Count)];
            }
            return PacmanPacman.Direction.start;
        }

        public PacmanPacman.Direction DetermineGhostMoveMedium(List<PacmanPacman.Direction> possible, PacmanCoordinate pos)
        {
            var random = new Random();
            var difference = Location - pos;
            if (possible.Contains(PacmanPacman.InverseDirection(Facing)))
            {
                possible.Remove(PacmanPacman.InverseDirection(Facing));
            }
            if (possible.Count > 0)
            {
                if (Math.Abs(difference.Xpos) > Math.Abs(difference.Ypos))
                {
                    if (difference.Xpos > 0)
                    {
                        if (possible.Contains(PacmanPacman.Direction.left))
                        {
                            return PacmanPacman.Direction.left;
                        }
                    }
                    else
                    {
                        if (possible.Contains(PacmanPacman.Direction.right))
                        {
                            return PacmanPacman.Direction.right;
                        }
                    }
                }
                else
                {
                    if (difference.Ypos > 0)
                    {
                        if (possible.Contains(PacmanPacman.Direction.up))
                        {
                            return PacmanPacman.Direction.up;
                        }
                    }
                    else
                    {
                        if (possible.Contains(PacmanPacman.Direction.down))
                        {
                            return PacmanPacman.Direction.down;
                        }
                    }
                }
                return possible[random.Next(0, possible.Count)];
            }
            return PacmanPacman.Direction.start;
        }
    }
}
