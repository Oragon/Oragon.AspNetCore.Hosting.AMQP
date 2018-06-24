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
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("MoAR_METHOD", context.Request.Method);
            props.Headers.Add("MoAR_PATH", context.Request.Path.ToString());
            props.Headers.Add("MoAR_CONTENTTYPE", context.Request.ContentType);
            props.Headers.Add("MoAR_CONTENTLENGTH", context.Request.ContentLength);
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
                if (props.Headers["MoAR_CONTENTTYPE"] != null)
                {
                    it.Content.Headers.ContentType = new MediaTypeHeaderValue(props.Headers["MoAR_CONTENTTYPE"].UTF8GetString());
                    it.Content.Headers.ContentLength = (long)props.Headers["MoAR_CONTENTLENGTH"];
                }
            });

            foreach (var header in props.Headers)
            {
                if (header.Key.StartsWith("MoAR_") == false)
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
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("MoAR_STATUSCODE", (int)response.StatusCode);
            props.Headers.Add("MoAR_CONTENTTYPE", response.Content.Headers.ContentType?.ToString());
            props.Headers.Add("MoAR_CONTENTLENGTH", response.Content.Headers.ContentLength ?? 0);
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
            context.Response.Headers.Clear();
            context.Response.StatusCode = (int)props.Headers["MoAR_STATUSCODE"];
            context.Response.ContentType = props.Headers["MoAR_CONTENTTYPE"].UTF8GetString();
            context.Response.ContentLength = props.Headers["MoAR_CONTENTLENGTH"].UTF8GetLong();
            foreach (var header in props.Headers)
            {
                if (header.Key.StartsWith("MoAR_") == false)
                    context.Response.Headers.Add(header.Key, header.Value.UTF8GetString());
            }
        }


    }
}
