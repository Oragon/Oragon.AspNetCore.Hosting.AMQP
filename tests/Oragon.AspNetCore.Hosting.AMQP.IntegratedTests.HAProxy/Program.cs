using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Oragon.AspNetCore.Hosting.AMQP.IntegratedTests.HAProxy
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel(configureOptions)
            .UseStartup<Startup>();


        private static void configureOptions(Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions options)
        {
            options.Limits.MaxConcurrentConnections = null;
            options.Limits.MaxConcurrentUpgradedConnections = null;
        }
    }
}
