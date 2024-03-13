using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Middleware
{
    public class NoCacheMiddleware
    {
        private readonly RequestDelegate _next;

        public NoCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode == 200)
                {
                    string path = context.Request.Path.Value;
                    if (path.StartsWith("/wwwroot/provider_documents", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                        {
                            NoStore = true
                        };
                    }
                }

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
