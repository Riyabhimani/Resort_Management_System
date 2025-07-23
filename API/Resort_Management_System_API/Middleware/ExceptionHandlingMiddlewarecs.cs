using Newtonsoft.Json;
using System.Net;

namespace Resort_Management_System_API.Middleware
{
    public class ExceptionHandlingMiddlewarecs
    {
        public class ExceptionHandlingMiddleware : IMiddleware
        {
            public async Task InvokeAsync(HttpContext context, RequestDelegate next)
            {
                try
                {
                    await next(context);
                }
                catch (Exception ex)
                {
                    await HandleException(context, ex);
                }
            }

            private Task HandleException(HttpContext context, Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message,
                    Status = "ERROR",
                    Details = ex.InnerException?.Message
                }));
            }
        }
    }
}
