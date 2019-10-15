using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using DotNetCore30Demo.AOP;
using DotNetCore30Demo.Utility.Helper;
using DotNetCore30Demo.Utility.MemoryCache;
using DotNetCore30Demo.Utility.Redis;
using Module = Autofac.Module;

namespace DotNetCore30Demo.Utility
{
    public class AutoFacModuleRegister : Module
    {

        /// <summary>
        /// 重新AutoFac管道Load方法，在这里注册注入
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            var cacheType = new List<Type>();
            //单一注入
            builder.RegisterType<LogAop>().InstancePerLifetimeScope();
            builder.RegisterType<RedisCacheAop>().InstancePerLifetimeScope();
            builder.RegisterType<MemoryCacheAop>().InstancePerLifetimeScope();

            if (AppSettings.App("AppSettings", "RedisCachingAOP", "Enabled").ObjToBool())
            {
                cacheType.Add(typeof(RedisCacheAop));
            }

            if (AppSettings.App("AppSettings", "MemoryCachingAOP", "Enabled").ObjToBool())
            {
                cacheType.Add(typeof(MemoryCacheAop));
            }

            if (AppSettings.App("AppSettings", "LogAOP", "Enabled").ObjToBool())
            {
                cacheType.Add(typeof(LogAop));
            }
            //必须是Service 结束的
            //builder.RegisterAssemblyTypes(GetAssemblyByName("")).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(GetAssemblyByName("DotNetCore30Demo.Repository")).Where(x => x.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .EnableInterfaceInterceptors()
                .InterceptedBy(cacheType.ToArray());
            

        }
        /// <summary>
        /// 根据程序集名称获取程序集
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly GetAssemblyByName(string assemblyName)
        {
            return Assembly.Load(assemblyName);
        }
    }
}