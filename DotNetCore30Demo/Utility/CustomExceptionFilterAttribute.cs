using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DotNetCore30Demo.Utility
{
    public class CustomExceptionFilterAttribute : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)//异常没有被处理
            {
                HttpStatusCode status = HttpStatusCode.InternalServerError;
                //写入日志
                _logger.LogError(WriteLog(context.Exception.Message, context.Exception));
                context.Result = new CustomExceptionResult((int)status, context.Exception);
                context.ExceptionHandled = true;
            }

        }

        /// <summary>
        /// 自定义返回格式
        /// </summary>
        /// <param name="throwMsg"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public string WriteLog(string throwMsg, Exception ex)
        {
            return $"【自定义错误】：{throwMsg} \r\n【异常类型】：{ex.GetType().Name} \r\n【异常信息】：{ex.Message} \r\n【堆栈调用】：{ex.StackTrace}";
        }
    }
}