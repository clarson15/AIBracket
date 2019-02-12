using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AIBracket.Web.Controllers
{

    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string BaseUrl = "https://localhost:44332/api/";

        [HttpPost("[action]")]
        public async Task<string> Register([FromBody] object data)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(BaseUrl + "Login/CreateAccount", data);
            var responsejr = await response.Content.ReadAsStringAsync();
            return responsejr;  
        }

        [HttpPost("[action]")]
        public async Task<string> Login([FromBody] object data)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(BaseUrl + "Login/Login", data);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
