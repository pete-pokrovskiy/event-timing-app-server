using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Croc.CFB.Web.Infrastructure
{
    public class LogAsyncActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requestId = Guid.NewGuid();

            Stopwatch requestTimer = new Stopwatch();
            requestTimer.Start();

            //var requestInfo = LogHelper.GetRequestInfo(context.HttpContext);
            //ClearTextFeedbackIfNeeded(requestInfo);

            //Log.Information("Request start. RequestId: {@requestId}. {@request}", requestId, requestInfo);

            var result = await next();

            requestTimer.Stop();
            //Log.Information("Request finish. RequestId: {@requestId}. Execution time (ms): {@time}", requestId, requestTimer.Elapsed);
        }       
    }
}
