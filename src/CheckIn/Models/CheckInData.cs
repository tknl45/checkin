using System;
using MongoDB.Bson;

namespace CheckIn.Models
{
    /// <summary>
    /// 活動簽到資料
    /// </summary>
    public class CheckInData : CheckInDataBase
    {
      

        /// <summary>
        /// 使用者id
        /// </summary>
        /// <value></value>
        public String userId { get; set; }

     

        /// <summary>
        /// 建立時間
        /// </summary>
        /// <value></value>
        public DateTime created_at { get; set; } = DateTime.Now;

       
    }
}
