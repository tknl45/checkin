using System;
using System.Collections.Generic;
using CheckIn.Models;
using CheckIn.Utility;
using MongoDB.Driver;

namespace CheckIn.DAO
{
    public class CheckInDAO
    {
        static CheckInDAO instance;
        private static IMongoDatabase _db;
        private static Object _lock_obj = new Object();
        public static CheckInDAO getInstance()
        {
            if (instance == null)
            {
                lock(_lock_obj){
                    instance = new CheckInDAO();
                }
                
            }

            return instance;
        }

        public CheckInDAO()
        {
            _db = MongoUtil.getInstance().getDB();
        }

        
        /// <summary>
        /// 確認是否簽到
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <param name="userId">使用者id</param>
        /// <param name="today">時間</param>
        /// <returns></returns>
        internal bool isCheckIn(string actId, string userId, string today)
        {
            var builder = Builders<CheckInData>.Filter;

            var querys = new List<FilterDefinition<CheckInData>>();			
			querys.Add(builder.Eq(checkInData => checkInData.userId, userId));
            querys.Add(builder.Eq(checkInData => checkInData.checkIn_data, today));

            var res = builder.And(querys);
			var list = _db.GetCollection<CheckInData>($"CheckIn_{actId}").Find(res).ToList<CheckInData>();
            if(list.Count == 0){
                return false;
            }else{
                return true;
            }	
        }

        /// <summary>
        /// 簽到
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <param name="userId">使用者id</param>
        /// <param name="today">時間</param>
        /// <returns></returns>
        internal void CheckIn(string actId, string userId, string today)
        {
            var data = new CheckInData();
            data.userId = userId;
            data.checkIn_data = today;
            data.check_or_not = true;

            _db.GetCollection<CheckInData>($"CheckIn_{actId}").InsertOne(data);
        }


        /// <summary>
        /// 查詢簽到資訊
        /// </summary>
        /// <param name="actId">活動id</param>
        /// <param name="userId">使用者id</param>
        /// <param name="today">時間</param>
        /// <returns></returns>
        internal List<CheckInData> queryCheckIn(string actId, string userId)
        {
            var builder = Builders<CheckInData>.Filter;

            var querys = new List<FilterDefinition<CheckInData>>();			
			querys.Add(builder.Eq(checkInData => checkInData.userId, userId));
            var res = builder.And(querys);
			return _db.GetCollection<CheckInData>($"CheckIn_{actId}").Find(res).ToList<CheckInData>();
        }
            
    }
}