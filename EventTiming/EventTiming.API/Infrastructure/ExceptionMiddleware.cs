using EventTiming.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace EventTiming.API.Infrastructure
{

    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }

    public class ExceptionMiddleware
    {

        private readonly RequestDelegate next;


        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {

            //var code = GetResponseCode(ex);//( HttpStatusCode.InternalServerError; // 500 if unexpected

            var result = JsonConvert.SerializeObject(new { error = ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            //пишем лог
            //var logId = Guid.NewGuid();
            
            //var requestInfo = LogHelper.GetRequestInfo(context);

            //Log.ForContext("ErrorId", logId).Error(ex, "Произошла ошибка. Параметры запроса: {@requestData}", requestInfo);

            //queue.QueueBackgroundWorkItem(async token =>
            //{
            //    //отправляем сообщение на поддержку
            //    errorProcessingService.SendErrorToSupport(new ErrorProcessingParams(ex, logId, requestInfo.User, AppConstants.AppName, false));
            //});

            return context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                SystemErrorOccurred = true,
                SystemErrorMessage = ex.ToBetterString()
            }
            //,
            //new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //}
            ));
        }

        
    }
}
