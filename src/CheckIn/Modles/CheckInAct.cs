using System;

namespace CheckIn.Modles
{
    /// <summary>
    /// 簽到活動
    /// </summary>
    public class CheckInAct
    {
        
        public String Id { get; set; }
        public String Name { get; set; }

        public String Description { get; set; }

        public String Site { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        public DateTime Created_at { get; set; }



       
    }
}
