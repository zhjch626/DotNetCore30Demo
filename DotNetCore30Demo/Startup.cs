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

            #region ���ַ���ע��-netcore�Դ�����
            // ����ע��
            services.AddScoped<IMemoryCaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            // Redisע��
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
            #endregion

            #region CORS

            //���ÿ�����
            var urls = Configuration["AppSettings:Cores"].Split(',');
            //����ڶ��ַ������������ԣ��ǵ��±�app������
            services.AddCors(c =>
            {
                //��������������ע����ʽ������Ҫʹ������ȫ���ŵĴ����������������������
                //c.AddPolicy("AllRequests", policy =>
                //{
                //    policy
                //        .AllowAnyOrigin()//�����κ�Դ
                //        .AllowAnyMethod()//�����κη�ʽ
                //        .AllowAnyHeader()//�����κ�ͷ
                //        .AllowCredentials();//����cookie
                //});
                //��������������ע����ʽ������Ҫʹ������ȫ���ŵĴ����������������������


                //һ��������ַ���
                c.AddPolicy(DefaultCorsPolicyName, policy =>
                {
                    // ֧�ֶ�������˿ڣ�ע��˿ںź�Ҫ��/б�ˣ�����localhost:8000/���Ǵ��
                    // ע�⣬http://127.0.0.1:1818 �� http://localhost:1818 �ǲ�һ���ģ�����д����
                    policy
                        .WithOrigins(urls)
                        .AllowAnyHeader()//Ensures that the policy allows any header.
                        .AllowAnyMethod();
                });
            });

            //�����һ�ְ취��ע���±� Configure �н�������
            #endregion

            #region AutoMapper
            services.AddAutoMapperProfiles();
            #endregion

            #region Controllers����

            services.AddControllers(option =>
            {
                //ͳһ�����ؽ��
                option.Filters.Add<ApiResultFilterAttribute>();
                //ȫ���쳣������
                option.Filters.Add<CustomExceptionFilterAttribute>();
            }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Validator>());

            #endregion

            #region ����Сд�� URL ·��ģʽ

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            #endregion

            #region ����ȫ����֤������Ϣ
            //services.AddTransient<IValidator<SchoolResource>, SchoolAddAndUpdate>();
            //����ȫ����֤������Ϣ
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

            #region ע��Hangfile

            services.AddHangFile(Configuration);

            #endregion

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //�������ע���ϵ��Autofac��
            builder.RegisterModule<AutoFacModuleRegister>();

            var controllerBaseType = typeof(ControllerBase);

            //�ڿ�������ʹ������ע��
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
                //���ݰ汾���Ƶ��� ����չʾ
                typeof(CustomApiVersion.ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });
                // ��swagger��ҳ�����ó������Զ����ҳ�棬�ǵ�����ַ�����д�������������.index.html
                //c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("index.html");//���������MiniProfiler�������ܼ�صģ������£���������AOP�Ľӿ����ܷ�����������㲻��Ҫ��������ʱ��ע�͵�����Ӱ���֡�
                c.RoutePrefix = string.Empty;//·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�,ע��localhost:8001/swagger�Ƿ��ʲ����ģ�ȥlaunchSettings.json��launchUrlȥ����������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "doc";
            });

            #endregion

            #region CORS
            //����ڶ��ַ�����ʹ�ò��ԣ���ϸ������Ϣ��ConfigureService��
            app.UseCors(DefaultCorsPolicyName);//�� CORS �м����ӵ� web Ӧ�ó��������, �������������


            #region �����һ�ְ汾
            //�����һ�ְ汾����ҪConfigureService�����÷��� services.AddCors();
            //    app.UseCors(options => options.WithOrigins("http://localhost:8021").AllowAnyHeader()
            //.AllowAnyMethod());  
            #endregion

            #endregion

            //ǿ��ִ��Https
           // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            #region Hangfire

            var enabled = Configuration["AppSettings:HangFire:Enabled"];
            if (!string.IsNullOrWhiteSpace(enabled) && bool.Parse(enabled))
            {
                app.UseHangfireServer();

                //1����ִ��һ��
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
