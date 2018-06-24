using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oragon.AspNetCore.Hosting.AMQP;
using RabbitMQ.Client;

namespace Oragon.AspNetCore.Hosting.AMQP.IntegratedTests.HAProxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConnectionFactory>(new ConnectionFactory()
            {
                HostName = "192.168.1.88",
                UserName = "usuario",
                Password = "senha",
                VirtualHost = "exemplo_amqp",
                Port = 5672
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseMoARServer(it =>
            it.WithGroupName("dynamic_api")
            .WithRoute("GET", "/api/", Pattern.FireAndForget)
            .WithRoute("*", "/api/", Pattern.FireAndForget)
            .WithPoolSize(100)
            .WithConnectionFactory(app.ApplicationServices.GetRequiredService<IConnectionFactory>())
        );

            app.UseMvc();


        }
    }
}
