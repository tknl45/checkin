using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CheckIn.Utility
{
    public class LogService {

        private static LogService _instance = null;
        private static string _hostname = null;

        private static string _uri = null;
        private static string _uri_elk = null;

        private static string _logLevel = null;

        /// <summary>
        /// 實例
        /// </summary>
        /// <returns></returns>
        public static LogService getInstance () {
            
            if (_instance == null) {
                //讀取設定檔
                var configuration = AppSettingsUtility.getInstance ().builder.Build ();

                _instance = new LogService ();
                _hostname = AppSettingsUtility.getInstance().GetLocalIPAddress() ?? Environment.GetEnvironmentVariable ("COMPUTERNAME") ?? Environment.GetEnvironmentVariable ("HOSTNAME");
                _uri = configuration["LogService:Uri"];
                _uri_elk = configuration["LogService:Elasticsearch"];
                _logLevel = configuration["LogService:LogLevel"];
            }

            return _instance;
        }

       

        String ShowTreeMessage(Exception e){
            String result = e.Message;
            var tabCount=1;
            while(e.InnerException != null){
                e = e.InnerException;
                result += "\r\n";
                for (int i = 0; i < tabCount; i++)
                {
                    result += "\t";
                }
                result += e.Message;
                tabCount++;

            }

            return result;
        }

       

        /// <summary>
        /// EAI失敗紀錄
        /// </summary>
        /// <param name="EAICommand"></param>
        /// <param name="EAIParameter"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="strDeviceNo"></param>
        /// <param name="CHTMemberSN"></param>
        /// <param name="CHTMemberId"></param>
        /// <param name="logTarget"></param>
        /// <returns></returns>
        public void EAIError (string EAICommand, string EAIParameter, string ErrorMsg, string strDeviceNo, string CHTMemberSN = "", string CHTMemberId = "", string logTarget="Default") {
            var url = (logTarget == "Default") ? $"{_uri}/Log/error" :  (logTarget == "ELK") ? $"{_uri_elk}/log/elk/error" : logTarget;



            if (_logLevel == "none") return;

            StringBuilder logSB = new StringBuilder ();

            string strEX = Regex.Replace (ErrorMsg, "[%!&+'\"?<>/\\\\]+", "");
            if (strEX.Length > 200) {
                strEX = strEX.Substring (0, 200);
            }

            Dictionary<string, object> para = new Dictionary<string, object> ();
            para.Add ("eaiCommand", EAICommand.Length > 50 ? EAICommand.Substring (0, 50) : EAICommand);
            para.Add ("eaiParameter", EAIParameter.Length > 500 ? EAIParameter.Substring (0, 500) : EAIParameter);
            para.Add ("errorMsg", ErrorMsg.Length > 200 ? ErrorMsg.Substring (0, 200) : ErrorMsg);
            para.Add ("custSN", CHTMemberSN);
            para.Add ("serverIP", _hostname);
            para.Add ("custID", CHTMemberId);
            para.Add ("deviceNo", strDeviceNo);
            para.Add ("receiveNo", "");

            string json = JsonConvert.SerializeObject (para, Formatting.Indented);

            //CONTENT-TYPE header
            var httpContent = new StringContent (json,
                Encoding.UTF8,
                "application/json");
            

            LogFile.getInstance ().writeContent (url);
            LogFile.getInstance ().writeContent (json);

            HttpRequestMessage postRequest = new HttpRequestMessage (HttpMethod.Post, url) {
                Content = httpContent
            };

            try {

                logSB.AppendLine ("URL:" + url);
                logSB.AppendLine ("INPUT:" + json);

                HttpClient _client = new HttpClient ();

                Task<HttpResponseMessage> response = _client.SendAsync (postRequest);

                response.Result.EnsureSuccessStatusCode ();

                var responseString = response.Result.Content.ReadAsStringAsync ().Result;

                logSB.AppendLine ("RESULT:" + responseString);

                if (_logLevel == "debug") LogFile.getInstance ().writeContent (logSB.ToString ());
            } catch (System.Exception e) {
                var msg = ShowTreeMessage(e);
                Console.WriteLine ("寫入紀錄發生錯誤：" +msg);
                logSB.AppendLine ("ERROR：" + msg);
                LogFile.getInstance ().writeContent (logSB.ToString (), "ERROR");

            }
        }

        /// <summary>
        /// 紀錄全部eai的結果
        /// </summary>
        /// <param name="Endpoint">Endpoint</param>
        /// <param name="Query_string">Query_string</param>
        /// <param name="Header_string"></param>
        /// <param name="Body">Body</param>
        /// <param name="Method"></param>
        /// <param name="Status"></param>
        /// <param name="ReturnMessage"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="OutputData"></param>
        /// <returns></returns>
        public void RestAPI_OUTPUTDATA(string Endpoint, string Query_string, string Header_string, string Body, string Method, string Status, string ReturnMessage, DateTime StartTime, DateTime EndTime,  string OutputData)
        {

            if (_logLevel == "none") return;
            if (_uri == null) return;

            StringBuilder logSB = new StringBuilder();

            Dictionary<string, object> para = new Dictionary<string, object>();
            para.Add("Endpoint", Endpoint);
            para.Add("Query_string", Query_string);
            para.Add("Header_string", Header_string);
            para.Add("Body", Body);
            para.Add("Method", Method);
            para.Add("Status", Status);
            para.Add("ReturnMessage", ReturnMessage);
            para.Add("StartTime", StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            para.Add("EndTime", EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            String InputData = "Method:" + Method +"/Query_string:" + Query_string + "/Header_string:" + Header_string + "/Body:" + Body;
            para.Add("InputData", InputData);
            para.Add("OutputData", OutputData);
            para.Add("ClientIP", _hostname);

            string json = JsonConvert.SerializeObject(para, Formatting.Indented);

            //CONTENT-TYPE header
            var httpContent = new StringContent(json,
                Encoding.UTF8,
                "application/json");
            var url = $"{_uri}/Log/RestAPI_outputData";

            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = httpContent
            };

            HttpClient _client = new HttpClient();

            Task<HttpResponseMessage> response = _client.SendAsync(postRequest);

            try
            {
                logSB.AppendLine("URL:" + url);
                logSB.AppendLine("INPUT:" + json);

                response.Result.EnsureSuccessStatusCode();
                var responseString = response.Result.Content.ReadAsStringAsync().Result;
                logSB.AppendLine("RESULT:" + responseString);
            }
            catch (System.Exception e)
            {
                var msg = ShowTreeMessage(e);
                Console.WriteLine("寫入紀錄發生錯誤：" + msg);
                logSB.AppendLine("ERROR：" + msg);



                Task.Factory.StartNew(
                       () => EAIError(Endpoint, InputData, e.Message, "")
                );
                
            }

        }

        
        /// <summary>
        /// 紀錄REST呼叫後的結果
        /// </summary>
        /// <param name="Endpoint">Endpoint</param>
        /// <param name="Query_string">Query_string</param>
        /// <param name="Header_string"></param>
        /// <param name="Body">Body</param>
        /// <param name="Method"></param>
        /// <param name="Status"></param>
        /// <param name="ReturnMessage"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="OutputData"></param>
        /// <returns></returns>
        public void REST_OUTPUT(string Endpoint, string Query_string, string Header_string, string Body, string Method, string Status, string ReturnMessage, DateTime StartTime, DateTime EndTime,  string OutputData)
        {
            var collection = "REST_OUTPUT";
            if (_logLevel == "none") return;
            if (_uri == null) return;

            StringBuilder logSB = new StringBuilder();

            Dictionary<string, object> para = new Dictionary<string, object>();
            para.Add("Endpoint", Endpoint);
            para.Add("Query_string", Query_string);
            para.Add("Header_string", Header_string);
            para.Add("Body", Body);
            para.Add("Method", Method);
            para.Add("Status", Status);
            para.Add("ReturnMessage", ReturnMessage);
            para.Add("StartTime", StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            para.Add("EndTime", EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            TimeSpan timeDiff = EndTime - StartTime;            
            para.Add ("TotalSeconds", timeDiff.TotalSeconds);
            String InputData = "Method:" + Method +"/Query_string:" + Query_string + "/Header_string:" + Header_string + "/Body:" + Body;
            para.Add("InputData", InputData);
            para.Add("OutputData", OutputData);
            para.Add("ClientIP", _hostname);

            string json = JsonConvert.SerializeObject(para, Formatting.Indented);

            //CONTENT-TYPE header
            var httpContent = new StringContent(json,
                Encoding.UTF8,
                "application/json");
            var url = $"{_uri}/Log/logData?collection="+ collection;

            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = httpContent
            };

            HttpClient _client = new HttpClient();

            Task<HttpResponseMessage> response = _client.SendAsync(postRequest);

            try
            {
                logSB.AppendLine("URL:" + url);
                logSB.AppendLine("INPUT:" + json);

                response.Result.EnsureSuccessStatusCode();
                var responseString = response.Result.Content.ReadAsStringAsync().Result;
                logSB.AppendLine("RESULT:" + responseString);

                if (_logLevel == "debug") LogFile.getInstance().writeContent(logSB.ToString());
            }
            catch (System.Exception e)
            {
                var msg = ShowTreeMessage(e);
                Console.WriteLine("寫入紀錄發生錯誤：" + msg);
                logSB.AppendLine("ERROR：" + msg);

                LogFile.getInstance().writeContent(logSB.ToString(), "ERROR");


                Task.Factory.StartNew(
                       () => {
                           EAIError(Endpoint, InputData, e.Message, "");
                           EAIError(Endpoint, InputData, e.Message, "","","","ELK");
                        }
                );
               
                
            }

        }
    }
}