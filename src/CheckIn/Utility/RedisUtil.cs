using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CheckIn.Utility
{
    /// <summary>
    /// 讀取Redis資料
    /// </summary>
    public class RedisUtil {
        static object _lock = new object ();

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection;

        public static IConfiguration Configuration { get; set; }

        private static int dbIndex = -1;

        private static Object obj_lock = new Object();

        static RedisUtil () {
            lock (obj_lock)
            {
                //讀取設定檔
                Configuration = AppSettingsUtility.getInstance ().builder.Build ();

                var url = Configuration["RedisConnection:ConnectionString"]??"localhost";
                var port = Int16.Parse (Configuration["RedisConnection:ConnectionPort"]??"6379");
                dbIndex = Int16.Parse (Configuration["RedisConnection:DbIndex"]??"1");
                var password = Configuration["RedisConnection:Password"]??"";
                var ssl = Boolean.Parse (Configuration["RedisConnection:Ssl"]??"false");

                ConfigurationOptions option = new ConfigurationOptions {
                    AbortOnConnectFail = false,
                    EndPoints = { { url, port } },
                    Password = password,
                    Ssl = ssl,
                    SyncTimeout = 3000
                };

                //連線
                LazyConnection = new Lazy<ConnectionMultiplexer> (() => ConnectionMultiplexer.Connect (option));
                
                
            }
        }


        public static ConnectionMultiplexer Connection => LazyConnection.Value;
        public static IDatabase GetDB => Connection.GetDatabase (dbIndex);
        public static IServer GetServer => Connection.GetServer (Connection.GetEndPoints () [0]);

        public static RedisUtil getInstance () {
            return new RedisUtil();
        }

        public Lazy<ConnectionMultiplexer> showConnInfo(){
            return LazyConnection;
        }

        /// <summary>
        /// 取得值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T> (string key) {
            var value = await GetDB.StringGetAsync (key);

            if (!value.IsNull)
                return JsonConvert.DeserializeObject<T> (value);
            else {
                return default (T);
            }
        }

        /// <summary>
        /// 取得剩餘時間
        /// </summary>
        /// <param name="key"></param>
        public TimeSpan? TimeToLive (string key) {
            return GetDB.KeyTimeToLive (key);
        }

        /// <summary>
        /// 設定值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeToLive">暫存時間（秒）</param>
        /// <returns></returns>
        public async Task<bool> SetAsync (string key, object value, int timeToLive) {
            TimeSpan? expireHours = null;
            if (timeToLive > -1) {
                expireHours = TimeSpan.FromSeconds (timeToLive);
            }

            return await GetDB.StringSetAsync (key, JsonConvert.SerializeObject (value), expireHours);
        }

        /// <summary>
        /// 移除key
        /// </summary>
        /// <param name="key"></param>
        public void Remove (string key) {
            GetDB.KeyDelete (key);
        }

        /// <summary>
        /// 移除全部key
        /// </summary>
        public void RemoveAll () {
            foreach (var key in GetServer.Keys ()) {
                GetDB.KeyDelete (key);
            }
        }

        /// <summary>
        /// 取得key列表
        /// </summary>
        /// <param name="keyPattern">可以透過regex來判斷</param>
        /// <returns></returns>
        public List<string> GetKeys (String keyPattern = "*") {
            List<string> list = new List<string> ();
            foreach (var key in GetServer.Keys (dbIndex, keyPattern)) {
                list.Add (key);
            }

            return list;
        }

    }
}