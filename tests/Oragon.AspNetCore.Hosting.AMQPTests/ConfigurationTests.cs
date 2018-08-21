using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Oragon.AspNetCore.Hosting.AMQP;
using RabbitMQ.Client;
using System.Collections.Generic;
using Xunit;

namespace Oragon.AspNetCore.Hosting.AMQPTests
{
    public class ConfigurationTests
    {
        [Fact]
        public void ConfigurationTests_Case1()
        {
            var IConnectionFactoryMock = new Mock<IConnectionFactory>();

            Configuration configurationToTest = new Configuration();
            configurationToTest
                .WithConnectionFactory(IConnectionFactoryMock.Object)
                .WithGroupName("group-name")
                .WithPoolSize(5)
                .WithMaxConnectionAge(30000)
                .WithRoute("GET", "AA", Pattern.FireAndForget);


            Assert.Equal("group-name", configurationToTest.GroupName);
            Assert.Equal("group-name_queue", configurationToTest.GetQueueName());
            Assert.Equal(5, configurationToTest.PoolSize);
            Assert.NotNull(configurationToTest.ConnectionFactory);
            Assert.Equal(30000, configurationToTest.MaxConnectionAge);
            Assert.Single(configurationToTest.Routes);
            Assert.Equal("get", configurationToTest.Routes[0].Method);
            Assert.Equal(Pattern.FireAndForget, configurationToTest.Routes[0].Pattern);
            Assert.Equal("aa", configurationToTest.Routes[0].Route);
        }
    }
}