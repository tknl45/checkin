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
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace CheckIn.Controllers
{
    [ApiController]
    [Route("Monitor")]
    public class MongoController : BaseController
    {

      

        /// <summary>
        /// 列出Collection
        /// </summary>
        [Route("ListCollection")]
        [HttpPost]
        public void ListCollection()
        {
            var result = MongoUtil.getInstance().ListCollection();
            this.Data = result;
        }

        /// <summary>
        /// 刪除Collection
        /// </summary>
        [Route("DropCollection")]
        [HttpPost]
        public void DropCollection([FromForm]string collection)
        {
            MongoUtil.getInstance().DropCollection(collection);
        }


        /// <summary>
        /// 查詢資料
        /// </summary>
        [Route("QueryCollection")]
        [HttpPost]
        public void QueryCollection([FromForm] MongoQuery mongoQuery, [FromQuery]int showStatus = 0)
        {
            switch (showStatus)
            {
                case 0:
                    var result = MongoUtil.getInstance().QueryCollection(mongoQuery.collection, mongoQuery.query, mongoQuery.project, mongoQuery.page, mongoQuery.limit, mongoQuery.sort );
                    this.Data = new Dictionary<String, Object>(){
                        {"showStatus", showStatus},
                        {"query", mongoQuery},
                        {"result", result}
                    };
                    break;
                case 1:
                    var count = MongoUtil.getInstance().QueryCollectionCount(mongoQuery.collection, mongoQuery.query, mongoQuery.project, mongoQuery.page, mongoQuery.limit, mongoQuery.sort );
                    Dictionary<string, long> pageData =  new Dictionary<string, long>();
                    pageData["total"] = count;
                    pageData["now"] = mongoQuery.page;
                    pageData["row"] = mongoQuery.limit;
                    this.Data = new Dictionary<String, Object>(){
                        {"showStatus", showStatus},
                        {"query", mongoQuery},
                        {"pageData", pageData}
                    };
                    break;
                case 2:
                    var result1 = MongoUtil.getInstance().QueryCollection(mongoQuery.collection, mongoQuery.query, mongoQuery.project, mongoQuery.page, mongoQuery.limit, mongoQuery.sort );
                    var count1 = MongoUtil.getInstance().QueryCollectionCount(mongoQuery.collection, mongoQuery.query, mongoQuery.project, mongoQuery.page, mongoQuery.limit, mongoQuery.sort );
                    Dictionary<string, long> pageData1 =  new Dictionary<string, long>();
                    pageData1["total"] = count1;
                    pageData1["now"] = mongoQuery.page;
                    pageData1["row"] = mongoQuery.limit;

                    this.Data = new Dictionary<String, Object>(){
                        {"showStatus", showStatus},
                        {"query", mongoQuery},
                        {"pageData", pageData1},
                        {"result", result1}
                    };
                    break;
            }
        }

        /// <summary>
        /// 統計資料
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// [
        ///   {
        ///     $match: {
        ///       EAICommand: ""
        ///     }
        ///  },
        /// 
        ///   {
        ///     $group: {
        ///       _id: {
        ///         Subject: "$EAICommand",
        ///         ErrorMsg: "$ErrorMsg"
        ///       },
        ///       count: {
        ///         $sum: 1
        ///       }
        ///     }
        ///   },
        /// 
        ///   {
        ///     $sort: {
        ///       count: -1
        ///     }
        ///   }
        /// 
        /// ]
        ///
        /// </remarks>
        /// <param name="mongoAggregate"></param>
        [Route("AggregateCollection")]
        [HttpPost]
        public void AggregateCollection([FromForm] MongoAggregate mongoAggregate)
        {
           
            this.Data = MongoUtil.getInstance().AggregateCollection(mongoAggregate.collection, mongoAggregate.pipeline);
            
            
        }


        [Route("test")]
        [HttpPost]
        public void test()
        {
            var bsonarr = "[{'$match':{EAICommand:'CHTMember','LogTime':{'$gt':new ISODate('2020-08-20T07:00:00.8017307Z')}}}]";
           
            this.Data = BsonSerializer.Deserialize<BsonArray>(bsonarr);
            
            
        }

    }
}
