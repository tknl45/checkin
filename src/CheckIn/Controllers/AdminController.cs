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
    [Route("Admin")]
    public class AdminController : BaseController
    {
       
       

       


        /// <summary>
        /// 增加活動
        /// </summary>
        /// <param name="act"></param>
        [Route("Act/Add")]
        [HttpPost]
        public void Act_Add([FromBody]CheckInAct act)
        {
            if(act == null)
            {
                this.Fail("查無資料，請再確認");
                return;
            }
            //檢驗參數，有問題會錯誤
            act = Validact(act);
            if(act == null){
                return;
            }

            //確認活動是否重複
            if(ActDAO.getInstance().getAct(act.actId) != null)
            {
                Fail($"活動id:{act.actId}已經設定");
                return;
            }

            act.created_at = DateTime.Now;
            ActDAO.getInstance().createAct(act);

            this.Ok("建立活動成功",act);
            
        }

        /// <summary>
        /// 檢查act正確性
        /// </summary>
        /// <returns></returns>
        private CheckInAct Validact(CheckInAct act)
        {

            if (string.IsNullOrWhiteSpace(act.actId))
            {
                act.actId = Guid.NewGuid().ToString();
            }

            if(act.startTime == null){
                Fail("startTime時間沒有設定");
                return null;
            }
            if(act.endTime == null){
                Fail("endTime時間沒有設定");
                return null;
            }

            if (act.startTime > act.endTime)
            {
               Fail("startTime時間大於endTime");
               return null;
            }

            if (act.endTime < DateTime.Now)
            {
                Fail("現在時間大於endTime");
                return null;
            }

            if (act.site.IndexOf("http://")!=0 && act.site.IndexOf("https://") != 0)
            {
                Fail("site 值需要http://開頭");
                return null;
            }



            return act;
        }
        
        /// <summary>
        /// 刪除活動
        /// </summary>
        /// <param name="actId">活動id</param>
        [Route("Act/Remote")]
        [HttpPost]
        public void Act_Remote([FromHeader]String actId)
        {
            bool isOK = ActDAO.getInstance().removeAct(actId);
            if(isOK){
                this.Ok("刪除活動成功");
            }else{
                this.Fail("刪除活動失敗");
            }
            
        }

        /// <summary>
        /// 更新活動
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <param name="act">活動</param>
        [Route("Act/Update")]
        [HttpPost]
        public void Act_Remote([FromHeader]String actId, [FromBody]CheckInAct act)
        {
            bool isOK = ActDAO.getInstance().updateAct(actId, act);
            if(isOK){
                this.Ok("更新活動成功");
            }else{
                this.Fail("更新活動失敗");
            }
            
        }


        /// <summary>
        /// 查詢活動
        /// </summary>
        /// <param name="actId">活動id</param>
        [Route("Act/{actId}")]
        [HttpGet]
        public void Act_Query([FromRoute]String actId)
        {
            var result = ActDAO.getInstance().getAct(actId);
            if(result == null){
                this.Fail("查無資料");
            }else{
                this.Ok("查詢成功",result);
            }
            
        }

        
    }


}
