using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckIn.Filters
{
 
    public class ExceptionFilter : IAsyncExceptionFilter
    {

        private readonly ILogger<ExceptionFilter> logger;

        /// <summary>
        /// 起始傳入logger
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="_logger"></param>
        public ExceptionFilter(ILoggerFactory loggerFactory, ILogger<ExceptionFilter> _logger )
        {
             logger = _logger;
        }

        /// <summary>
        /// 自動繼承
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnExceptionAsync(ExceptionContext context)
        {


            var ex = context.Exception;
            System.Console.WriteLine(ex.ToString());
            

            var message = ex.Message;
            var error_stack = ex.StackTrace;

            while(ex.InnerException != null){
                ex = ex.InnerException;
                message += ">>>>>"+ex.Message;
                error_stack += ">>>>>"+ex.StackTrace;
            }


            // 構建錯誤信息對象
            var dic = new Dictionary<string, object>
            {
                ["Result"] = -1,
                ["Message"] = message,
                ["Error_stack"] = error_stack
            };
            // 設置結果轉為JSON
            context.Result = new JsonResult(dic);
            context.ExceptionHandled = true;

            
            try
            {
                Task.Factory.StartNew(
                    () => {
                        Utility.LogService.getInstance().EAIError("ERROR", ex.ToString(), ex.Message, "");                        
                    }
                );
            }
            finally
            {
                
            }
            


            return Task.CompletedTask;
        }


    }
}