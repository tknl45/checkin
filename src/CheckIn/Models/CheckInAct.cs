using System;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CheckIn.Models
{
    /// <summary>
    /// 簽到活動
    /// </summary>
    public class CheckInAct
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public ObjectId Id { get; set; } = new ObjectId();
        public String actId { get; set; }
        public String name { get; set; } = null;

        public String description { get; set; } = null;

        public String site { get; set; } = "https://not-input";

        public DateTime? startTime { get; set; } = null;

        public DateTime? endTime { get; set; } = null;

        public DateTime created_at { get; set; } = new DateTime();



       
    }
}
