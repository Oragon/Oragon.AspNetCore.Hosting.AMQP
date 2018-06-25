using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    public class RouteInfo
    {
        public string Route { get; internal set; }

        public Pattern Pattern { get; internal set; }

        public string Method { get; internal set; }



        public bool Match(HttpContext context)
        {
            return
                (this.Method == "*" || context.Request.Method.ToUpperInvariant() == this.Method.ToUpperInvariant())
                &&
                context.Request.Path.Value.ToLowerInvariant().StartsWith(this.Route.ToLowerInvariant());
        }
    }
}
