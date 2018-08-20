using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Oragon.AspNetCore.Hosting.AMQP.Server
{
    public class AmqpServerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly Configuration configuration;
        private readonly List<RouteInfo> routeInfos;
        private readonly string connectionName;

        public AmqpServerMiddleware(RequestDelegate next, Configuration configuration)
        {
            this.next = next ?? throw new ArgumentNullException("next");
            this.configuration = configuration ?? throw new ArgumentNullException("configuration");
            this.routeInfos = this.configuration.Routes;
            this.connectionName = $"{Environment.MachineName}#{Process.GetCurrentProcess().Id}#0|Oragon.AspNetCore.Hosting.AMQP.Server|{this.configuration.GroupName}";
            this.CreatePool();
        }

        private ConnectionPoolItem BuildConnectionPoolItem()
        {
            var rabbitMQConnection = this.configuration.ConnectionFactory.CreateConnection(this.connectionName);
            var rabbitMQModel = rabbitMQConnection.CreateModel();
            return new ConnectionPoolItem()
            {
                connection = rabbitMQConnection,
                model = rabbitMQModel,
                queueName = this.configuration.GetQueueName()
            };
        }

        private ConnectionPoolItem RecycleConnectionPoolItem(ref ConnectionPoolItem connectionPoolItem)
        {
            connectionPoolItem.usage++;
            if (connectionPoolItem.usage == this.configuration.MaxConnectionAge)
            {
                connectionPoolItem.model.Close();
                connectionPoolItem.model.Dispose();
                connectionPoolItem.connection.Close();
                connectionPoolItem.connection.Dispose();
                return BuildConnectionPoolItem();
            }
            return connectionPoolItem;
        }

        private void CreatePool()
        {
            bool oneTimeExec = true;

            for (var connectionSeq = 1; connectionSeq <= this.configuration.PoolSize; connectionSeq++)
            {
                ConnectionPoolItem connectionPoolItem = this.BuildConnectionPoolItem();

                if (oneTimeExec)
                {
                    connectionPoolItem.model.QueueDeclare(this.configuration.GetQueueName(), true, false, false, null);
                    oneTimeExec = false;
                }

                connectionPool.Enqueue(connectionPoolItem);
            }
        }

        private readonly ConcurrentQueue<ConnectionPoolItem> connectionPool = new ConcurrentQueue<ConnectionPoolItem>();

        public async Task Invoke(HttpContext context)
        {
            RouteInfo route = this.routeInfos.FirstOrDefault(it => it.Match(context));
            if (route != null && route.Pattern != Pattern.Ignore)
            {
                ConnectionPoolItem poolItem;

                while (!this.connectionPool.TryDequeue(out poolItem)) Console.WriteLine("wait...");

                if (route.Pattern == Pattern.Rpc)
                {
                    InvokeWithRpcChoreography(ref poolItem, ref context);
                }
                else if (route.Pattern == Pattern.FireAndForget)
                {
                    InvokeWithFireAndForgetChoreography(ref poolItem, ref context);
                }

                this.connectionPool.Enqueue(this.RecycleConnectionPoolItem(ref poolItem));
            }
            else
            {
                await this.next(context);
            }
        }

        private void InvokeWithFireAndForgetChoreography(ref ConnectionPoolItem poolItem, ref HttpContext context)
        {
            byte[] payload = context.Request.Body.ReadToEnd();

            context.Response.StatusCode = 200;

            IBasicProperties propsIn = poolItem.model.CreateBasicProperties();

            ContextAdapter.FillPropertiesFromRequest(propsIn, context);

            poolItem.model.BasicPublish(string.Empty, poolItem.queueName, propsIn, payload);
        }

        private void InvokeWithRpcChoreography(ref ConnectionPoolItem poolItem, ref HttpContext context)
        {
            byte[] payload = context.Request.Body.ReadToEnd();

            IBasicProperties propsIn = poolItem.model.CreateBasicProperties();

            ContextAdapter.FillPropertiesFromRequest(propsIn, context);

            using (var rpcClient = new SimpleRpcClient(
                    model: poolItem.model,
                    queueName: poolItem.queueName
                ))
            {
                payload = rpcClient.Call(propsIn, payload, out IBasicProperties propsOut);

                ContextAdapter.FillResponseFromProperties(context, propsOut);

                context.Response.Body.Write(payload, 0, payload.Length);

                rpcClient.Subscription.Ack();
            }
        }
    }
}