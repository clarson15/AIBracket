using System;
using System.ComponentModel.DataAnnotations;

namespace AIBracket.Data.Entities
{
    public class Bot
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdentityId { get; set; }
        public string PrivateKey { get; set; }
        public string Name { get; set; }
        public int Game { get; set; }
    }
}
