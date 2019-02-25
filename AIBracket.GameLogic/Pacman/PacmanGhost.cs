using AIBracket.GameLogic.Pacman.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.GameLogic.Pacman.Ghost
{
    public class PacmanGhost : PacmanEntity
    {
        public enum Ghost { Blue, Pink, Red, Orange };
        public PacmanGhost(Ghost g) : base(g)
        {
           
        }
    }
}
