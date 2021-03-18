using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CheckIn.Utility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;

namespace CheckIn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //不是debug模式也參數中也沒有--service，代表是一個服務
            var isService = (!Debugger.IsAttached && args.Contains ("--service"));

            //把服務資訊傳給AppSettingsUtility
            AppSettingsUtility.isService = isService;

            //顯示
            System.Console.WriteLine ($"isService : {isService}");
            //因為 ASP.NET Core 組態需要命令列引數成對的名稱和數值，所以會先移除 --console 參數
            args = args.Where (arg => arg != "--service").ToArray ();
           

            if (isService) //佈署windows service
            {
                var builder = CreateWinServiceHostBuilder (args);

                var host = builder.Build();

                host.Run();

            } else {
                
                var builder = CreateHostBuilder (args);

                var host = builder.Build();
                
                host.Run();
            }

        }

        private static void setting(IWebHostBuilder webBuilder){
            string[] urls = { "http://localhost:8080"};//, "https://localhost:8012" };

            //讀取設定檔
            var configuration = AppSettingsUtility.getInstance ().builder.Build ();

            var str_urls = configuration["server.urls"];
            if (!String.IsNullOrWhiteSpace (str_urls)) {
                urls = str_urls.Split (";");
            }
            Console.WriteLine ("urls " + urls[0]);

            var str_KeepAliveTimeout = configuration["KestrelSetting:KeepAliveTimeout"];
            var str_RequestHeadersTimeout = configuration["KestrelSetting:RequestHeadersTimeout"];

            //HTTP持久連線的時間。
            int keepAliveTimeout;
            int.TryParse (str_KeepAliveTimeout, out keepAliveTimeout);
            keepAliveTimeout = (keepAliveTimeout <= 0) ? 2 * 60 : keepAliveTimeout;

            //Server 處理一個封包最長的時間。
            int requestHeadersTimeout = 30;
            int.TryParse (str_RequestHeadersTimeout, out requestHeadersTimeout);
            requestHeadersTimeout = (requestHeadersTimeout <= 0) ? 30 : requestHeadersTimeout;

            System.Console.WriteLine ($"keepAliveTimeout:{keepAliveTimeout} / requestHeadersTimeout:{requestHeadersTimeout}");
            var runtime = configuration["server.Runtime"];
            if (!String.IsNullOrEmpty(runtime) && runtime != "IIS")
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.AddServerHeader = false;
                    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromSeconds (keepAliveTimeout);
                    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds (requestHeadersTimeout);
                })
                .UseUrls(urls)                    
                .UseStartup<Startup>();
            }else{
                webBuilder.UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration();
            }
        }

        public static IHostBuilder CreateWinServiceHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder => setting(webBuilder));

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => setting(webBuilder));
    

         
        

    }
}
