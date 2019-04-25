using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AIBracket.Data;
using AIBracket.Web.Auth;
using AIBracket.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AIBracket.Web.Models;
using AIBracket.Web.Managers;

namespace AIBracket.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class BotController : BaseController
    {
        public BotController(AIBracketContext appDbContext, UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions) : base(appDbContext, userManager, jwtFactory, jwtOptions)
        {

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBotsByUser(string Id)
        {
            var data = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(x => x.Type == "id")?.Value);
            if (data == null || data.Id.ToString() != Id)
            {
                var bots1 = _appDbContext.Bots.Where(x => x.IdentityId.ToString() == Id).OrderBy(x => x.Game).Select(x => new { x.Id, x.Name, x.Game, Active = GameCacheManager.IsBotActive(x.Id.ToString()), HighScore = _appDbContext.PacmanGames.Where(y => y.BotId.ToString() == x.Id.ToString()).DefaultIfEmpty().Max(y => y.Score) });
                return Ok(bots1);
            }
            var bots = _appDbContext.Bots.Where(x => x.IdentityId == data.Id).OrderBy(x => x.Game).Select(x => new { x.Id, x.Name, x.PrivateKey, x.Game, Active = GameCacheManager.IsBotActive(x.Id.ToString()), HighScore = _appDbContext.PacmanGames.Where(y => y.BotId.ToString() == x.Id.ToString()).DefaultIfEmpty().Max(y => y.Score) });
            return Ok(bots);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetBotDetails(string Id)
        {
            var bot = _appDbContext.Bots.FirstOrDefault(x => x.Id.ToString() == Id);
            if(bot != null)
            {
                return Ok(new { bot.Id, bot.IdentityId, bot.Game, bot.Name });
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBot(BotCreateViewModel bot)
        {
            var data = await _userManager.FindByIdAsync(User.Claims.First(x => x.Type == "id").Value);
            if (data == null)
            {
                return Unauthorized();
            }
            var guid = Guid.NewGuid().ToByteArray();
            var newBot = new Bot
            {
                IdentityId = data.Id,
                PrivateKey = Convert.ToBase64String(guid).Substring(0, 16),
                Name = bot.Name,
                Game = bot.Game
            };
            var botmodel = _appDbContext.Bots.Add(newBot);
            await _appDbContext.SaveChangesAsync();
            return Ok(newBot.Id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBot([FromBody]string Id)
        {
            var success = Guid.TryParse(Id, out Guid guid);
            if (!success)
            {
                return BadRequest("Invalid Id");
            }
            var data = await _userManager.FindByIdAsync(User.Claims.First(x => x.Type == "id").Value);
            if (data == null)
            {
                return Unauthorized();
            }
            var bot = await _appDbContext.Bots.FindAsync(guid);
            if(bot != null)
            {
                if (bot.IdentityId == data.Id)
                {
                    _appDbContext.Bots.Remove(bot);
                    switch (bot.Game)
                    {
                        case 1:
                            _appDbContext.PacmanGames.RemoveRange(_appDbContext.PacmanGames.Where(x => x.BotId == bot.Id));
                            break;
                        default:
                            break;
                    }
                    await _appDbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return Unauthorized("You cannot delete someone else's bot");
                }
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetBotHistory(string Id)
        {
            var games = _appDbContext.PacmanGames.Where(x => x.BotId.ToString() == Id).ToList();
            return Ok(games);
        }
    }
}