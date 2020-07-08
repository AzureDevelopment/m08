using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace C_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    var clientId = settings.GetValue<string>("AZURE_CLIENT_ID");
                    Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", clientId);
                    var tenantId = settings.GetValue<string>("AZURE_TENANT_ID");
                    Environment.SetEnvironmentVariable("AZURE_TENANT_ID", tenantId);
                    var secret = settings.GetValue<string>("AZURE_CLIENT_SECRET");
                    Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", secret);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
