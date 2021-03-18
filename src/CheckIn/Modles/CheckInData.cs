using System;

namespace CheckIn.Modles
{
    /// <summary>
    /// 活動簽到資料
    /// </summary>
    public class ActCheckInData
    {
        public String Id { get; set; }

        /// <summary>
        /// 使用者id
        /// </summary>
        /// <value></value>
        public String UserId { get; set; }

        /// <summary>
        /// 簽到對象ex. yyyy-mm-dd
        /// </summary>
        /// <value></value>
        public String CheckIn_data { get; set; }

        /// <summary>
        /// 是否簽到
        /// </summary>
        /// <value></value>
        public bool Check_or_not { get; set; } = false;


        /// <summary>
        /// 建立時間
        /// </summary>
        /// <value></value>
        public DateTime Created_at { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        /// <value></value>
        public DateTime Update_at { get; set; }




       
    }
}
