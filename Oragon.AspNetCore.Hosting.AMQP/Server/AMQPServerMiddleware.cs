using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oragon.AspNetCore.Hosting.AMQP.Server
{
    public class AMQPServerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly Configuration configuration;
        private readonly RouteInfo[] routeInfos;

        public AMQPServerMiddleware(RequestDelegate next, Configuration configuration)
        {
            this.next = next ?? throw new ArgumentNullException("next");
            this.configuration = configuration ?? throw new ArgumentNullException("configuration");
            this.routeInfos = this.configuration.Routes.ToArray();
            this.CreatePool();
        }

        private void CreateQueue()
        {
            ConnectionPoolItem connectionPoolItem = null;
            while (this.connectionPool.TryPeek(out connectionPoolItem) == false)
            {
                Thread.Sleep(15);
            }

        }

        private void CreatePool()
        {
            int pid = Process.GetCurrentProcess().Id;
            string machineName = Environment.MachineName;
            bool oneTimeExec = true;
            var rabbitMQConnection = this.configuration.ConnectionFactory.CreateConnection($"{machineName}#{pid}#0|Oragon.AspNetCore.Hosting.AMQP.Server|{this.configuration.GroupName}");
            for (var connectionSeq = 1; connectionSeq <= this.configuration.PoolSize; connectionSeq++)
            {
                var rabbitMQModel = rabbitMQConnection.CreateModel();

                if (oneTimeExec)
                {
                    rabbitMQModel.QueueDeclare(this.configuration.GetQueueName(), true, false, false, null);
                    oneTimeExec = false;
                }

                var rpcClient = new SimpleRpcClient(
                        model: rabbitMQModel,
                        queueName: this.configuration.GetQueueName()
                    );

                connectionPool.Enqueue(new ConnectionPoolItem()
                {
                    connection = rabbitMQConnection,
                    model = rabbitMQModel,
                    rpc = rpcClient
                });
            }
        }

        private ConcurrentQueue<ConnectionPoolItem> connectionPool = new ConcurrentQueue<ConnectionPoolItem>();

        private void UsePool(Action<ConnectionPoolItem> action)
        {
            ConnectionPoolItem poolItem = null;

            while (this.connectionPool.TryDequeue(out poolItem) == false) Thread.Sleep(3);

            action(poolItem);

            this.connectionPool.Enqueue(poolItem);
        }

        public async Task Invoke(HttpContext context)
        {
            RouteInfo route = this.routeInfos.FirstOrDefault(it => it.Match(context));
            if (route != null)
            {

                if (route.Pattern == Pattern.Rpc)
                {

                    InvokeWithRpcChoreography(context);

                }
                else if (route.Pattern == Pattern.FireAndForget)
                {

                    InvokeWithFireAndForgetChoreography(context);

                }

            }
            else
            {

                await this.next(context);

            }
        }

        private void InvokeWithFireAndForgetChoreography(HttpContext context)
        {
            this.UsePool(poolItem =>
            {

                byte[] payload = context.Request.Body.ReadToEnd();

                context.Response.StatusCode = 200;

                IBasicProperties propsIn = poolItem.model.CreateBasicProperties();

                propsIn.DeliveryMode = 2;

                ContextAdapter.FillPropertiesFromRequest(propsIn, context);

                poolItem.model.BasicPublish(string.Empty, this.configuration.GetQueueName(), propsIn, payload);

            });

            //context.Response.StatusCode = 204;


        }

        private void InvokeWithRpcChoreography(HttpContext context)
        {
            this.UsePool(poolItem =>
            {

                byte[] payload = context.Request.Body.ReadToEnd();

                IBasicProperties propsIn = poolItem.model.CreateBasicProperties();
                propsIn.DeliveryMode = 2;

                ContextAdapter.FillPropertiesFromRequest(propsIn, context);

                payload = poolItem.rpc.Call(propsIn, payload, out IBasicProperties propsOut);

                ContextAdapter.FillResponseFromProperties(context, propsOut);

                context.Response.Body.Write(payload, 0, payload.Length);

                poolItem.rpc.Subscription.Ack();

            });
        }
    }
}