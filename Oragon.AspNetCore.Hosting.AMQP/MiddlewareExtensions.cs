using Microsoft.AspNetCore.Builder;
using Oragon.AspNetCore.Hosting.AMQP.Server;
using System;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAmqp(this IApplicationBuilder app, Action<Configuration> builderConfigurer)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            var builderConfiguration = new Configuration();
            builderConfigurer(builderConfiguration);
            return app.UseMiddleware<AMQPServerMiddleware>(new object[] { builderConfiguration });
        }
    }
}