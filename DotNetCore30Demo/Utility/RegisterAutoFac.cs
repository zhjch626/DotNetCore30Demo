using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore30Demo.Utility
{
    public class RegisterAutoFac
    {
        public static IServiceProvider ForRegisterAutoFac(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule<AutoFacModuleRegister>();
            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }
    }
}