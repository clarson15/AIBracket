using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AIBracket.Data;
using AIBracket.Data.Entities;
using AIBracket.Web.Auth;
using AIBracket.Web.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AIBracket.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class GameController : BaseController
    {

        public GameController(AIBracketContext appDbContext, UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions) : base(appDbContext, userManager, jwtFactory, jwtOptions)
        {
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetFeaturedGame()
        {
            var game = GameCacheManager.GetFeaturedGameId();
            if(game == null)
            {
                return Ok(0);
            }
            return Ok(game);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetLeaderboard()
        {
            var leaderboard = _appDbContext.PacmanGames.OrderBy(x => x.Score).Take(20);
            return Ok(leaderboard.Select(x => new
            {
                x.Score,
                x.Id,
                x.StartDate,
                _appDbContext.Bots.First(y => y.Id == x.BotId).Name
            }));
        }
    }
}