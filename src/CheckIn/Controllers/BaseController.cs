using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheckIn.Controllers
{
public class BaseController : Controller
    {
        public int Result { get; set; } = 1;
        public object Data { get; set; } = null;
        public string Message { get; set; } = "成功";

        /// <summary>
        /// 紀錄呼叫data，用在filter時寫入log_XXX_YYYY的資料表中
        /// </summary>
        public Dictionary<string, object> LogData { get; set; } = null;

        /// <summary>
        /// 訊息紀錄
        /// </summary>
        public string LogDetailMessage { get; set; } = "";


        public BaseController()
        {
           
        }

        /// <summary>
        /// 成功時回傳值
        /// </summary>
        /// <param name="data">回傳物件</param>
        /// <param name="msg">訊息</param>
        /// <returns></returns>
        protected void Ok(string msg= "成功",Object data = null )
        {
            Result = 1;
            Message = msg;
            Data =  data;
        }


        /// <summary>
        /// 失敗時回傳值
        /// </summary>
        /// <param name="msg">訊息</param>
        /// <param name="data">回傳物件</param>
        /// <returns></returns>
        protected void Fail(string msg, Object data = null)
        {
            Result = -1;
            Message = msg;
            Data =  data;
        }
        
    }
}