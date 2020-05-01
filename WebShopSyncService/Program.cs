using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebShopSyncService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var config = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", optional: true)
                    .AddCommandLine(args)
                    .Build();
                    var httpPort = config["HttpPort"];
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:" + httpPort);
                });
    }
}
