using AIBracket.GameLogic.Pacman.Board;
using AIBracket.GameLogic.Pacman.Pacman;
using AIBracket.GameLogic.Pacman.Ghost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIBracket.GameLogic.Pacman.Coordinate;

namespace AIBracket.GameLogic.Pacman.Game
{
   
    public class PacmanGame
    {
        public PacmanBoard Board { get; private set; }
        public PacmanPacman Pacman { get; private set; }
        public PacmanGhost[] Ghosts { get; private set; }
        public  DateTime TimeStarted { get; private set; }
        public DateTime TimeEnded { get; private set; }
        public int Score { get; private set; }
        private int GhostScoreMultiplier { get; set; }
        private int SpawnGhostCounter { get; set; }
        private int PoweredUpCounter { get; set; }
        private int FruitSpawnCounter { get; set; }
        private bool SlowGhosts { get; set; }
        public bool GameRunning { get; private set; }

        public PacmanGame()
        {
            Board = new PacmanBoard();
            Pacman = new PacmanPacman();
            Ghosts = new PacmanGhost[4]
            {
                new PacmanGhost(),
                new PacmanGhost(),
                new PacmanGhost(),
                new PacmanGhost()
            };
            Random random = new Random();
            foreach (var g in Ghosts)
            {
                g.Facing = (PacmanPacman.Direction)random.Next(1, 5);
            }
            Score = 0;
            GhostScoreMultiplier = 1;
            SpawnGhostCounter = 10;
            PoweredUpCounter = 0;
            FruitSpawnCounter = 0;
            SlowGhosts = false;
            TimeStarted = DateTime.Now;
            GameRunning = true;
        }

        /// <summary>
        /// Updates score based on tiles that pacman enters
        /// </summary>
        /// <param name="t">Tile entered by pacman</param>
        private void UpdateScore(PacmanBoard.Tile t)
        {
            if(t == PacmanBoard.Tile.dot)
            {
                Score += 10;
            }
            else if(t == PacmanBoard.Tile.fruit)
            {
                Score += 100;
            }
            else if(t == PacmanBoard.Tile.powerUp)
            {
                Score += 50;
            }
            return;
        }

        /// <summary>
        /// Call after all dots and powerUp are consumed
        /// </summary>
        private void ResetBoard()
        {
            Board = new PacmanBoard();
            Pacman = new PacmanPacman();
            Ghosts = new PacmanGhost[]
            {
                new PacmanGhost(),
                new PacmanGhost(),
                new PacmanGhost(),
                new PacmanGhost()
            };
        }

        /// <summary>
        /// Checks the result of executing a direction based on the position of an entity
        /// </summary>
        /// <param name="d">Direction entity is trying to move towards</param>
        /// <param name="pos">Position of entity currently</param>
        /// <returns>Whether a move is valid</returns>
        private bool ValidMove(PacmanPacman.Direction d, PacmanCoordinate pos)
        {
            switch(d)
            {
                case PacmanPacman.Direction.start:
                    return true;
                case PacmanPacman.Direction.up:
                    return Board.GetTile(pos.Xpos, pos.Ypos - 1) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.down:
                    return Board.GetTile(pos.Xpos, pos.Ypos + 1) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.left:
                    return Board.GetTile(pos.Xpos - 1, pos.Ypos) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.right:
                    return Board.GetTile(pos.Xpos + 1, pos.Ypos) != PacmanBoard.Tile.wall;
            }
            return false;
        }

        /// <summary> This method is called after a move is made by pacman to determine whether or not to update the board.
        /// It also calls update score if a fruit or dot was consumed </summary>
        private void CheckTile(PacmanCoordinate pos) 
        {
            switch (Board.GetTile(pos))
            {
                case PacmanBoard.Tile.portal:
                    Pacman.Location = new PacmanCoordinate(Board.GetCorrespondingPortal(pos));
                    break;
                case PacmanBoard.Tile.powerUp:
                    foreach (var g in Ghosts)
                    {
                        if(!g.IsVulnerable && !g.IsDead)
                        {
                            g.IsVulnerable = true;
                            PoweredUpCounter = 30;
                        }
                    }
                    UpdateScore(Board.GetTile(pos));
                    Board.UpdateTile(pos);
                    if (Board.DotCount <= 0)
                        ResetBoard();
                    GhostScoreMultiplier = 1;
                    break;
                case PacmanBoard.Tile.dot:
                case PacmanBoard.Tile.fruit:
                    UpdateScore(Board.GetTile(pos));
                    Board.UpdateTile(pos);
                    if (Board.DotCount <= 0)
                        ResetBoard();
                    break;
                default:
                    break;
            }
            return;
        }

        private void PortalGhost(PacmanCoordinate pos, int i)
        {
            if(Board.GetTile(pos) == PacmanBoard.Tile.portal)
            {
                Ghosts[i].Location = new PacmanCoordinate(Board.GetCorrespondingPortal(pos));
            }
        }

        private void SpawnGhost()
        {
            if(SpawnGhostCounter <= 0)
            {
                foreach (var g in Ghosts)
                {
                    if(g.IsDead == true)
                    {
                        g.IsDead = false;
                        SpawnGhostCounter = 10;
                        return;
                    }
                }
            }
            SpawnGhostCounter--;
        }

        /// <summary>
        /// Checks whether a ghost has collided with pacman and handles resetting positions after death
        /// </summary>
        private void PacmanGhostCollide()
        {
            for (int i = 0; i < Ghosts.Length; i++)
            {
                if (Ghosts[i].GetPosition() == Pacman.GetPosition())
                {
                    if (Ghosts[i].IsVulnerable)
                    {
                        Ghosts[i].IsDead = true;
                        Ghosts[i].IsVulnerable = false;
                        Ghosts[i].Location.Xpos = 13;
                        Ghosts[i].Location.Ypos = 11;
                        Score += 200 * GhostScoreMultiplier;

                    }
                    else if(!Ghosts[i].IsDead)
                    {
                        Pacman.Location.Xpos = 13;
                        Pacman.Location.Ypos = 17;
                        Pacman.Facing = PacmanPacman.Direction.right;
                        Pacman.Lives--;
                        if (Pacman.Lives == 0)
                        {
                            GameRunning = false;
                            return;
                        }
                        Ghosts = new PacmanGhost[4]
                        {
                            new PacmanGhost(),
                            new PacmanGhost(),
                            new PacmanGhost(),
                            new PacmanGhost()
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Finds a random direction for the ghost prioritizing not going backwards
        /// </summary>
        /// <param name="d">Direction or Facing</param>
        /// <param name="pos">Location</param>
        /// <returns>New Direction</returns>
        private PacmanPacman.Direction DetermineGhostMove(PacmanPacman.Direction d, PacmanCoordinate pos)
        {
            var random = new Random();
            List<PacmanPacman.Direction> directions = new List<PacmanPacman.Direction>(); 
            for (int i = 1; i < 5; i++)
            {
                if (ValidMove((PacmanPacman.Direction)i, pos))
                {
                    directions.Add((PacmanPacman.Direction)i);
                }
            }
            if(directions.Count > 1)
            {
                for (int i = 0; i < directions.Count; i++)
                {
                    if(directions[i] == PacmanPacman.InverseDirection(d))
                    {
                        directions.RemoveAt(i);
                        break;
                    }
                }
            }
            return directions[random.Next(0, directions.Count)];
        }
        
        /// <summary>Processes every tick of the game base on directions of each entity passed in the array
        /// p should be passed 5 directions
        /// p[0] represents Pacman
        /// p[1] through p[4] represent ghosts in order passed </summary>
        
        public void UpdateGame(PacmanPacman.Direction p)
        {
            SpawnGhost();
            if(FruitSpawnCounter == 60)
            {
                Board.SpawnFruit();
                FruitSpawnCounter = -1;
            }
            FruitSpawnCounter++;

            
            // Move ghosts
            for (int i = 0; i < Ghosts.Length; i++)
            {
                if (Ghosts[i].IsDead)
                    continue;
                if (Ghosts[i].IsVulnerable)
                {
                    if (!SlowGhosts)
                    {
                        Ghosts[i].Move();
                        Ghosts[i].Facing = DetermineGhostMove(Ghosts[i].Facing, Ghosts[i].Location);
                        PortalGhost(Ghosts[i].Location, i);
                    }
                }
                else
                {
                    Ghosts[i].Move();
                    Ghosts[i].Facing = DetermineGhostMove(Ghosts[i].Facing, Ghosts[i].Location);
                    PortalGhost(Ghosts[i].Location, i);
                }
            }
            SlowGhosts = !SlowGhosts;
            PacmanGhostCollide();

            // Move Pacman
            if (ValidMove(p, Pacman.GetPosition()))
            {
                Pacman.Facing = p;
            }
            if(ValidMove(Pacman.Facing, Pacman.GetPosition()))
            {
                Pacman.Move();
                CheckTile(Pacman.GetPosition());
            }

            PacmanGhostCollide();

            if (PoweredUpCounter == 0)
            {
                foreach (var g in Ghosts)
                {
                    g.IsVulnerable = false;
                }
                GhostScoreMultiplier = 1;
            }
            if (PoweredUpCounter > -1)
            {
                PoweredUpCounter--;
            }
        }

        public void PrintBoard()
        {
            Console.WriteLine("Score: {0} Lives: {1}", Score, Pacman.Lives);
            bool foundGhost;
            char tile = 'e';
            for (int i = 0; i < 31; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    foundGhost = false;
                    if (Pacman.Location.Xpos == j && Pacman.Location.Ypos == i)
                    {
                        tile = '<';
                        Console.Write("{0} ", tile);
                        continue;
                    }
                    else
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (Ghosts[k].Location.Xpos == j && Ghosts[k].Location.Ypos == i)
                            {
                                tile = Ghosts[k].IsVulnerable ? '~' : '#';
                                foundGhost = true;
                            }
                        }

                        if (!foundGhost)
                        {
                            switch (Board.GetTile(j, i))
                            {
                                case PacmanBoard.Tile.blank:
                                    tile = ' ';
                                    break;
                                case PacmanBoard.Tile.dot:
                                    tile = '.';
                                    break;
                                case PacmanBoard.Tile.fruit:
                                    tile = 'F';
                                    break;
                                case PacmanBoard.Tile.powerUp:
                                    tile = '!';
                                    break;
                                case PacmanBoard.Tile.wall:
                                    tile = 'X';
                                    break;
                                default:
                                    tile = '?';
                                    break;
                            }
                        }
                        Console.Write("{0} ", tile);
                    }
                }
                Console.Write("\n");
            }
        }
    }
}
