using CheckIn.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CheckIn.Filters
{
    class ResultFilter : IAsyncResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            var controller = (BaseController)context.Controller;

            var result = new
            {
                Result = controller.Result,                

                // 取得由 API 返回的處理訊息
                Message = controller.Message,

                // 取得由 API 返回的資料
                //result.Data = actionExecutedContext.ActionContext.Response.Content.ReadAsAsync<object>().Result;
                Data = controller.Data,

            };

           
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            //throw new NotImplementedException();
            context.HttpContext.Response.WriteAsync($"{GetType().Name} in. \r\n");
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            await next();

            //處理成功回傳值
            var controller = (BaseController)context.Controller;

            var result = new
            {
                // 取得由 API 返回的 Result Code               
                Result = controller.Result,

                // 取得由 API 返回的處理訊息
                Message = controller.Message,

                // 取得由 API 返回的資料                
                Data = controller.Data,
            };


            var json = JsonConvert.SerializeObject(result, Formatting.None);

            var bytes = Encoding.UTF8.GetBytes(json);
            int length = bytes.Length;

            var response = context.HttpContext.Response;
            //this must be set before start writing into the stream
            response.ContentLength = length;
            response.ContentType = "application/json";

            await response.Body.WriteAsync(bytes, 0, length);


            

        }
    }
}