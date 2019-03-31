using System;
using System.ComponentModel.DataAnnotations;

namespace AIBracket.Data.Entities
{
    public class PacmanGames
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BotId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Score { get; set; }
        public int Difficulty { get; set; }
    }
}
