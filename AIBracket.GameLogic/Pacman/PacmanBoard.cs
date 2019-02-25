using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AIBracket.GameLogic.Pacman.Board
{
    public class PacmanBoard
    {
        public enum Tile { wall, blank, dot, fruit, powerUp };
        private Tile[,] board;

        //*** New to initialize values to what they are supposed to be depending on the board
        public PacmanBoard() => board = new Tile[26, 26];

        public Tile getTile(int x, int y)
        {
            return board[x, y];          
        }

        // Called after pacman enters a consumable (fruit or dot) tile
        public void updateTile(int x, int y, Tile t)
        {
            if(board[x, y] == Tile.dot || board[x, y] == Tile.fruit)
            {
                board[x, y] = Tile.blank;
            }
            else
            {
                Console.Error.WriteLine("Error: PacmanBoard.updateTile tried to update a non consumable tile");
            }
        }

        public void openSpawn()
        {
            //*** Change the two tiles that are the spawn doors to blank
        }
    }
}
