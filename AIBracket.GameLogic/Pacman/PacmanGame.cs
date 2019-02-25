using AIBracket.GameLogic.Pacman.Board;
using AIBracket.GameLogic.Pacman.Entity;
using AIBracket.GameLogic.Pacman.Ghost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AIBracket.GameLogic.Pacman.Game
{
   
    public class PacmanGame
    {
        private PacmanBoard board;
        private PacmanEntity pacman;
        private PacmanGhost[] ghosts;
        private DateTime timeStarted;
        private int score;
        private bool isPoweredUp;

        public PacmanGame()
        {
            PacmanEntity.lives = 3;
            board = new PacmanBoard();
            pacman = new PacmanEntity();
            for(int i = 0; i < 4; ++i)
            {
                ghosts[i] = new PacmanGhost((PacmanGhost.Ghost)i);
            }
            score = 0;
            isPoweredUp = false;
            timeStarted = new DateTime();
        }
        
        private void updateScore(PacmanBoard.Tile t)
        {
            if(t == PacmanBoard.Tile.dot)
            {
                score += 1;
            }
            else if(t == PacmanBoard.Tile.fruit)
            {
                score += 2;
            }
            return;
        }


        // This method is called after a move is made by pacman to determine whether or not to update the board.
        // It also calls update score if a fruit or dot was consumed
        public void checkTile(int pacX, int pacY, PacmanBoard.Tile t) // Delete this function and put it in this.run ??
        {
            switch (t)
            {
                case PacmanBoard.Tile.wall: //*** Code in spawn doors once we know the map coordinates for them
                    break;
                case PacmanBoard.Tile.blank:
                    break;
                case PacmanBoard.Tile.dot:
                case PacmanBoard.Tile.fruit:
                    board.updateTile(pacX, pacY, t);
                    updateScore(t);
                    break;
                default:
                    break;
            }
            return;
        }

        public void run()
        {

        }
    }
}
