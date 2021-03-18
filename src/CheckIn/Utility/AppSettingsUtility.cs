namespace CheckIn.Utility
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using Microsoft.Extensions.Configuration;
    public class AppSettingsUtility {

        /// <summary>
        /// 由Program.cs傳入
        /// </summary>
        public static bool isService = false;
        
        private static AppSettingsUtility _instance = null;

        private static IConfigurationBuilder _builder = null;

        public IConfigurationBuilder builder { get => _builder; }

        public String rootPath { get => getRootPath(); }

        public static AppSettingsUtility getInstance () {

            if (_instance == null) {

                _instance = new AppSettingsUtility ();    

                _builder = new ConfigurationBuilder ()
                    .SetBasePath (getRootPath())
                    .AddJsonFile ("appsettings.json");

            }

            return _instance;

        }

        /// <summary>
        /// 取得檔案路徑，區分是否註冊服務
        /// 用於 Docker 或 Web Server (使用Server相對路徑載入appsettings.json)
        /// _builder = new ConfigurationBuilder ()
        /// .SetBasePath (Directory.GetCurrentDirectory ())
        /// .AddJsonFile ("appsettings.json");
        /// 用於 Windows Service (使用檔案絕對路徑載入appsettings.json)
        /// </summary>
        /// <returns></returns>
        public static String getRootPath(){
            
            String rootPath = Directory.GetCurrentDirectory();

            if (AppSettingsUtility.isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                rootPath = Path.GetDirectoryName(pathToExe);   
            }

            return rootPath;
        }

        /// <summary>
        /// Get Local IP Address
        /// </summary>
        /// <returns></returns>
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                var ipString = ip.ToString();
                
                if (ip.AddressFamily == AddressFamily.InterNetwork && ipString.IndexOf(":")  == -1)
                {
                    if(ipString == "10.0.75.1" || ipString == "10.180.1.46" ){ //排除 OA端的DNS與七賢VIP
                        continue;
                    }                    
                    return ipString;
                }
            }
            return null;
        }
    }
}