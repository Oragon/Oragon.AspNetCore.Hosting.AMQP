using Microsoft.AspNetCore.TestHost;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Oragon.AspNetCore.Hosting.AMQP.Worker
{
    public class RpcServer : SimpleRpcServer
    {
        private readonly TestServer testServer;

        public RpcServer(Subscription subscription, TestServer testServer) : base(subscription)
        {
            this.testServer = testServer;
        }

        public override byte[] HandleCall(bool isRedelivered, IBasicProperties requestProperties, byte[] body, out IBasicProperties replyProperties)
        {
            byte[] returnValue = null;
            replyProperties = null;

            if (requestProperties.Headers != null)
            {
                var path = requestProperties.Headers["AMQP_PATH"].UTF8GetString();
                var method = requestProperties.Headers["AMQP_METHOD"].UTF8GetString();

                var request = testServer.CreateRequest(path);

                if (body.Length > 0)
                {
                    request.And(it =>
                    {

                        it.Content = new StreamContent(new MemoryStream(body));

                    });
                }

                ContextAdapter.FillRequestBuilderFromProperties(requestProperties, request);

                replyProperties = this.m_subscription.Model.CreateBasicProperties();
                replyProperties.DeliveryMode = 2;
                replyProperties.Headers = new Dictionary<string, object>();
                var response = request.SendAsync(method).GetAwaiter().GetResult();

                ContextAdapter.FillPropertiesFromResponse(replyProperties, response);

                returnValue = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                this.m_subscription.Ack();
            }

            return returnValue;
        }
    }
}
