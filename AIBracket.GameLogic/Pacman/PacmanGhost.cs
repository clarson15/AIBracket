using AIBracket.GameLogic.Pacman.Coordinate;
using AIBracket.GameLogic.Pacman.Pacman;
using System;
using System.Collections.Generic;

namespace AIBracket.GameLogic.Pacman.Ghost
{
    public class PacmanGhost
    {
        const int DividendForChanceOfHinderingMove = -250;
        const int StartingPercentage = 100;
        public enum Ghost { Blue, Pink, Red, Orange };
        public bool IsDead, IsVulnerable;
        private int DeadCounter;
        public PacmanCoordinate Location;
        public PacmanPacman.Direction Facing { get; set; }

        public PacmanGhost(int number)
        {
            Facing = PacmanPacman.Direction.start;
            IsDead = true;
            IsVulnerable = false;
            DeadCounter = 10 * number;
            Location = new PacmanCoordinate(13, 12);
        }

        public PacmanCoordinate GetPosition()
        {
            return Location;
        }

        public void Move()
        {
            switch (Facing)
            {
                case PacmanPacman.Direction.start:
                    break;
                case PacmanPacman.Direction.up:
                    Location.Ypos -= 0.2m;
                    break;
                case PacmanPacman.Direction.down:
                    Location.Ypos += 0.2m;
                    break;
                case PacmanPacman.Direction.left:
                    Location.Xpos -= 0.2m;
                    break;
                case PacmanPacman.Direction.right:
                    Location.Xpos += 0.2m;
                    break;
                default:
                    Console.Error.WriteLine("Error: entered PacmanEntity.move switch default");
                    break;
            }
        }

        /// <summary>
        /// Starts the timer until respawn after a ghost dies
        /// </summary>
        public void StartDeathCounter()
        {
            DeadCounter = 10;
        }

        /// <summary>
        /// Decrements DeadCounter
        /// </summary>
        /// <returns>If the ghost should be respawned</returns>
        public bool DecrementDeathTimerCheckRespawn()
        {
            return (--DeadCounter) <= 0;
        }

        /// <summary>
        /// Determines the Ghosts moves based on a target position prioritizing minimizing distance
        /// Has a chance to choose a random valid move
        /// </summary>
        /// <param name="possible">List of valid directions</param>
        /// <param name="pos">Target Coordinate</param>
        /// <param name="chance">Percent Chance the move will be random</param>
        /// <returns></returns>
        public PacmanPacman.Direction DetermineGhostMove(List<PacmanPacman.Direction> possible, PacmanCoordinate pos, int score)
        {
            // f(x) = x / -250 + 100 
            // Dividend should change after testing
            var chance = (score / DividendForChanceOfHinderingMove) + StartingPercentage;
            var random = new Random();
            var chanceForRandomMove = random.Next(101);
            var difference = Location - pos;
            if (possible.Count > 1 && possible.Contains(PacmanPacman.InverseDirection(Facing)))
            {
                possible.Remove(PacmanPacman.InverseDirection(Facing));
            }
            if (chanceForRandomMove >= chance)
            {
                return possible[random.Next(0, possible.Count)];
            }
            if (possible.Count > 0)
            {
                if (Math.Abs(difference.Xpos) > Math.Abs(difference.Ypos))
                {
                    if (difference.Xpos > 0)
                    {
                        if (possible.Contains(PacmanPacman.Direction.left))
                        {
                            return PacmanPacman.Direction.left;
                        }
                    }
                    else
                    {
                        if (possible.Contains(PacmanPacman.Direction.right))
                        {
                            return PacmanPacman.Direction.right;
                        }
                    }
                }
                else
                {
                    if (difference.Ypos > 0)
                    {
                        if (possible.Contains(PacmanPacman.Direction.up))
                        {
                            return PacmanPacman.Direction.up;
                        }
                    }
                    else
                    {
                        if (possible.Contains(PacmanPacman.Direction.down))
                        {
                            return PacmanPacman.Direction.down;
                        }
                    }
                }
                return possible[random.Next(0, possible.Count)];
            }
            return PacmanPacman.Direction.start;
        }
    }
}
