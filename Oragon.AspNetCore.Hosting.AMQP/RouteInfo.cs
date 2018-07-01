using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    public class RouteInfo
    {
        private string route;
        public string Route { get { return this.route; } internal set { this.route = value.ToLowerInvariant(); } }

        public Pattern Pattern { get; internal set; }


        private string method;
        public string Method { get { return this.method; } internal set { this.method = value.ToLowerInvariant(); } }


        public RouteInfo() { }

        public RouteInfo(string method, string route, Pattern pattern)
        {
            this.Method = method;
            this.Route = route;
            this.Pattern = pattern;
        }

        public bool Match(HttpContext context)
        {
            return
                (this.method == "*" || context.Request.Method.ToLowerInvariant() == this.method)
                &&
                (this.route == "*" || context.Request.Path.Value.ToLowerInvariant().StartsWith(this.route))
                ;
        }
    }
}
