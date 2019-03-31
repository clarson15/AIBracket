using AIBracket.Data.Entities;

namespace AIBracket.Web.Models
{
    public class RegistrationViewModel : AppUser
    {
        public string Password { get; set; }
        public string Location { get; set; }
    }
}