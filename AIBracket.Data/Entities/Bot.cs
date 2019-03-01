using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.Data.Entities
{
    public class Bot
    {
        public Guid Id { get; set; }
        public Guid IdentityId { get; set; }
        public string PrivateKey { get; set; }
        public string Name { get; set; }
        public int Game { get; set; }

    }
}
