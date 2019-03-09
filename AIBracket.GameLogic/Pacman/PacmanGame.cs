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
        public PacmanBoard board { get; private set; }
        public PacmanPacman pacman { get; private set; }
        public PacmanGhost[] ghosts { get; private set; }
        public  DateTime TimeStarted { get; private set; }
        public DateTime TimeEnded { get; private set; }
        public int score { get; private set; }
        private int GhostScoreMultiplier { get; set; }
        private int SpawnGhostCounter { get; set; }
        private int PoweredUpCounter { get; set; }
        private int FruitSpawnCounter { get; set; }
        public bool GameRunning { get; private set; }

        public PacmanGame()
        {
            board = new PacmanBoard();
            pacman = new PacmanPacman();
            ghosts = new PacmanGhost[4]
            {
                new PacmanGhost(),
                new PacmanGhost(),
                new PacmanGhost(),
                new PacmanGhost()
            };
            Random random = new Random();
            foreach (var g in ghosts)
            {
                g.Facing = (PacmanPacman.Direction)random.Next(1, 5);
            }
            score = 0;
            GhostScoreMultiplier = 1;
            SpawnGhostCounter = 10;
            PoweredUpCounter = 0;
            FruitSpawnCounter = 0;
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
                score += 10;
            }
            else if(t == PacmanBoard.Tile.fruit)
            {
                score += 100;
            }
            else if(t == PacmanBoard.Tile.powerUp)
            {
                score += 50;
            }
            return;
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
                    return board.GetTile(pos.Xpos, pos.Ypos - 1) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.down:
                    return board.GetTile(pos.Xpos, pos.Ypos + 1) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.left:
                    return board.GetTile(pos.Xpos - 1, pos.Ypos) != PacmanBoard.Tile.wall;
                case PacmanPacman.Direction.right:
                    return board.GetTile(pos.Xpos + 1, pos.Ypos) != PacmanBoard.Tile.wall;
            }
            return false;
        }

        /// <summary> This method is called after a move is made by pacman to determine whether or not to update the board.
        /// It also calls update score if a fruit or dot was consumed </summary>
        private void CheckTile(PacmanCoordinate pos) 
        {
            switch (board.GetTile(pos))
            {
                case PacmanBoard.Tile.portal:
                    pacman.Location = board.GetCorrespondingPortal(pos);
                    break;
                case PacmanBoard.Tile.powerUp:
                    foreach (var g in ghosts)
                    {
                        if(!g.IsVulnerable && !g.IsDead)
                        {
                            g.IsVulnerable = true;
                            PoweredUpCounter = 30;
                        }
                    }
                    UpdateScore(board.GetTile(pos));
                    GhostScoreMultiplier = 1;
                    break;
                case PacmanBoard.Tile.dot:
                case PacmanBoard.Tile.fruit:
                    UpdateScore(board.GetTile(pos));
                    board.UpdateTile(pos);
                    break;
                default:
                    break;
            }
            return;
        }

        private void PortalGhost(PacmanCoordinate pos, int i)
        {
            if(board.GetTile(pos) == PacmanBoard.Tile.portal)
            {
                ghosts[i].Location = board.GetCorrespondingPortal(pos);
            }
        }

        private void SpawnGhost()
        {
            if(SpawnGhostCounter <= 0)
            {
                foreach (var g in ghosts)
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
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (ghosts[i].GetPosition() == pacman.GetPosition())
                {
                    if (ghosts[i].IsVulnerable)
                    {
                        ghosts[i].IsDead = true;
                        ghosts[i].IsVulnerable = false;
                        ghosts[i].Location.Xpos = 13;
                        ghosts[i].Location.Ypos = 11;
                        score += 200 * GhostScoreMultiplier;

                    }
                    else
                    {
                        if(pacman.Lives == 0)
                        {
                            GameRunning = false;
                        }
                        else
                        {
                            pacman.Location.Xpos = 13;
                            pacman.Location.Ypos = 17;
                            pacman.Facing = PacmanPacman.Direction.start;
                            pacman.Lives--;
                        }
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
                board.SpawnFruit();
                FruitSpawnCounter = -1;
            }
            FruitSpawnCounter++;

            
            // Move ghosts
            for (int i = 0; i < ghosts.Length; i++)
            {
                ghosts[i].Facing = DetermineGhostMove(ghosts[i].Facing, ghosts[i].Location);
                ghosts[i].Move();
                PortalGhost(ghosts[i].Location, i);
            }

            PacmanGhostCollide();

            // Move Pacman
            if (ValidMove(p, pacman.GetPosition()))
            {
                pacman.Facing = p;
            }
            if(ValidMove(pacman.Facing, pacman.GetPosition()))
            {
                pacman.Move();
                CheckTile(pacman.GetPosition());
            }

            PacmanGhostCollide();

            if (PoweredUpCounter == 0)
            {
                foreach (var g in ghosts)
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
            Console.WriteLine("Score: {0} Lives: {1}", score, pacman.Lives);
            bool foundGhost;
            char tile = 'e';
            for (int i = 0; i < 31; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    foundGhost = false;
                    if (pacman.Location.Xpos == j && pacman.Location.Ypos == i)
                    {
                        tile = '<';
                        Console.Write("{0} ", tile);
                        continue;
                    }
                    else
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (ghosts[k].Location.Xpos == j && ghosts[k].Location.Ypos == i)
                            {
                                tile = ghosts[k].IsVulnerable ? '~' : '#';
                                foundGhost = true;
                            }
                        }

                        if (!foundGhost)
                        {
                            switch (board.GetTile(j, i))
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
                                    tile = 'F';
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
