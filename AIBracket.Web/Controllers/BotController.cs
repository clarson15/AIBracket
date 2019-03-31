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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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

        [HttpGet]
        public async Task<IActionResult> GetBotsByUser()
        {
            var data = await _userManager.FindByIdAsync(User.Claims.First(x => x.Type == "id").Value);
            if (data == null)
            {
                return Unauthorized();
            }
            var bots = _appDbContext.Bots.Where(x => x.IdentityId == data.Id).OrderBy(x => x.Game);
            return Ok(bots);
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
            var success = Guid.TryParse(Id, out Guid guid);
            if (!success)
            {
                return BadRequest("Invalid Bot Id");
            }
            var games = _appDbContext.PacmanGames.Where(x => x.BotId == guid).ToList();
            return Ok(games);
        }
    }
}