using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.Data.Entities
{
    // Add profile data for application users by adding properties to this class
    public class AppUser : IdentityUser<Guid>
    {

        public AppUser()
        {
            CreatedAt = DateTime.Now;
        }

        // Extended Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
