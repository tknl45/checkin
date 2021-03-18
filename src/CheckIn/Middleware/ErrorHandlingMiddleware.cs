namespace CheckIn.Middleware
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var statusCode = context.Response.StatusCode;
                if (ex is ArgumentException)
                {
                    statusCode = 200;
                }
                await HandleExceptionAsync2(context, statusCode, ex);
            }
            finally
            {
                var statusCode = context.Response.StatusCode;
                var msg = "";
                //Console.WriteLine("statusCode2:"+statusCode);
                if (statusCode == 401)
                {
                    msg = "未授權";
                }
                else if (statusCode == 404)
                {
                    msg = "未找到服務";
                }
                else if (statusCode == 502)
                {
                    msg = "請求錯誤";
                }else if (statusCode == 304)
                {
                    msg = "";
                }
                else if (statusCode != 200)
                {
                    msg = "未知錯誤";
                    //Console.WriteLine("statusCode != 200 /"+statusCode);
                    //Console.WriteLine("msg:"+msg);
                }
                

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    //Console.WriteLine("msg:"+msg);
                    //Console.WriteLine("statusCode:"+statusCode);
                    await HandleExceptionAsync(context, statusCode, msg);
                }
            }
        }


        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string msg)
        {
            var data = new { code = statusCode.ToString(), is_success = false, msg = msg };
            var result = JsonConvert.SerializeObject(new { data = data });
            context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(result);
        }


        private static Task HandleExceptionAsync2(HttpContext context, int statusCode, Exception ex)
        {
            // 構建錯誤信息對象

            var data = new Dictionary<string, object>
            {
                ["Result"] = -1,
                ["Message"] = ex.Message,
                ["Error_stack"] = ex.StackTrace
            };

            var result = JsonConvert.SerializeObject(data);
            context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(result);
        }


    }

    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}