using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace DotNetCore30Demo.AOP
{
    /// <summary>
    /// 拦截器LogAop 继承IInterceptor接口
    /// </summary>
    public class LogAop : IInterceptor
    {
        private readonly ILogger<LogAop> _logger;

        public LogAop(ILogger<LogAop> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 实例化IInterceptor唯一方法 
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            //记录被拦截方法信息的日志信息
            var dataIntercept = "" +
                                $"【当前执行方法】：{ invocation.Method.Name} \r\n" +
                                $"【携带的参数有】： {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())} \r\n";

            MiniProfiler.Current.Step($"执行Repository方法：{invocation.Method.Name}() -> ");
            //在被拦截的方法执行完毕后 继续执行当前方法，注意是被拦截的是异步的
            invocation.Proceed();

            //执行之后做处理
            //todo
            _logger.LogInformation(dataIntercept);
        }
    }
}