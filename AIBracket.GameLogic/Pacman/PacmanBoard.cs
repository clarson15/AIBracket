using AIBracket.GameLogic.Pacman.Coordinate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AIBracket.GameLogic.Pacman.Board
{
    public class PacmanBoard
    {
        public enum Tile { wall, blank, dot, fruit, powerUp, portal };
        private Tile[,] Board;
        private readonly int Width, Height;
        // Portals holds coordinates of corresponding portal on the map
        private readonly PacmanCoordinate[] Portals;

        public PacmanBoard(int width = 28, int height = 31)
        {
            Width = width;
            Height = height;
            if (Width == 28 && Height == 31)
            {
                Board = new Tile[,]
                {
                    { Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.portal, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.dot, Tile.powerUp, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.powerUp, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.dot, Tile.dot, Tile.powerUp, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.blank, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.powerUp, Tile.dot, Tile.wall, Tile.wall, Tile.dot, Tile.dot, Tile.dot, Tile.dot, Tile.wall},
                    { Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.portal, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall, Tile.wall}
                };
            }
            else
            {
                Console.Error.WriteLine("Functionality not added for different dimensions");
            }

            // This finds the portals in the map and initializes the portals array based on the values it found
            Portals = new PacmanCoordinate[2]
            {
                new PacmanCoordinate(),
                new PacmanCoordinate()
            };

            int sizeOfPortal = 0;
            for (int i = 0; i < this.Board.GetLength(1); i++)
            {
                for (int j = 0; j < this.Board.GetLength(0); j++)
                {
                    if (Board[j, i] == Tile.portal)
                    {
                        Portals[sizeOfPortal].Xpos = j;
                        Portals[sizeOfPortal].Ypos = i;
                        sizeOfPortal++;
                        if (sizeOfPortal == 2)
                        {
                            return;
                        }
                    }
                }
            }
        }

        public Tile GetTile(int x, int y)
        {
            if(x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return Tile.wall;
            }
            return Board[x, y];          
        }

        public Tile GetTile(PacmanCoordinate p)
        {
            if (p.Xpos < 0 || p.Xpos >= Width || p.Ypos < 0 || p.Ypos >= Height)
            {
                return Tile.wall;
            }
            return Board[p.Xpos, p.Ypos];
        }

        // Called after pacman enters a consumable (fruit or dot) tile
        public void UpdateTile(PacmanCoordinate pos)
        {
            if (Board[pos.Xpos, pos.Ypos] == Tile.dot || Board[pos.Xpos, pos.Ypos] == Tile.fruit || Board[pos.Xpos, pos.Ypos] == Tile.powerUp)
            {
                Board[pos.Xpos, pos.Ypos] = Tile.blank;
            }
            else
            {
                Console.Error.WriteLine("Error: PacmanBoard.UpdateTile tried to update a non consumable tile");
            }
        }

        public void SpawnFruit()
        {
            var random = new Random();
            int x, y;
            while(true)
            {
                x = random.Next(1, 27);
                y = random.Next(1, 27);
                if (Board[x, y] == Tile.blank)
                {
                    Board[x, y] = Tile.fruit;
                    return;
                }
            }
        }

        /// <returns>Coordinates to the other portal on the map</returns>
        public PacmanCoordinate GetCorrespondingPortal(PacmanCoordinate p)
        {
            if(p == Portals[0])
            {
                return Portals[1];
            }
            else if(p == Portals[1])
            {
                return Portals[0];
            }
            else
            {
                Console.Error.WriteLine("Error: Coordinate {0}, {1} is not a designated portal");
                return p;
            }
        }

        
    }
}
