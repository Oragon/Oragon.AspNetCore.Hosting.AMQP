using RabbitMQ.Client;
using System.Collections.Generic;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    public class Configuration
    {
        #region properties

        public string GroupName { get; internal set; }

        public string GetQueueName() => $"{this.GroupName}_queue";

        public int PoolSize { get; internal set; } = 5;

        public long MaxConnectionAge { get; internal set; } = 20000;

        public IConnectionFactory ConnectionFactory { get; internal set; }

        public List<RouteInfo> Routes { get; internal set; } = new List<RouteInfo>();

        #endregion properties

        #region Fluent

        public Configuration WithGroupName(string groupName)
        {
            this.GroupName = groupName;
            return this;
        }

        public Configuration WithPoolSize(int poolSize)
        {
            this.PoolSize = poolSize;
            return this;
        }

        public Configuration WithConnectionFactory(IConnectionFactory connectionFactory)
        {
            this.ConnectionFactory = connectionFactory;
            return this;
        }

        public Configuration WithRoute(string method, string route, Pattern pattern)
        {
            this.Routes.Add(new RouteInfo() { Method = method, Pattern = pattern, Route = route });
            return this;
        }

        #endregion Fluent
    }
}