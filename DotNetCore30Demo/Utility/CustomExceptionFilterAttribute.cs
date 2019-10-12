using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DotNetCore30Demo.Utility
{
    public class CustomExceptionFilterAttribute : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)//异常没有被处理
            {
                HttpStatusCode status = HttpStatusCode.InternalServerError;
                //写入日志
                _logger.LogError(context.Exception.ToString());
                context.Result = new CustomExceptionResult((int)status, context.Exception);
                context.ExceptionHandled = true;
            }

        }
    }
}