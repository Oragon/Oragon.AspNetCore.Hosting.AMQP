using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oragon.AspNetCore.Hosting.AMQP.Worker;
using RabbitMQ.Client;

namespace Oragon.AspNetCore.Hosting.AMQP.IntegratedTests.HAWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHostBuilder config = CreateWebHostBuilder(args);
            AMQPWorkerBootstrapper.Run(config, builder =>
                builder
                .WithGroupName("dynamic_api")
                .WithConnectionFactory(new ConnectionFactory()
                {
                    HostName = "192.168.1.88",
                    UserName = "usuario",
                    Password = "senha",
                    VirtualHost = "exemplo_amqp",
                    Port = 5672
                })
                .WithPoolSize(10)
               
            );

            //config.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
