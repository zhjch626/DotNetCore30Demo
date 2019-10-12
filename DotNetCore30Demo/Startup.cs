using System.Linq;
using Autofac;
using DotNetCore30Demo.DataAccess;
using DotNetCore30Demo.Model;
using DotNetCore30Demo.Repository;
using DotNetCore30Demo.Resource;
using DotNetCore30Demo.Utility;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetCore30Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string DefaultCorsPolicyName = "Demo.Cors";

        private const string ApiName = "Demo.Core";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region CORS

            //���ÿ�����
            var urls = Configuration["AppConfig:Cores"].Split(',');
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

            #region DB
            services.AddChimp<ChimpDbContext>(opt => opt.UseSqlServer(Configuration["ConnectionStrings:MsSqlConnectionString"]));
            #endregion

            #region Swagger
            services.AddSwagger(ApiName);

            #endregion
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //�������ע���ϵ
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
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
