using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    public static class ContextAdapter
    {

        /// <summary>
        /// 1
        /// </summary>
        /// <param name="props"></param>
        /// <param name="context"></param>
        public static void FillPropertiesFromRequest(IBasicProperties props, HttpContext context)
        {
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("AMQP_METHOD", context.Request.Method);
            props.Headers.Add("AMQP_PATH", context.Request.Path.ToString());
            props.Headers.Add("AMQP_CONTENTTYPE", context.Request.ContentType);
            props.Headers.Add("AMQP_CONTENTLENGTH", context.Request.ContentLength);
            foreach (var header in context.Request.Headers)
            {
                props.Headers.Add(header.Key, header.Value.ToString());
            }
        }

        /// <summary>
        /// 2
        /// </summary>
        /// <param name="props"></param>
        /// <param name="requestBuilder"></param>
        public static void FillRequestBuilderFromProperties(IBasicProperties props, RequestBuilder requestBuilder)
        {
            requestBuilder.And(it =>
            {
                if (props.Headers["AMQP_CONTENTTYPE"] != null)
                {
                    it.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(props.Headers["AMQP_CONTENTTYPE"].UTF8GetString());
                    it.Content.Headers.ContentLength = (long)props.Headers["AMQP_CONTENTLENGTH"];
                }
            });

            foreach (var header in props.Headers)
            {
                if (header.Key.StartsWith("AMQP_") == false)
                    requestBuilder.AddHeader(header.Key, header.Value.UTF8GetString());
            }
        }

        /// <summary>
        /// 3
        /// </summary>
        /// <param name="props"></param>
        /// <param name="response"></param>
        public static void FillPropertiesFromResponse(IBasicProperties props, HttpResponseMessage response)
        {
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("AMQP_STATUSCODE", (int)response.StatusCode);
            props.Headers.Add("AMQP_CONTENTTYPE", response.Content.Headers.ContentType?.ToString());
            props.Headers.Add("AMQP_CONTENTLENGTH", response.Content.Headers.ContentLength ?? 0);
            foreach (var header in response.Headers)
            {
                props.Headers.Add(header.Key, header.Value.ToString());
            }
        }

        /// <summary>
        /// 4
        /// </summary>
        /// <param name="context"></param>
        /// <param name="props"></param>
        public static void FillResponseFromProperties(HttpContext context, IBasicProperties props)
        {
            context.Response.StatusCode = (int)props.Headers["AMQP_STATUSCODE"];
            context.Response.ContentType = props.Headers["AMQP_CONTENTTYPE"].UTF8GetString();
            context.Response.ContentLength = props.Headers["AMQP_CONTENTLENGTH"].UTF8GetLong();
            foreach (var header in props.Headers)
            {
                if (header.Key.StartsWith("AMQP_") == false)
                    context.Response.Headers.Add(header.Key, header.Value.UTF8GetString());
            }
        }


    }
}
