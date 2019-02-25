using AIBracket.GameLogic.Pacman.Ghost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.GameLogic.Pacman.Entity
{
    public class PacmanEntity
    {
        public enum direction { start, up, down, left, right };
        public static int lives;
        protected int xpos, ypos;
        protected direction facing { get; set; }
        
        public PacmanEntity()
        {
            xpos = 10; //*** Change these
            ypos = 11;
            facing = direction.start;
            lives--;
        }

        // Ghost Base Constructor
        public PacmanEntity(PacmanGhost.Ghost g) 
        {
            switch (g)
            { //*** Determine starting positions
                case PacmanGhost.Ghost.Blue:
                    xpos = 10; //*** Change these
                    ypos = 11;
                    facing = direction.start;
                    break;
                case PacmanGhost.Ghost.Pink:
                    xpos = 10; //*** Change these
                    ypos = 11;
                    facing = direction.start;
                    break;
                case PacmanGhost.Ghost.Red:
                    xpos = 10; //*** Change these
                    ypos = 11;
                    facing = direction.start;
                    break;
                case PacmanGhost.Ghost.Orange:
                    xpos = 10; //*** Change these
                    ypos = 11;
                    facing = direction.start;
                    break;
            }
        }

        public Object getPosition()
        {
            return new { xpos, ypos };
        }

        public void move()
        {
            switch (this.facing)
            {
                case direction.start:
                    break;
                case direction.up:
                    ypos++;
                    break;
                case direction.down:
                    ypos--;
                    break;
                case direction.left:
                    xpos--;
                    break;
                case direction.right:
                    xpos++;
                    break;
                default:
                    Console.Error.WriteLine("Error: entered PacmanEntity.move switch default");
                    break;
            }
        }
        
    
    }
}
