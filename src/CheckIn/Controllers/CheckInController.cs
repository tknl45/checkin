using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CheckIn.DAO;
using CheckIn.Models;
using CheckIn.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CheckIn.Controllers
{
    [ApiController]
    [Route("CheckIn")]
    public class CheckInController : BaseController
    {
       
        /// <summary>
        /// 輸入活動id與使用者id做本日簽到
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <param name="userId">使用者id</param>
        /// <returns></returns>
        [Route("checkIn")]
        [HttpGet]
        public void checkIn(string actId, string userId)
        {
            var act = ActDAO.getInstance().getAct(actId);

            if(act == null){
                this.Fail("無此活動");
                return;
            }


            var today = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd"); //轉成UTC+8
            // Check if the user has checked in
            var isCheckIn = CheckInDAO.getInstance().isCheckIn(actId, userId, today);

            if(!isCheckIn){
                //簽到
                CheckInDAO.getInstance().CheckIn(actId, userId, today);
                this.Message = "今日簽到完畢";
            }else{
                this.Message = "今日已簽到";
            }
            
            showCheckIn(actId, userId);
        }


        /// <summary>
        /// 輸入活動id與使用者id顯示簽到列表
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <param name="userId">使用者id</param>
        /// <returns></returns>
        [Route("showCheckIn")]
        [HttpGet]
        public void showCheckIn(string actId, string userId)
        {
            // Get how many days the user has checked in.
            var list = CheckInDAO.getInstance().queryCheckIn(actId, userId);


            // Response the successfully checked in message.        
            this.Data = list;
        }


        /// <summary>
        /// 顯示目前開啟中在的活動資訊列表
        /// </summary>
        /// <returns></returns>
        [Route("showActs")]
        [HttpGet]
        public void showActs(DateTime? day)
        {
            var chooseDay = DateTime.Now;
            if(day != null){
                chooseDay = (DateTime)day;
            }

            this.Data = ActDAO.getInstance().showActs(chooseDay);
        }

     






        /// <summary>
        /// 顯示今日是否已簽到
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <param name="userId">使用者id</param>
        /// <returns></returns>
        [Route("ifUserCheckInToday")]
        [HttpGet]
        public void ifUserCheckInToday(string actId, string userId)
        {
            var today = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd"); //轉成UTC+8
            // Check if the user has checked in
            var isCheckIn = CheckInDAO.getInstance().isCheckIn(actId, userId, today);

            if(isCheckIn){
                this.Ok("已簽到", isCheckIn);
            }else{
                this.Ok("未簽到", isCheckIn);
            }

        }

         /// <summary>
        /// 輸入活動id與使用者id完整活動簽到成功失敗列表
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <param name="userId">使用者id</param>
        /// <returns></returns>
        [Route("getDateAndCheckOrNotInformation")]
        [HttpGet]
        public void getDateAndCheckOrNotInformation(string actId, string userId){
            
            var list = new List<CheckInDataBase>();
            var howManyDaysTheUserHasCheckedInConsecutivelyUntilToday = 0;
            var act = ActDAO.getInstance().getAct(actId);

            if(act == null){
                this.Fail("無此活動");
                return;
            }
            var ok_list = CheckInDAO.getInstance().queryCheckIn(actId, userId);

            var pointDate = (DateTime)act.startTime;
            while(pointDate < act.endTime){
                var data = new CheckInDataBase();
                var day = pointDate.ToUniversalTime().AddHours(8).ToString("yyyy-MM-dd"); //轉成UTC+8
                var findArr = ok_list.Where<CheckInData>(e => e.checkIn_data == day);
                if(findArr.Count() != 0){
                    data = findArr.ToList<CheckInData>()[0];
                    howManyDaysTheUserHasCheckedInConsecutivelyUntilToday++;
                }else{
                    data.checkIn_data = day;     
                    data.check_or_not = false;               
                }

                list.Add(data);

                pointDate = pointDate.AddDays(1);
            }

            var result = new Dictionary<String, Object>();
            result["howManyDaysTheUserHasCheckedInConsecutivelyUntilToday"] = howManyDaysTheUserHasCheckedInConsecutivelyUntilToday;
            result["detail"] = list;

            Ok("查詢成功",result);


        }

        

        
    }


}
