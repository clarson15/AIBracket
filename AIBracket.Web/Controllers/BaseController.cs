using AIBracket.Data;
using AIBracket.Data.Entities;
using AIBracket.Web.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;

namespace AIBracket.Web.Controllers
{

    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly AIBracketContext _appDbContext;
        protected readonly UserManager<AppUser> _userManager;
        protected readonly IJwtFactory _jwtFactory;
        protected readonly JsonSerializerSettings _serializerSettings;
        protected readonly JwtIssuerOptions _jwtOptions;

        protected string UserId => User.Claims.First(x => x.Type == "id").Value;

        public BaseController(AIBracketContext appDbContext, UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
    }
}