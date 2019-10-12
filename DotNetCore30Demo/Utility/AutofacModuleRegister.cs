using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace DotNetCore30Demo.Utility
{
    public class AutoFacModuleRegister:Module
    {
        /// <summary>
        /// 重新AutoFac管道Load方法，在这里注册注入
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            //必须是Service 结束的
           //builder.RegisterAssemblyTypes(GetAssemblyByName("")).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(GetAssemblyByName("DotNetCore30Demo.Repository")).Where(x => x.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();
            //单一注入
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