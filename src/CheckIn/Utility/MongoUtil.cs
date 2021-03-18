using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CheckIn.Utility
{
    /// <summary>
    /// 此為呼叫mongo範例程式
    /// </summary>
    public class MongoUtil
    {

        public static IMongoDatabase _db;

        public static IConfiguration Configuration { get; set; }
        public static MongoUtil getInstance()
		{

            if (_db == null)
			{
                //讀取設定檔
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
                Configuration = builder.Build();


                //連線mongodb
                var connectionString = Configuration["MongoLogConnection:ConnectionString"];
                var database = Configuration["MongoLogConnection:Database"];


                MongoClient _client = new MongoClient(connectionString);
                _db = _client.GetDatabase(database);


            }

			return new MongoUtil();
		}

        /// <summary>
        /// Return the number of milliseconds since 1970/01/01
        /// </summary>
        /// <returns></returns>
        private double getTime(){
            DateTime baseDate = new DateTime(1970, 1, 1);
            TimeSpan diff = DateTime.Now.ToUniversalTime() - baseDate;
            return diff.TotalMilliseconds;
        }

      

        public void Log(string message, string TAG="LOG")
        {
            var prjName = Assembly.GetCallingAssembly().GetName().Name;

            MongoLog log = new MongoLog
            {
                message = message,
                logLevel = TAG
            };
            _db.GetCollection<MongoLog>($"{prjName}_log").InsertOne(log);
        }

        public void LogInfo(string message)
        {
            var prjName = Assembly.GetCallingAssembly().GetName().Name;

            MongoLog log = new MongoLog
            {
                message = message,
                logLevel = "INFO"
            };
            _db.GetCollection<MongoLog>($"{prjName}_log").InsertOne(log);
        }


        //寫入錯誤紀錄
        public void LogError(Exception exception)
        {
            var prjName = Assembly.GetCallingAssembly().GetName().Name;
           


            MongoLog log = new MongoLog
            {
                message = exception.Message,
                logLevel = "ERROR",
                stackTrace = exception.StackTrace

            };
            _db.GetCollection<MongoLog>($"{prjName}_log").InsertOne(log);
        }

        //讀取錯誤紀錄
        public List<MongoLog> ListLog()
        {
            var prjName = Assembly.GetCallingAssembly().GetName().Name;

            var list =  _db.GetCollection<MongoLog>($"{prjName}_log")
            .Find(Builders<MongoLog>.Filter.Empty)
            .ToList();

            return list;
        }

        /// <summary>
        /// DB是否存在？
        /// </summary>
        /// <returns></returns>
        public Boolean isMongoLive(){
            return _db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);            
        }

        /// <summary>
        /// 查詢資料表
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="project"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        internal JArray QueryCollection(string collection, string query, string project, int page, int limit, string sort)
        {
            var queryDoc = BsonSerializer.Deserialize<BsonDocument>(query);
            var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sort);
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(project);


            var bsonList = _db.GetCollection<BsonDocument>(collection)
            .Find(queryDoc)
            .Project(projectDoc)
            .Skip((page-1) * limit)
            .Sort(sortDoc)
            .Limit(limit)
            .ToList<BsonDocument>();


            return ToJArray(bsonList);
        }

         /// <summary>
        /// 查詢資料表總量
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="project"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        internal long  QueryCollectionCount(string collection, string query, string project, int page, int limit, string sort)
        {
            var queryDoc = BsonSerializer.Deserialize<BsonDocument>(query);


           return _db.GetCollection<BsonDocument>(collection).CountDocuments(queryDoc);
        }

        internal List<string> ListCollection(){

            return _db.ListCollectionNames().ToList<string>();

        }

        internal void DropCollection(string colleciton){

            var collection = _db.GetCollection<BsonDocument>(colleciton);
            _db.DropCollection(collection.CollectionNamespace.CollectionName);

        }

        private JArray ToJArray(List<BsonDocument> bsonList){
            JArray ja = new JArray();
            foreach (var bson in bsonList)
            {
                var jo = JObject.Parse(ToJson(bson));
                ja.Add(jo);
            }
            return ja;

        }
        private BsonDocument[] ToBsonArr(JArray jArray){
            var result = new BsonDocument[jArray.Count];
            int i = 0;
            foreach (var item in jArray)
            {
                var str = ((JObject)item).ToString(Formatting.None);
                result[i] = BsonDocument.Parse(str);
                i++;
            }

            return result;

        }


        
        private string ToJson(BsonDocument bson)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonBinaryWriter(stream))
                {
                    BsonSerializer.Serialize(writer, typeof(BsonDocument), bson);
                }
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new BsonDataReader(stream))
                {
                    var sb = new StringBuilder();
                    var sw = new StringWriter(sb);
                    using (var jWriter = new JsonTextWriter(sw))
                    {
                        jWriter.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        jWriter.WriteToken(reader);
                    }
                    return sb.ToString();
                }
            }
        }

        /// <summary>
        /// 聚合查詢
        /// </summary>
        /// <param name="collection">資料表</param>
        /// <param name="bson">語法</param>
        internal JArray AggregateCollection(string collection, string bson)
        {
            

            JArray a = JArray.Parse(bson);

            var pipeline = ToBsonArr(a);
            //var pipeline = BsonDocument.Parse(bson);
            

           

            var aggregate = _db.GetCollection<BsonDocument>(collection).Aggregate<BsonDocument>(pipeline);

            var results = aggregate.ToList();

            return ToJArray(results);
        }
    }



    /// <summary>
    /// 紀錄物件
    /// </summary>
    public class MongoLog
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public DateTime updatetime { get; set; } = DateTime.Now;
        public String message { get; set; }

        public String stackTrace { get; set; }

        public string logLevel { get; set; }
        
    }
}