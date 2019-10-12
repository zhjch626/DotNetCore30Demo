using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetCore30Demo.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DotNetCore30Demo.Utility
{
    public static class SwaggerServiceCollection
    {
        
        public static void AddSwagger(this IServiceCollection service,string apiName)
        {
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            service.AddSwaggerGen(options =>
            {
                //遍历出全部的版本，做文档信息展示
                typeof(CustomApiVersion.ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    options.SwaggerDoc(version, new OpenApiInfo()
                    {
                        // {ApiName} 定义成全局变量，方便修改
                        Title = $"{apiName} 接口文档",
                        Version = version,
                        Description = $"{apiName} HTTP API " + version,
                        //TermsOfService = new Uri(""),
                        Contact = new OpenApiContact()
                        {
                            Name = "Andy",
                            Email = "Andy@163.com",
                            Url = new Uri("https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.0&tabs=visual-studio")
                        }

                    });

                    // 按相对路径排序，作者：Andy
                    options.OrderActionsBy(o => o.RelativePath);
                });


                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath,true);//默认的第二个参数是false，这个是controller的注释，记得修改

                //var xmlModelPath = Path.Combine(basePath, "DotNetCore30Demo.Entity.xml");//这个就是Model层的xml文件名
                //options.IncludeXmlComments(xmlModelPath);


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"n", 
                    Name = "Authorization", //jwt默认的参数名称
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                    }, new string[] { }
                } });
            });
        }
    }
}