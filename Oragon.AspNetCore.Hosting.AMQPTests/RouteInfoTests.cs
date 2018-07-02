using Microsoft.AspNetCore.Http;
using Moq;
using Oragon.AspNetCore.Hosting.AMQP;
using Xunit;

namespace Oragon.AspNetCore.Hosting.AMQPTests
{
    public class RouteInfoTests
    {
        [Fact]
        public void RouteEmpty()
        {
            var request = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();

            request.SetupGet(it => it.Method).Returns("GET");
            request.SetupGet(it => it.Path).Returns(new PathString(""));
            httpContext.SetupGet(it => it.Request).Returns(request.Object);

            var route = new RouteInfo("GET", "", Pattern.FireAndForget);

            bool result = route.Match(httpContext.Object);

            Assert.True(result);
        }

        [Fact]
        public void RouteIsPart()
        {
            var request = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();

            request.SetupGet(it => it.Method).Returns("GET");
            request.SetupGet(it => it.Path).Returns(new PathString("/api/controller/action"));
            httpContext.SetupGet(it => it.Request).Returns(request.Object);

            var route = new RouteInfo("GET", "/api/", Pattern.FireAndForget);

            bool result = route.Match(httpContext.Object);

            Assert.True(result);
        }

        [Fact]
        public void RouteMethodWildCard()
        {
            var request = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();

            request.SetupGet(it => it.Method).Returns("GET");
            request.SetupGet(it => it.Path).Returns(new PathString("/api/controller/action"));
            httpContext.SetupGet(it => it.Request).Returns(request.Object);

            var route = new RouteInfo("*", "/api/", Pattern.FireAndForget);

            bool result = route.Match(httpContext.Object);

            Assert.True(result);
        }

        [Fact]
        public void RoutePathWildCard()
        {
            var request = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();

            request.SetupGet(it => it.Method).Returns("GET");
            request.SetupGet(it => it.Path).Returns(new PathString("/api/controller/action"));
            httpContext.SetupGet(it => it.Request).Returns(request.Object);

            var route = new RouteInfo("GET", "*", Pattern.FireAndForget);

            bool result = route.Match(httpContext.Object);

            Assert.True(result);
        }

        [Fact]
        public void RouteWildCardForAll()
        {
            var request = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();

            request.SetupGet(it => it.Method).Returns("GET");
            request.SetupGet(it => it.Path).Returns(new PathString("/api/controller/action"));
            httpContext.SetupGet(it => it.Request).Returns(request.Object);

            var route = new RouteInfo("*", "*", Pattern.FireAndForget);

            bool result = route.Match(httpContext.Object);

            Assert.True(result);
        }

        [Fact]
        public void NegativeForMethod()
        {
            var request = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();

            request.SetupGet(it => it.Method).Returns("POST");
            request.SetupGet(it => it.Path).Returns(new PathString("/api/controller/action"));
            httpContext.SetupGet(it => it.Request).Returns(request.Object);

            var route = new RouteInfo("GET", "*", Pattern.FireAndForget);

            bool result = route.Match(httpContext.Object);

            Assert.False(result);
        }

        [Fact]
        public void NegativeForPath()
        {
            var request = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();

            request.SetupGet(it => it.Method).Returns("GET");
            request.SetupGet(it => it.Path).Returns(new PathString("/api/controller/action"));
            httpContext.SetupGet(it => it.Request).Returns(request.Object);

            var route = new RouteInfo("*", "/other", Pattern.FireAndForget);

            bool result = route.Match(httpContext.Object);

            Assert.False(result);
        }
    }
}