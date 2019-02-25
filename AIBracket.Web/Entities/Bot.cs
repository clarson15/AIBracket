using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.Web.Entities
{
    public class Bot
    {
        public int Id { get; set; }
        public string IdentityId { get; set; }
        public AppUser Identity { get; set; }
        public string PrivateKey { get; set; }
        public string Name { get; set; }
        public int Game { get; set; }

    }
}
