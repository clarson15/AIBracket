using AIBracket.GameLogic.Pacman.Coordinate;
using AIBracket.GameLogic.Pacman.Ghost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.GameLogic.Pacman.Pacman
{
    public class PacmanPacman
    {
        public enum Direction { start, up, down, left, right };
        public int Lives;
        public PacmanCoordinate Location;
        public Direction Facing { get; set; }
        
        public PacmanPacman()
        {
            Location = new PacmanCoordinate(13, 17);
            Facing = Direction.start;
            Lives = 3;
        }

        public PacmanCoordinate GetPosition()
        {
            return Location;
        }
        
        public void Move()
        {
            switch (Facing)
            {
                case Direction.start:
                    break;
                case Direction.up:
                    Location.Ypos--;
                    break;
                case Direction.down:
                    Location.Ypos++;
                    break;
                case Direction.left:
                    Location.Xpos--;
                    break;
                case Direction.right:
                    Location.Xpos++;
                    break;
                default:
                    Console.Error.WriteLine("Error: entered PacmanEntity.Move switch default");
                    break;
            }
        } 
        /// <summary>
        /// Returns the opposite direction
        /// </summary>
        public static Direction InverseDirection(Direction d)
        {
            switch(d)
            {
                case Direction.down:
                    return Direction.up;
                case Direction.up:
                    return Direction.down;
                case Direction.left:
                    return Direction.right;
                case Direction.right:
                    return Direction.left;
                default:
                    return Direction.start;
            }
            return d;
        }
    }
}