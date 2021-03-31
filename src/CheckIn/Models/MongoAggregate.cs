using System;

namespace CheckIn.Models
{
    /// <summary>
    /// Mongo 查詢
    /// </summary>
    public class MongoAggregate
    {
        /// <summary>
        /// 資料表
        /// </summary>
        /// <value></value>
        public String collection { get; set; } =  "LOG_EAIError";

        /// <summary>
        /// pipeline 查詢
        /// </summary>
        /// <value></value>
        [System.ComponentModel.DataAnnotations.Required]
        public String pipeline { get; set; } = "[]";

    }
}