using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    internal class ConnectionPoolItem
    {
        internal IConnection connection;

        internal IModel model;
        internal SimpleRpcClient rpc;
    }
}
