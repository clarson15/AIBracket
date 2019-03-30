using System;
using System.ComponentModel.DataAnnotations;

namespace AIBracket.Data.Entities
{
    public class PacmanGames
    {
        [Key]
        public Guid Id;
        public Guid BotId;
        public DateTime StartDate;
        public DateTime EndDate;
        public int Score;
        public int Difficulty;
    }
}
