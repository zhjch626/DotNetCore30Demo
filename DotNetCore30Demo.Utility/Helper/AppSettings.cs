using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace DotNetCore30Demo.Utility.Helper
{
    /// <summary>
    /// appsettings.json
    /// </summary>
    public class AppSettings
    {
        private static IConfiguration Configuration { get; set; }

        public AppSettings(string contentPath)
        {
            string path = "appsettings.json";

            //如果你把配置文件 是 根据环境变量来分开了，可以这样写
            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";
            //Configuration = new ConfigurationBuilder()
            //.Add(new JsonConfigurationSource { Path = Path, ReloadOnChange = true })//请注意要把当前appsetting.json 文件->右键->属性->复制到输出目录->始终复制
            //.Build();

            Configuration = new ConfigurationBuilder()
                    .SetBasePath(contentPath)
                    .Add(new JsonConfigurationSource()
                    {
                        Path = path,
                        Optional = false,
                        ReloadOnChange = true 
                    })//这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
                    .Build();
        }
        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static string App(params string[] sections)
        {
            try
            {
                var val = string.Empty;
                foreach (var t in sections)
                {
                    val += $"{t}:";
                }
                return Configuration[val.TrimEnd(':')];
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}