using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oragon.AspNetCore.Hosting.AMQP.Worker
{
    public class AMQPWorkerBootstrapper
    {

        public static void Run(IWebHostBuilder builder, Action<Configuration> builderConfigurer)
        {
            string fingerPrint = $"{Environment.MachineName}#{Process.GetCurrentProcess().Id}";

            var configuration = new Configuration();
            builderConfigurer(configuration);

            using (var rabbitMQConnection = configuration.ConnectionFactory.CreateConnection($"{fingerPrint}|Oragon.AspNetCore.Hosting.AMQP.Worker|{configuration.GroupName}"))
            {
                using (var model = rabbitMQConnection.CreateModel())
                {
                    model.QueueDeclare(configuration.GetQueueName(), true, false, false, null);
                }
            }

            using (TestServer testServer = new TestServer(builder))
            {
                for (int connectionSeq = 0; connectionSeq <= configuration.PoolSize; connectionSeq++)
                {
                    Thread newThread = new Thread(() => {
                        using (var rabbitMQConnection = configuration.ConnectionFactory.CreateConnection($"{fingerPrint}#{connectionSeq}|Oragon.AspNetCore.Hosting.AMQP.Worker|{configuration.GroupName}"))
                        {
                            using (var model = rabbitMQConnection.CreateModel())
                            {
                                var rpcServer = new RpcServer(
                                    new Subscription(
                                        model: model,
                                        queueName: configuration.GetQueueName(),
                                        autoAck: false
                                    ),
                                    testServer
                                );
                                rpcServer.MainLoop();
                            }
                        }
                    });
                    newThread.Start();
                }
                while (true)
                {
                    Thread.Sleep(500);
                }
            }
        }
    }


}
