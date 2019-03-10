using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIBracket.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseUrls(urls: "http://*;https://*").UseKestrel(x =>
            {
                x.Listen(IPAddress.Loopback, 443, listenOptions =>
                {
                    listenOptions.UseHttps("Cert.pfx", "password");
                });
            }).UseStartup<Startup>();
        }
    }
}
