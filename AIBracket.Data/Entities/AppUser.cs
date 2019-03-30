using Microsoft.AspNetCore.Identity;
using System;

namespace AIBracket.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {

        public AppUser()
        {
            CreatedAt = DateTime.Now;
            SpectatorId = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 16);
        }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SpectatorId { get; set; }
    }
}
