using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Oragon.AspNetCore.Hosting.AMQP;
using RabbitMQ.Client;
using System.Collections.Generic;
using Xunit;

namespace Oragon.AspNetCore.Hosting.AMQPTests
{
    public class ContextAdapterTests
    {
        [Fact]
        public void FillPropertiesFromRequest_Case1()
        {
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(it => it.Request.Method).Returns("GET");
            httpContextMock.SetupGet(it => it.Request.Path).Returns(new PathString("/a"));
            httpContextMock.SetupGet(it => it.Request.ContentType).Returns("application/json");
            httpContextMock.SetupGet(it => it.Request.ContentLength).Returns(9);
            httpContextMock.SetupGet(it => it.Request.Headers).Returns(new HeaderDictionary(new Dictionary<string, StringValues>{
                { "HTTP_KEY1", new StringValues("HTTP_VALUE1") },
                { "HTTP_KEY2", new StringValues("HTTP_VALUE2") }
            }));

            var basicPropertiesMock = new Mock<IBasicProperties>();
            basicPropertiesMock.SetupProperty(it => it.Headers);

            var basicProperties = basicPropertiesMock.Object;
            var httpContext = httpContextMock.Object;
            ContextAdapter.FillPropertiesFromRequest(basicProperties, httpContext);

            Assert.NotNull(basicProperties.Headers);
            Assert.NotEmpty(basicProperties.Headers);

            Assert.True(basicProperties.Headers.ContainsKey("AMQP_METHOD"));
            Assert.Equal("GET", basicProperties.Headers["AMQP_METHOD"]);

            Assert.True(basicProperties.Headers.ContainsKey("AMQP_PATH"));
            Assert.Equal("/a", basicProperties.Headers["AMQP_PATH"]);

            Assert.True(basicProperties.Headers.ContainsKey("AMQP_CONTENTTYPE"));
            Assert.Equal("application/json", basicProperties.Headers["AMQP_CONTENTTYPE"]);

            Assert.True(basicProperties.Headers.ContainsKey("AMQP_CONTENTLENGTH"));
            Assert.Equal(9L, basicProperties.Headers["AMQP_CONTENTLENGTH"]);

            Assert.True(basicProperties.Headers.ContainsKey("HTTP_KEY1"));
            Assert.Equal("HTTP_VALUE1", basicProperties.Headers["HTTP_KEY1"]);

            Assert.True(basicProperties.Headers.ContainsKey("HTTP_KEY2"));
            Assert.Equal("HTTP_VALUE2", basicProperties.Headers["HTTP_KEY2"]);
        }
    }
}