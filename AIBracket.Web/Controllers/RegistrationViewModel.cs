using AIBracket.Data.Entities;

namespace AIBracket.Web.Controllers
{
    public class RegistrationViewModel : AppUser
    {
        public string Password { get; set; }
        public string Location { get; set; }
    }
}