using System;
using MongoDB.Bson;

namespace CheckIn.Models
{
    /// <summary>
    /// 活動簽到資料
    /// </summary>
    public class CheckInData
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public ObjectId Id { get; set; } = new ObjectId();

        /// <summary>
        /// 使用者id
        /// </summary>
        /// <value></value>
        public String userId { get; set; }

        /// <summary>
        /// 簽到對象ex. yyyy-mm-dd
        /// </summary>
        /// <value></value>
        public String checkIn_data { get; set; }

        /// <summary>
        /// 是否簽到
        /// </summary>
        /// <value></value>
        public bool check_or_not { get; set; } = false;


        /// <summary>
        /// 建立時間
        /// </summary>
        /// <value></value>
        public DateTime created_at { get; set; } = DateTime.Now;

       
    }
}
