using System;
using System.IO;

namespace CheckIn.Utility
{
 public class LogFile
    {
        private static LogFile _instance = new LogFile();

       
        
        public static LogFile getInstance()
        {
            
            string path = AppSettingsUtility.getInstance().rootPath;
            string logFolder = $@"{path}/logs";
            if (Directory.Exists(logFolder))
            {
                //資料夾存在
            }
            else
            {
                //新增資料夾
                Directory.CreateDirectory($@"{logFolder}");
            }

          

            return _instance;
        }

        
        
        /// <summary>
        /// 傳入檔名與內容寫入log
        /// </summary>
        /// <param name="content">寫入內容</param>
        /// <param name="fileName">檔名</param>
        public bool writeContent(string content, string fileName="log")
        {
            string path = AppSettingsUtility.getInstance().rootPath;
            string time = DateTime.Now.ToString("yyyyMMddHHmmss.fff");
            fileName += $"_{time}_log.txt";
            try
            {  
                using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"{path}/logs/{fileName}", true))
                {
                    file.WriteLine("/***" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "***/");
                    file.WriteLine(content);
                }
                return true;
            }
            catch (Exception)
            {
                //寫入失敗
                return false;         
            }
            
        }
    }
}