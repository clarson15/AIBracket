using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.Data.Entity
{
    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public int uid { get; set; }
        public string create_date { get; set; }
        public string token { get; set; }
    }
}
