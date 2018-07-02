using RabbitMQ.Client;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    internal class ConnectionPoolItem
    {
        internal IConnection connection;

        internal IModel model;

        internal string queueName;

        internal long usage;
    }
}