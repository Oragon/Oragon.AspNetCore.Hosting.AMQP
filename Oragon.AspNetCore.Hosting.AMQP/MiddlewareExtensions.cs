using Microsoft.AspNetCore.Builder;
using Oragon.AspNetCore.Hosting.AMQP.Server;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMoARServer(this IApplicationBuilder app, Action<Configuration> builderConfigurer)
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
