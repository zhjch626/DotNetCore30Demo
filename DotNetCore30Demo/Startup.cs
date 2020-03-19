using System.Linq;
using System.Reflection;
using Autofac;
using DotNetCore30Demo.DataAccess;
using DotNetCore30Demo.IRepository;
using DotNetCore30Demo.Model;
using DotNetCore30Demo.Repository;
using DotNetCore30Demo.Resource;
using DotNetCore30Demo.Utility;
using DotNetCore30Demo.Utility.Helper;
using DotNetCore30Demo.Utility.MemoryCache;
using DotNetCore30Demo.Utility.Redis;
using EasyNetQ;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetCore30Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IHostEnvironment HostEnvironment { get; }

        private const string DefaultCorsPolicyName = "Demo.Cors";

        private const string ApiName = "Demo.Core";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region 部分服务注入-netcore自带方法
            // 缓存注入
            services.AddScoped<IMemoryCaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            // Redis注入
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
            #endregion

            #region CORS

            //配置跨域处理
            var urls = Configuration["AppSettings:Cores"].Split(',');
            //跨域第二种方法，声明策略，记得下边app中配置
            services.AddCors(c =>
            {
                //↓↓↓↓↓↓↓注意正式环境不要使用这种全开放的处理↓↓↓↓↓↓↓↓↓↓
                //c.AddPolicy("AllRequests", policy =>
                //{
                //    policy
                //        .AllowAnyOrigin()//允许任何源
                //        .AllowAnyMethod()//允许任何方式
                //        .AllowAnyHeader()//允许任何头
                //        .AllowCredentials();//允许cookie
                //});
                //↑↑↑↑↑↑↑注意正式环境不要使用这种全开放的处理↑↑↑↑↑↑↑↑↑↑


                //一般采用这种方法
                c.AddPolicy(DefaultCorsPolicyName, policy =>
                {
                    // 支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
                    // 注意，http://127.0.0.1:1818 和 http://localhost:1818 是不一样的，尽量写两个
                    policy
                        .WithOrigins(urls)
                        .AllowAnyHeader()//Ensures that the policy allows any header.
                        .AllowAnyMethod();
                });
            });

            //跨域第一种办法，注意下边 Configure 中进行配置
            #endregion

            #region AutoMapper
            services.AddAutoMapperProfiles();
            #endregion

            #region Controllers配置

            services.AddControllers(option =>
            {
                //统一处理返回结果
                option.Filters.Add<ApiResultFilterAttribute>();
                //全局异常过滤器
                option.Filters.Add<CustomExceptionFilterAttribute>();
            }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Validator>());

            #endregion

            #region 采用小写的 URL 路由模式

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            #endregion

            #region 更改全局验证返回信息
            //services.AddTransient<IValidator<SchoolResource>, SchoolAddAndUpdate>();
            //更改全局验证返回信息
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                    new ValidationFailedResult(actionContext.ModelState);
            });
            #endregion

            services.AddSingleton(new AppSettings(HostEnvironment.ContentRootPath));

            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Authority = "http://localhost:5000";
                    options.ApiName = "clientservice";
                });



            #region IoC - EventBus

            //services.AddSingleton(RabbitHutch.CreateBus("RabbitMQ:Dev"));

            #endregion

            #region DB
            services.AddChimp<ChimpDbContext>(opt => opt.UseSqlServer(Configuration["ConnectionStrings:MsSqlConnectionString"]));
            #endregion

            #region Swagger
            services.AddSwagger(ApiName);

            #endregion

            #region 注入Hangfile

            services.AddHangFile(Configuration);

            #endregion

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //添加依赖注入关系（Autofac）
            builder.RegisterModule<AutoFacModuleRegister>();

            var controllerBaseType = typeof(ControllerBase);

            //在控制器中使用依赖注入
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType).PropertiesAutowired();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //根据版本名称倒序 遍历展示
                typeof(CustomApiVersion.ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });
                // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：解决方案名.index.html
                //c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("index.html");//这里是配合MiniProfiler进行性能监控的，《文章：完美基于AOP的接口性能分析》，如果你不需要，可以暂时先注释掉，不影响大局。
                c.RoutePrefix = string.Empty;//路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
            });

            #endregion

            #region CORS
            //跨域第二种方法，使用策略，详细策略信息在ConfigureService中
            app.UseCors(DefaultCorsPolicyName);//将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求。


            #region 跨域第一种版本
            //跨域第一种版本，请要ConfigureService中配置服务 services.AddCors();
            //    app.UseCors(options => options.WithOrigins("http://localhost:8021").AllowAnyHeader()
            //.AllowAnyMethod());  
            #endregion

            #endregion

            //强制执行Https
           // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            #region Hangfire

            var enabled = Configuration["AppSettings:HangFire:Enabled"];
            if (!string.IsNullOrWhiteSpace(enabled) && bool.Parse(enabled))
            {
                app.UseHangfireServer();

                //1分钟执行一次
                //RecurringJob.AddOrUpdate<ISchoolRepository>(x => x.GetAll(), Cron.Minutely);
            }



            #endregion

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            // easyNetQ
           // app.UseSubscribe("ClientMessageService", new[] { Assembly.GetExecutingAssembly() });
        }
    }
}
