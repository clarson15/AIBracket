using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AIBracket.Data;
using AIBracket.Web.Auth;
using AIBracket.Data.Entities;
using AutoMapper;
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
    public class BotController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AIBracketContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly JwtIssuerOptions _jwtOptions;

        public BotController(IMapper mapper, AIBracketContext appDbContext, UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _mapper = mapper;
            _appDbContext = appDbContext;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
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
            var botmodel = _appDbContext.Bots.Add(new Bot
            {
                IdentityId = data.Id,
                PrivateKey = Convert.ToBase64String(guid).Substring(0, 16),
                Name = bot.Name,
                Game = bot.Game
            });
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBot([FromBody] int Id)
        {
            var data = await _userManager.FindByIdAsync(User.Claims.First(x => x.Type == "id").Value);
            if (data == null)
            {
                return Unauthorized();
            }
            var bot = await _appDbContext.Bots.FindAsync(Id);
            if(bot != null)
            {
                _appDbContext.Bots.Remove(bot);
            }
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}