# MonitorService
中華電信 APP讀取mongoDB 統計log資料



##  Mongo 查詢功能一覽
*  /Monitor/ListCollection  列出資料表
*  /Monitor/QueryCollection  查詢資料
### 查指令是否過久

collection : `LOG_EAI_REST_OUTPUT`

query
```
{TotalSeconds: {"$gt":10}}
```


### 查某最近VM是否有指令出錯

collection : `LOG_EAI_OUTPUTDATA`

query
```
{ "$nor": [ { STATUS:"1" }, { STATUS:"0" } ] , ClientIP: "10.180.22.33"}
```

### 查某時間區間下 指令的狀態 (很慢 不建議)

collection : `LOG_EAI_OUTPUTDATA`

query
```
{Command:"OCSG.GetSubscriber.REQ" ,LogTime: {'$gte': new ISODate('2020-08-25T08:00:00.000Z'),'$lt': new ISODate('2020-08-25T08:10:00.000Z') }}
```


*  /Monitor/AggregateCollection  使用aggregate語法查資料

##  統計功能一覽
*  GET  ​/Monitor​/CHTMemberErrorStat  顯示近幾分鐘會員錯誤數量
*  GET  ​/Monitor​/CHTMemberDayErrorStat  會員錯誤數量日期統計
*  GET  ​/Monitor​/ErrorStatLastMins  最近10分鐘錯誤統計
*  GET  ​/Monitor​/EAIStatLastMins  最近10分鐘EAI狀態統計
*  GET  ​/Monitor​/APIStatLastMins  最近10分鐘API狀態統計
*  GET  ​/Monitor​/IPStatLastMins  最近10分鐘IP狀態統計








