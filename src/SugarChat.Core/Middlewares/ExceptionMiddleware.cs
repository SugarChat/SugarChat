using Microsoft.AspNetCore.Http;
using SugarChat.Core.Basic;
using SugarChat.Core.Common;
using SugarChat.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception e)
            {
                await HandleException(context, e);
                return;
            }
        }

        private async Task HandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/json;charset=utf-8;";
            string error = "";
            if(e is BusinessException bex)
            {
                var json = new BasicResponse(bex.Code, bex.Message);
                error = JsonSerializer.Serialize(json);
            }
            else
            {
                var json = new BasicResponse((int)CommonExceptionEnum.InternalError, e.Message);
                error = JsonSerializer.Serialize(json);
            }

            await context.Response.WriteAsync(error);
        }
    }
}
