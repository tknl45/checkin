using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CheckIn.Modles;
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
        /// checkIn
        /// </summary>
        /// <param name="act_id">活動id</param>
        /// <param name="user_id">使用者id</param>
        /// <returns></returns>
        [Route("checkIn")]
        [HttpGet]
        public void checkIn(string act_id, string user_id)
        {
            // Check if the user has checked in

            // Get how many days the user has checked in.
            
            // Response the successfully checked in message.
        }


        /// <summary>
        /// showCheckIn
        /// </summary>
        /// <param name="act_id">活動id</param>
        /// <param name="user_id">使用者id</param>
        /// <returns></returns>
        [Route("showCheckIn")]
        [HttpGet]
        public void showCheckIn(string act_id, string user_id)
        {
            // Get the Day and check_or_not information from check_ins table with designated user_id.
            
            // Get how many days the user has checked in.
            
            // Response the successfully checked in message.
        }


        /// <summary>
        /// showActs
        /// </summary>
        /// <returns></returns>
        [Route("showActs")]
        [HttpGet]
        public void showActs()
        {
            // Get the Day and check_or_not information from check_ins table with designated user_id.
            
            // Get how many days the user has checked in.
            
            // Response the successfully checked in message.
        }






        /// <summary>
        /// ifUserCheckInToday
        /// </summary>
        /// <param name="act_id">活動id</param>
        /// <param name="user_id">使用者id</param>
        /// <returns></returns>
        [Route("ifUserCheckInToday")]
        [HttpGet]
        public void ifUserCheckInToday(string act_id, string user_id)
        {

        }

         /// <summary>
        /// getDateAndCheckOrNotInformation
        /// </summary>
        /// <param name="act_id">活動id</param>
        /// <param name="user_id">使用者id</param>
        /// <returns></returns>
        [Route("getDateAndCheckOrNotInformation")]
        [HttpGet]
        public void getDateAndCheckOrNotInformation(string act_id, string user_id){
            var howManyDaysTheUserHasCheckedInConsecutivelyUntilToday = 0;

        }

        

        
    }


}
