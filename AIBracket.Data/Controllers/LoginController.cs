using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using AIBracket.Data.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIBracket.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private static SQLiteConnection _conn;

        public LoginController()
        {
            if ((_conn.State & System.Data.ConnectionState.Open) == 0) {
                var connString = "Data Source = database.sqlite";
                _conn = new SQLiteConnection(connString);
                _conn.Open();
            }
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginObject data)
        {
            var sql = "SELECT password FROM Users WHERE username = '" + data.username + "'";
            var cmd = new SQLiteCommand(sql, _conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            var password = (string)reader.GetValue(0);
            var valid = BCrypt.Net.BCrypt.Verify(data.password, password);
            if (valid)
            {
               return StatusCode(200);
            }
            return StatusCode(401, "Incorrect username and/or password.");
        }

        [HttpPost]
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
            sql = "INSERT INTO Users (username, password, salt, created_date) Values ('" + data.username + "', '" + password + "', '" + salt + "', '" + created_date + "')";
            cmd = new SQLiteCommand(sql, _conn);
            cmd.ExecuteNonQuery();
            return StatusCode(200);
        }
    }
}