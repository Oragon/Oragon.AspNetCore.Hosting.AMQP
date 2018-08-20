using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Diagnostics;
using System.Threading;

namespace Oragon.AspNetCore.Hosting.AMQP.Worker
{
    public static class AmqpWorkerBootstrapper
    {
        public static void Run(IWebHostBuilder builder, Action<Configuration> builderConfigurer)
        {
            string fingerPrint = $"{Environment.MachineName}#{Process.GetCurrentProcess().Id}";

            var configuration = new Configuration();
            builderConfigurer(configuration);

            using (TestServer testServer = new TestServer(builder))
            {
                Func<IConnectionFactory> getConnectionFactory = () =>
                {
                    var connectionFactory = configuration.ConnectionFactory ?? (IConnectionFactory)testServer.Host.Services.GetService(typeof(IConnectionFactory)) ?? throw new InvalidOperationException("IConnectionFactory not found");

                    return connectionFactory;
                };

                using (var rabbitMQConnection = getConnectionFactory().CreateConnection($"{fingerPrint}|Oragon.AspNetCore.Hosting.AMQP.Worker|{configuration.GroupName}"))
                {
                    using (var model = rabbitMQConnection.CreateModel())
                    {
                        model.QueueDeclare(configuration.GetQueueName(), true, false, false, null);
                    }
                }

                for (int connectionSeq = 0; connectionSeq <= configuration.PoolSize; connectionSeq++)
                {
                    Thread newThread = new Thread(() =>
                    {
                        using (var rabbitMQConnection = getConnectionFactory().CreateConnection($"{fingerPrint}#{connectionSeq}|Oragon.AspNetCore.Hosting.AMQP.Worker|{configuration.GroupName}"))
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