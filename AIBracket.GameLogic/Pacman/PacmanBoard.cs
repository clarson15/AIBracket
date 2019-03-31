using AIBracket.GameLogic.Pacman.Coordinate;
using AIBracket.GameLogic.Pacman.Pacman;
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
        public readonly int Width, Height;
        public int DotCount { get; private set; }
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
            DotCount = 0;
            for (int i = 0; i < this.Board.GetLength(1); i++)
            {
                for (int j = 0; j < this.Board.GetLength(0); j++)
                {
                    if (Board[j, i] == Tile.dot || Board[j, i] == Tile.powerUp)
                    {
                        DotCount++;
                    }
                    if (Board[j, i] == Tile.portal && sizeOfPortal < 2)
                    {
                        Portals[sizeOfPortal].Xpos = j;
                        Portals[sizeOfPortal].Ypos = i;
                        sizeOfPortal++;
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

        // Called after pacman enters a consumable (fruit, dot, or powerup) tile
        public void UpdateTile(PacmanCoordinate pos)
        {
            if (Board[pos.Xpos, pos.Ypos] == Tile.dot || Board[pos.Xpos, pos.Ypos] == Tile.powerUp)
            {
                DotCount--;
                Board[pos.Xpos, pos.Ypos] = Tile.blank;
            }
            else if (Board[pos.Xpos, pos.Ypos] == Tile.fruit)
            {
                Board[pos.Xpos, pos.Ypos] = Tile.blank;
            }
            else
            {
                Console.Error.WriteLine("Error: PacmanBoard.UpdateTile tried to update a non consumable tile");
            }
        }

        public PacmanCoordinate SpawnFruit()
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
                    return new PacmanCoordinate(x, y);
                }
            }
        }

        /// <summary>
        /// Checks the result of executing a direction based on the position of an entity
        /// </summary>
        /// <param name="d">Direction entity is trying to move towards</param>
        /// <param name="pos">Position of entity currently</param>
        /// <returns>Whether a move is valid</returns>
        public bool ValidMove(PacmanPacman.Direction d, PacmanCoordinate pos)
        {
            switch (d)
            {
                case PacmanPacman.Direction.start:
                    return true;
                case PacmanPacman.Direction.up:
                    return GetTile(pos.Xpos, pos.Ypos - 1) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.down:
                    return GetTile(pos.Xpos, pos.Ypos + 1) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.left:
                    return GetTile(pos.Xpos - 1, pos.Ypos) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.right:
                    return GetTile(pos.Xpos + 1, pos.Ypos) != PacmanBoard.Tile.wall;
            }
            return false;
        }

        public List<PacmanPacman.Direction> PotentialDirections(PacmanCoordinate pos)
        {
            List<PacmanPacman.Direction> directions = new List<PacmanPacman.Direction>(); 
            for (int i = 1; i< 5; i++)
            {
                if (ValidMove((PacmanPacman.Direction) i, pos))
                {
                    directions.Add((PacmanPacman.Direction) i);
                }
            }
            return directions;
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
