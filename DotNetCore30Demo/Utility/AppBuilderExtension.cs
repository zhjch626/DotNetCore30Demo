using System.Reflection;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetCore30Demo.Utility
{
    public static class AppBuilderExtension
    {
        public static IApplicationBuilder UseSubscribe(this IApplicationBuilder appBuilder, string subscriptionIdPrefix, Assembly[] arrAssembly)
        {
            var services = appBuilder.ApplicationServices.CreateScope().ServiceProvider;

            var lifeTime = services.GetService<IHostApplicationLifetime>();
            var bus = services.GetService<IBus>();
            lifeTime.ApplicationStarted.Register(() =>
            {
                var subscriber = new AutoSubscriber(bus, subscriptionIdPrefix);
                subscriber.Subscribe(arrAssembly);
                subscriber.SubscribeAsync(arrAssembly);
            });

            lifeTime.ApplicationStopped.Register(() => bus.Dispose());

            return appBuilder;
        }
    }
}