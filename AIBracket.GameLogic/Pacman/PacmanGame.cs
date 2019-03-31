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
        public enum EventType { BoardReset, PacmanUpdate, GhostUpdate, Dot, Fruit, PowerUp, FruitSpawn, PacmanLives, GhostDie, Score, DifficultyIncrease };
        public enum PacmanDifficulty { Easy, Medium, Hard };
        public List<KeyValuePair<EventType, string>> CurrentGameEvent;
        public PacmanBoard Board { get; private set; }
        public PacmanPacman Pacman { get; private set; }
        public PacmanGhost[] Ghosts { get; private set; }
        public  DateTime TimeStarted { get; private set; }
        public DateTime TimeEnded { get; private set; }
        public PacmanDifficulty Difficulty { get; private set; }
        public int Score { get; private set; }
        private int GhostScoreMultiplier { get; set; }
        private int SpawnGhostCounter { get; set; }
        private int PoweredUpCounter { get; set; }
        private int FruitSpawnCounter { get; set; }
        private bool SlowGhosts { get; set; }
        public bool GameRunning { get; private set; }

        public PacmanGame()
        {
            CurrentGameEvent = new List<KeyValuePair<EventType, string>>();
            Board = new PacmanBoard();
            CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.BoardReset, GetBoardString()));
            Pacman = new PacmanPacman();
            CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.PacmanLives, $"{Pacman.Lives}"));
            Ghosts = new PacmanGhost[4]
            {
                new PacmanGhost(),
                new PacmanGhost(),
                new PacmanGhost(),
                new PacmanGhost()
            };
            Random random = new Random();
            for (var i = 0; i < Ghosts.Length; i++)
            {
                Ghosts[i].Facing = (PacmanPacman.Direction)random.Next(1, 5);
                CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.GhostUpdate, $"{i} {Ghosts[i].GetPosition().ToString()} {Ghosts[i].IsDead} {Ghosts[i].IsVulnerable}"));
            }
            Difficulty = PacmanDifficulty.Easy;
            Score = 0;
            CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.Score, $"{Score}"));
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
        private void UpdateScore(PacmanCoordinate pos)
        {
            var t = Board.GetTile(pos);
            if(t == PacmanBoard.Tile.dot)
            {
                Score += 10;
                CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.Dot, pos.ToString() + " 10"));
            }
            else if(t == PacmanBoard.Tile.fruit)
            {
                Score += 100;
                CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.Fruit, pos.ToString() + " 100"));
            }
            else if(t == PacmanBoard.Tile.powerUp)
            {
                Score += 50;
                CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.PowerUp, pos.ToString() + " 50"));
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
            CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.BoardReset, GetBoardString()));
        }

        public void UpdateDifficulty(PacmanDifficulty d)
        {
            Difficulty = d;
            CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.DifficultyIncrease, $"{(int)d}"));
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
                    UpdateScore(pos);
                    Board.UpdateTile(pos);
                    if (Board.DotCount <= 0)
                        ResetBoard();
                    GhostScoreMultiplier = 1;
                    break;
                case PacmanBoard.Tile.dot:
                case PacmanBoard.Tile.fruit:
                    UpdateScore(pos);
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
                for (int i = 0; i < Ghosts.Length; i++)
                {
                    if(Ghosts[i].IsDead == true)
                    {
                        Ghosts[i].IsDead = false;
                        CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.GhostDie, i + " 0"));
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
        private bool PacmanGhostCollide()
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
                        CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.GhostDie, i + $" 1 {200 * GhostScoreMultiplier}"));

                    }
                    else if (!Ghosts[i].IsDead)
                    {
                        Pacman.Location.Xpos = 13;
                        Pacman.Location.Ypos = 17;
                        Pacman.Facing = PacmanPacman.Direction.right;
                        Pacman.Lives--;
                        CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.PacmanLives,  $"{Pacman.Lives}"));
                        Ghosts = new PacmanGhost[4]
                        {
                            new PacmanGhost(),
                            new PacmanGhost(),
                            new PacmanGhost(),
                            new PacmanGhost()
                        };
                        if (Pacman.Lives <= 0)
                        {
                            Pacman.Lives = 0;
                            GameRunning = false;
                            TimeEnded = DateTime.Now;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        

        /// <summary>Processes every tick of the game base on directions of each entity passed in the array
        /// p should be passed 5 directions
        /// p[0] represents Pacman
        /// p[1] through p[4] represent ghosts in order passed </summary>

        public void UpdateGame(PacmanPacman.Direction p)
        {
            SpawnGhost();
            if (FruitSpawnCounter == 60)
            {
                CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.FruitSpawn, Board.SpawnFruit().ToString()));
                FruitSpawnCounter = -1;
            }
            FruitSpawnCounter++;


            // Move ghosts
            for (int i = 0; i < Ghosts.Length; i++)
            {
                if (Ghosts[i].IsDead)
                {
                    continue;
                }
                if (Ghosts[i].IsVulnerable)
                {
                    if (!SlowGhosts)
                    {
                        PortalGhost(Ghosts[i].Location, i);
                        Ghosts[i].Facing = Ghosts[i].DetermineGhostMoveMedium(Board.PotentialDirections(Ghosts[i].Location), Pacman.Location);
                        Ghosts[i].Move();
                    }
                }
                else
                {
                    PortalGhost(Ghosts[i].Location, i);
                    Ghosts[i].Facing = Ghosts[i].DetermineGhostMoveMedium(Board.PotentialDirections(Ghosts[i].Location), Pacman.Location);
                    Ghosts[i].Move();
                }
            }
            SlowGhosts = !SlowGhosts;
            PacmanGhostCollide();

            // Move Pacman
            if (Board.ValidMove(p, Pacman.GetPosition()))
            {
                Pacman.Facing = p;
            }
            if (Board.ValidMove(Pacman.Facing, Pacman.GetPosition()))
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

            for (var i = 0; i < Ghosts.Length; i++)
            {
                CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.GhostUpdate, $"{i} {Ghosts[i].GetPosition().ToString()} {Ghosts[i].IsDead} {Ghosts[i].IsVulnerable}"));
            }
            CurrentGameEvent.Add(new KeyValuePair<EventType, string>(EventType.PacmanUpdate, $"{Pacman.GetPosition().ToString()}"));
        }

        public string GetBoardString()
        {
            var ret = $"{Board.Width} {Board.Height} ";
            for (int i = 0; i < Board.Height; i++)
            {
                for (int j = 0; j < Board.Width; j++)
                {
                    ret += (int)Board.GetTile(j, i);
                    if (j != Board.Width - 1)
                    {
                        ret += " ";
                    }
                }
                if (i != Board.Height - 1)
                {
                    ret += " ";
                }
            }
            return ret;
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
