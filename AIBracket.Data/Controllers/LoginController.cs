using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AIBracket.Data.DTO;
using AIBracket.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AIBracket.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private IConfiguration _config;
        private static SQLiteConnection _conn;

        public LoginController(IConfiguration config)
        {
            _config = config;
            if (_conn == null || (_conn.State & System.Data.ConnectionState.Open) == 0) {
                var connString = "Data Source = database.sqlite";
                _conn = new SQLiteConnection(connString);
                _conn.Open();
            }
        }

        [HttpPost("[action]")]
        public IActionResult Authenticate([FromBody] LoginObject data)
        {
            var sql = "SELECT password FROM Users WHERE username = '" + data.username + "'";
            var cmd = new SQLiteCommand(sql, _conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            var password = (string)reader.GetValue(0);
            var valid = BCrypt.Net.BCrypt.Verify(data.password, password);
            if (valid)
            {
                var usersql = "SELECT uid, create_date FROM Users WHERE username = '" + data.username + "'";
                var usercmd = new SQLiteCommand(usersql, _conn);
                var userreader = usercmd.ExecuteReader();
                userreader.Read();
                var user = new User();
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("Private Key"));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.uid.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                user.username = data.username;
                user.uid = userreader.GetInt32(0);
                user.create_date = userreader.GetString(1);
                user.token = tokenString;
                return Ok(user);
            }
            return Unauthorized("Incorrect username and/or password.");
        }

        [HttpPost("[action]")]
        public IActionResult CreateAccount([FromBody] LoginObject data)
        {
            var sql = "SELECT username FROM Users WHERE username = '" + data.username + "'";
            var cmd = new SQLiteCommand(sql, _conn);
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                return StatusCode(401, "Username is already taken.");
            }
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var password = BCrypt.Net.BCrypt.HashPassword(data.password, salt);
            string created_date = DateTime.Now.ToString();
            sql = "INSERT INTO Users (username, password, salt, create_date) Values ('" + data.username + "', '" + password + "', '" + salt + "', '" + created_date + "')";
            cmd = new SQLiteCommand(sql, _conn);
            cmd.ExecuteNonQuery();
            return StatusCode(200);
        }
    }
}