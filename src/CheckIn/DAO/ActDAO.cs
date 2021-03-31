using System;
using System.Collections.Generic;
using System.ComponentModel;
using CheckIn.Models;
using CheckIn.Utility;
using MongoDB.Driver;

namespace CheckIn.DAO
{
    public class ActDAO
    {
        static ActDAO instance;
        private static IMongoDatabase _db;
        private static Object _lock_obj = new Object();
        public static ActDAO getInstance()
        {
            if (instance == null)
            {
                lock(_lock_obj){
                    instance = new ActDAO();
                }
                
            }

            return instance;
        }

        public ActDAO()
        {
            _db = MongoUtil.getInstance().getDB();
        }

        /// <summary>
        /// 取得單一活動資料
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <returns></returns>
        public CheckInAct getAct(string actId){            
            var res = Builders<CheckInAct>.Filter.Eq(act => act.actId, actId);
			var list = _db.GetCollection<CheckInAct>("CheckInAct").Find(res).ToList<CheckInAct>();
            if(list.Count == 0){
                return null;
            }else{
                return list[0];
            }			
        }

        /// <summary>
        /// 刪除活動資料
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <returns></returns>
        public bool removeAct(string actId){            
            var res = Builders<CheckInAct>.Filter.Eq(act => act.actId, actId);	
			var result = _db.GetCollection<CheckInAct>("CheckInAct").DeleteMany(res);
			return result.DeletedCount != 0;
        }

        /// <summary>
        /// 建立活動資料
        /// </summary>
        /// <param name="act">活動</param>
        /// <returns></returns>
        public void createAct(CheckInAct act){
            _db.GetCollection<CheckInAct>($"CheckInAct").InsertOne(act);
        }

        /// <summary>
        /// 更新活動資料
        /// </summary>
        /// <param name="new_act">活動</param>
        /// <param name="actId">活動id</param>
        /// <returns></returns>
        public bool updateAct(String actId, CheckInAct new_act){


			

			var updList = new List<UpdateDefinition<CheckInAct>>();
		
			if( actId != new_act.actId){
				var upd = Builders<CheckInAct>.Update.Set(act => act.actId, new_act.actId);
				updList.Add(upd);
			}

			
			if(!String.IsNullOrEmpty(new_act.name)){
				var upd = Builders<CheckInAct>.Update.Set(n => n.name, new_act.name);
				updList.Add(upd);
			}
			if(!String.IsNullOrEmpty(new_act.description)){
				var upd = Builders<CheckInAct>.Update.Set(n => n.description, new_act.description);
				updList.Add(upd);
			}
			if(!String.IsNullOrEmpty(new_act.site)){
				var upd = Builders<CheckInAct>.Update.Set(n => n.site, new_act.site);
				updList.Add(upd);
			}
			if(new_act.startTime != null){
				var upd = Builders<CheckInAct>.Update.Set(n => n.startTime, new_act.startTime);
				updList.Add(upd);
			}


			if(new_act.endTime != null){
				var upd = Builders<CheckInAct>.Update.Set(n => n.endTime, new_act.endTime);
				updList.Add(upd);
			}
			
			
			
			var finalUpd = Builders<CheckInAct>.Update.Combine(updList);

			
			var res = Builders<CheckInAct>.Filter.Eq(act => act.actId, actId);	
			var result = _db.GetCollection<CheckInAct>("CheckInAct").UpdateOne(res,finalUpd);

			return result.ModifiedCount != 0;
        }

        /// <summary>
        /// 顯示目前有的活動清單
        /// </summary>
        /// <returns></returns>
        internal List<CheckInAct> showActs(DateTime day)
        {
            var builder = Builders<CheckInAct>.Filter;
            var list = new BindingList<FilterDefinition<CheckInAct>>();
			list.Add(builder.Gte(checkInAct => checkInAct.endTime, day));
			list.Add(builder.Lte(checkInAct => checkInAct.startTime, day));
            var res = builder.And(list);


			return _db.GetCollection<CheckInAct>("CheckInAct").Find(res).ToList<CheckInAct>();
            
        }
    }
}