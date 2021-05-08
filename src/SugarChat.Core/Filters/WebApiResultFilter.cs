using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SugarChat.Core.Basic;
using SugarChat.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Filters
{
    public class WebApiResultFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if(objectResult.Value is BasicResponse)
                {

                }
                else if (objectResult.Value == null)
                {
                    context.Result = new ObjectResult(new BasicResponse((int)CommonExceptionEnum.NotFound, "Resouce Not Found"));
                }
                else
                {
                    context.Result = new ObjectResult(new BasicResponse<object>(objectResult.Value));
                }
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new BasicResponse((int)CommonExceptionEnum.NotFound, "Resouce Not Found"));
            }
            else if (context.Result is ContentResult)
            {
                context.Result = new ObjectResult(new BasicResponse<string>((context.Result as ContentResult).Content));
            }
            // Consider whether you need it or not.

            //else if (context.Result is StatusCodeResult)
            //{
            //    context.Result = new ObjectResult(new BasicResponse((context.Result as StatusCodeResult).StatusCode, ""));
            //}
        }
    }
}
