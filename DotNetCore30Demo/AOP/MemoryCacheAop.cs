using System.Reflection;
using Castle.DynamicProxy;
using DotNetCore30Demo.Utility.Attribute;
using DotNetCore30Demo.Utility.MemoryCache;

namespace DotNetCore30Demo.AOP
{
    public class MemoryCacheAop : CacheAopBase
    {
        //通过注入的方式，把缓存操作接口通过构造函数注入
        private readonly IMemoryCaching _cache;
        public MemoryCacheAop(IMemoryCaching cache)
        {
            _cache = cache;
        }
        //Intercept方法是拦截的关键所在，也是IInterceptor接口中的唯一定义
        public override void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法的特性验证
            //如果需要验证
            if (method.IsDefined(typeof(CachingAttribute), true))
            {
                var attribute = (CachingAttribute)method.GetCustomAttribute(typeof(CachingAttribute), true);
                //获取自定义缓存键
                var cacheKey = CustomCacheKey(invocation);

                //根据key获取相应的缓存值
                var cacheValue = _cache.Get<object>(cacheKey);

                if (cacheValue != null)
                {
                    //将当前获取到的缓存值，赋值给当前执行方法
                    invocation.ReturnValue = cacheValue;
                    return;
                } //去执行当前的方法
                invocation.Proceed();
                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    _cache.Set(cacheKey, invocation.ReturnValue, attribute.AbsoluteExpiration);
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }
    }
}