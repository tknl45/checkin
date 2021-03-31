using System;

namespace CheckIn.Models
{
    /// <summary>
    /// Mongo 查詢
    /// </summary>
    public class MongoQuery
    {
        /// <summary>
        /// 資料表，常用如下：
        /// 
        /// LOG_RESTAPI_OUTPUTDATA / LOG_Function / LOG_EAIError / LOG_EAI_REST_OUTPUT
        /// 
        /// </summary>
        /// <value></value>
        public String collection { get; set; } =  "LOG_EAIError";

        /// <summary>
        /// query 預設查詢{}
        /// </summary>
        /// <value></value>
        public String query { get; set; } = "{}";

        /// <summary>
        /// 排序， 預設{_id:-1}
        /// </summary>
        /// <value></value>
        public String sort { get; set; } = "{_id:-1}";

        /// <summary>
        /// 筆數 20
        /// </summary>
        /// <value></value>
        public int limit  { get; set; } =  20;

        /// <summary>
        /// 頁碼 從1開始
        /// </summary>
        /// <value></value>
        public int page  { get; set; } =  1 ;

        /// <summary>
        /// 顯示語法，預設 {}
        /// </summary>
        /// <value></value>
        public  String project { get; set; } = "{}";
        
    }
}