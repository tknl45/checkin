using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace CheckIn.Utility
{
public class HttpRequestUtility
    {
        private HttpClient _client;
        private static HttpRequestUtility _instance;
        private static Object obj_lock = new Object();
        public static HttpRequestUtility getInstance(HttpClientHandler handler=null)
        {

            lock (obj_lock)
            {
                _instance = new HttpRequestUtility();
                _instance._client = new HttpClient(); //建立http client 物件   

                if(handler != null)
                {
                    _instance._client = new HttpClient(handler); //建立http client 物件   
                }
                _instance._client.Timeout = TimeSpan.FromSeconds(60);
            }


            return _instance;
        }



        /// <summary>
        /// 以Post 方式呼叫API
        /// </summary>
        /// <param name="api_url"></param>
        /// <param name="queryString"></param>
        /// <param name="contentType"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<String> postRequest(String api_url, String queryString, Dictionary<String, String> headers = null, String contentType = "application/x-www-form-urlencoded")
        {
            var httpContent = new StringContent(queryString,
                                    Encoding.UTF8,
                                    contentType);
           
            System.Console.WriteLine("[POST]" + api_url);

            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, api_url)
            {
                Content = httpContent
            };

            if (headers != null)
            {
                System.Console.WriteLine("headers:" + JsonConvert.SerializeObject(headers));

                foreach (var key in headers.Keys)
                {
                    System.Console.WriteLine($"header[{key}]" + headers[key]);
                    postRequest.Headers.Add(key, headers[key]);
                }
            } 

            DateTime StartTime = DateTime.Now;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _client.SendAsync(postRequest).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            System.Console.WriteLine($"{elapsedMs}ms");
            System.Console.WriteLine(responseString);
            System.Console.WriteLine($"[{api_url}][{elapsedMs}ms]" + responseString);

            // log
            string Endpoint = api_url;
            String Query_string = "";
            if (Endpoint.IndexOf("?") > -1)
            {
                Endpoint = Endpoint.Substring(0, Endpoint.IndexOf("?"));
                Query_string = api_url.Substring(api_url.IndexOf("?") + 1);
            }
            string Header_string = JsonConvert.SerializeObject(headers);
            string Body = queryString;
            string Method = "POST";
            


            try
            {
                System.Console.WriteLine("StatusCode:" + response.StatusCode);
                response.EnsureSuccessStatusCode();

                //Log
                string Status = response.StatusCode + "";
                string ReturnMessage = "";
                DateTime EndTime = DateTime.Now;
                string OutputData = responseString;
                await Task.Factory.StartNew(
                    () => LogService.getInstance().RestAPI_OUTPUTDATA(Endpoint, Query_string, Header_string, Body, Method, Status, ReturnMessage, StartTime, EndTime, OutputData)
                );

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);


                //Log
                string Status = response.StatusCode + "";
                string ReturnMessage = "";
                DateTime EndTime = DateTime.Now;
                string OutputData = "";
                await Task.Factory.StartNew(
                    () => LogService.getInstance().RestAPI_OUTPUTDATA(Endpoint, Query_string, Header_string, Body, Method, Status, ReturnMessage, StartTime, EndTime, OutputData)
                );

                return "{\"result\":\"-1\",\"msg\":\"" + e.Message + "\"}";
            }


            

            

            return responseString;
        }

        internal async Task<String> getRequest(string api_url, Dictionary<String, String> headers)
        {
            System.Console.WriteLine("[GET]" + api_url);
            System.Console.WriteLine("[GET]" + api_url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, api_url);
            foreach (var key in headers.Keys)
            {

                System.Console.WriteLine($"header[{key}]" + headers[key]);
                request.Headers.Add(key, headers[key]);
            }


            // log
            string Endpoint = api_url;
            String Query_string = "";
            if (Endpoint.IndexOf("?") > -1)
            {
                Endpoint = Endpoint.Substring(0, Endpoint.IndexOf("?"));
                Query_string = api_url.Substring(api_url.IndexOf("?") + 1);
            }
            string Header_string = JsonConvert.SerializeObject(headers);
            string Body = "";
            string Method = "GET";




            DateTime StartTime = DateTime.Now;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _client.SendAsync(request).ConfigureAwait(false);



            var responseString = await response.Content.ReadAsStringAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            try
            {
                System.Console.WriteLine(responseString);
                System.Console.WriteLine("StatusCode:" + response.StatusCode);
                System.Console.WriteLine("StatusCode:" + response.StatusCode);
                response.EnsureSuccessStatusCode();

                //Log
                string Status = response.StatusCode + "";
                string ReturnMessage = "";
                DateTime EndTime = DateTime.Now;
                string OutputData = responseString;
                await Task.Factory.StartNew(
                    () => LogService.getInstance().RestAPI_OUTPUTDATA(Endpoint, Query_string, Header_string, Body, Method, Status, ReturnMessage, StartTime, EndTime, OutputData)
                );
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);


                //Log
                string Status = response.StatusCode + "";
                string ReturnMessage = "";
                DateTime EndTime = DateTime.Now;
                string OutputData = responseString;
                await Task.Factory.StartNew(
                    () => LogService.getInstance().RestAPI_OUTPUTDATA(Endpoint, Query_string, Header_string, Body, Method, Status, ReturnMessage, StartTime, EndTime, OutputData)
                );


                return "{\"result\":\"-1\",\"msg\":\"" + e.Message + "\"}";
            }

            System.Console.WriteLine($"{elapsedMs}ms");
            System.Console.WriteLine(responseString);
            System.Console.WriteLine($"[{api_url}][{elapsedMs}ms]" + responseString);



            return responseString;
        }

        internal async Task<String> putRequest(string api_url, Dictionary<String, String> headers, NameValueCollection querys)
        {
            System.Console.WriteLine("[PUT]" + api_url);
            System.Console.WriteLine("[PUT]" + api_url);
            String contentType = "application/x-www-form-urlencoded";
            string queryString = querys.ToString();
            var httpContent = new StringContent(queryString,
                                    Encoding.UTF8,
                                    contentType);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, api_url)
            {
                Content = httpContent
            };


            foreach (var key in headers.Keys)
            {
                System.Console.WriteLine($"header[{key}]" + headers[key]);
                request.Headers.Add(key, headers[key]);
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _client.SendAsync(request).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            System.Console.WriteLine($"{elapsedMs}ms");
            System.Console.WriteLine(responseString);
            System.Console.WriteLine($"[{api_url}][{elapsedMs}ms]" + responseString);
            try
            {
                System.Console.WriteLine("StatusCode:" + response.StatusCode);
                response.EnsureSuccessStatusCode();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.Message);
                return "{\"result\":\"-1\",\"msg\":\"" + e.Message + "\"}";
            }

            return responseString;
        }
        /// <summary>
        /// 以 PUT 方式呼叫API
        /// </summary>
        /// <param name="api_url"></param>
        /// <param name="queryString"></param>
        /// <param name="contentType"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<String> putRequest(String api_url, String queryString, Dictionary<String, String> headers = null, String contentType = "application/x-www-form-urlencoded")
        {
            var httpContent = new StringContent(queryString,
                                    Encoding.UTF8,
                                    contentType);

            System.Console.WriteLine("[PUT]" + api_url);

            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Put, api_url)
            {
                Content = httpContent
            };

            if (headers != null)
            {
                System.Console.WriteLine("headers:" + JsonConvert.SerializeObject(headers));

                foreach (var key in headers.Keys)
                {
                    System.Console.WriteLine($"header[{key}]" + headers[key]);
                    postRequest.Headers.Add(key, headers[key]);
                }
            }

            DateTime StartTime = DateTime.Now;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _client.SendAsync(postRequest).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            System.Console.WriteLine($"{elapsedMs}ms");
            System.Console.WriteLine(responseString);
            System.Console.WriteLine($"[{api_url}][{elapsedMs}ms]" + responseString);

            // log
            string Endpoint = api_url;
            String Query_string = "";
            if (Endpoint.IndexOf("?") > -1)
            {
                Endpoint = Endpoint.Substring(0, Endpoint.IndexOf("?"));
                Query_string = api_url.Substring(api_url.IndexOf("?") + 1);
            }
            string Header_string = JsonConvert.SerializeObject(headers);
            string Body = queryString;
            string Method = "PUT";



            try
            {
                System.Console.WriteLine("StatusCode:" + response.StatusCode);
                response.EnsureSuccessStatusCode();

                //Log
                string Status = response.StatusCode + "";
                string ReturnMessage = "";
                DateTime EndTime = DateTime.Now;
                string OutputData = responseString;
                await Task.Factory.StartNew(
                    () => LogService.getInstance().RestAPI_OUTPUTDATA(Endpoint, Query_string, Header_string, Body, Method, Status, ReturnMessage, StartTime, EndTime, OutputData)
                );

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);


                //Log
                string Status = response.StatusCode + "";
                string ReturnMessage = "";
                DateTime EndTime = DateTime.Now;
                string OutputData = "";
                await Task.Factory.StartNew(
                    () => LogService.getInstance().RestAPI_OUTPUTDATA(Endpoint, Query_string, Header_string, Body, Method, Status, ReturnMessage, StartTime, EndTime, OutputData)
                );

                return "{\"result\":\"-1\",\"msg\":\"" + e.Message + "\"}";
            }






            return responseString;
        }


        /// <summary>
        /// 以 DELETE 方式呼叫API
        /// </summary>
        /// <param name="api_url"></param>
        /// <param name="queryString"></param>
        /// <param name="contentType"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<String> deleteRequest(String api_url, String queryString, Dictionary<String, String> headers = null, String contentType = "application/x-www-form-urlencoded")
        {
            var httpContent = new StringContent(queryString,
                                    Encoding.UTF8,
                                    contentType);

            System.Console.WriteLine("[DELETE]" + api_url);

            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Delete, api_url)
            {
                Content = httpContent
            };

            if (headers != null)
            {
                System.Console.WriteLine("headers:" + JsonConvert.SerializeObject(headers));

                foreach (var key in headers.Keys)
                {
                    System.Console.WriteLine($"header[{key}]" + headers[key]);
                    postRequest.Headers.Add(key, headers[key]);
                }
            }

            DateTime StartTime = DateTime.Now;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _client.SendAsync(postRequest).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            System.Console.WriteLine($"{elapsedMs}ms");
            System.Console.WriteLine(responseString);
            System.Console.WriteLine($"[{api_url}][{elapsedMs}ms]" + responseString);

            // log
            string Endpoint = api_url;
            String Query_string = "";
            if (Endpoint.IndexOf("?") > -1)
            {
                Endpoint = Endpoint.Substring(0, Endpoint.IndexOf("?"));
                Query_string = api_url.Substring(api_url.IndexOf("?") + 1);
            }
            string Header_string = JsonConvert.SerializeObject(headers);
            string Body = queryString;
            string Method = "DELETE";



            try
            {
                System.Console.WriteLine("StatusCode:" + response.StatusCode);
                response.EnsureSuccessStatusCode();

                //Log
                string Status = response.StatusCode + "";
                string ReturnMessage = "";
                DateTime EndTime = DateTime.Now;
                string OutputData = responseString;
                await Task.Factory.StartNew(
                    () => LogService.getInstance().RestAPI_OUTPUTDATA(Endpoint, Query_string, Header_string, Body, Method, Status, ReturnMessage, StartTime, EndTime, OutputData)
                );

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);


                //Log
                string Status = response.StatusCode + "";
                string ReturnMessage = "";
                DateTime EndTime = DateTime.Now;
                string OutputData = "";
                await Task.Factory.StartNew(
                    () => LogService.getInstance().RestAPI_OUTPUTDATA(Endpoint, Query_string, Header_string, Body, Method, Status, ReturnMessage, StartTime, EndTime, OutputData)
                );

                return "{\"result\":\"-1\",\"msg\":\"" + e.Message + "\"}";
            }






            return responseString;
        }




    }
}