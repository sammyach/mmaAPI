using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mmaAPI.Helpers
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var uuid = context.Request.Headers["x-uuid"].FirstOrDefault();
            //if (uuid == null) // it's a new session
            //{
            //    uuid = Guid.NewGuid().ToString();
            //}
            context.Items["uuid"] = uuid;

            await _next(context);
        }
    }
}
