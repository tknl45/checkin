# CheckIn API

## 使用框架

* .Net Core 3.1
* swagger

## 開發執行以下指令
> dotnet run

## 設定檔位置
> appsetting.json

請參考裡面的值設定




## 佈署win服務方式
> {專案檔}.csproj 中確認Framework 的 `<PropertyGroup>`加入`<RuntimeIdentifier>win7-x64</RuntimeIdentifier>`



```
<PropertyGroup>
<TargetFramework>netcoreapp3.1</TargetFramework>
<RuntimeIdentifier>win7-x64</RuntimeIdentifier>
</PropertyGroup>
```

> 佈署windows service

dotnet publish --configuration Release -o "{佈署指定資料夾}"

```
dotnet publish --configuration Release -o "D:\testService\CheckIn\src"
```

> 使用命令提示字元「cmd」 以「系統管理員身份」執行


sc create {服務名稱} binpath= "{佈署指定資料夾}\{專案檔}.exe --service"

```
sc create CheckIn binpath= "D:\testService\CheckIn\CheckIn.exe --service"
```



> 使用cmd 在最高管理員下執行

1. 開啟服務
sc start {服務名稱}
```
sc start CheckIn
```

2. 查詢服務狀態
sc query {服務名稱}
```
sc query CheckIn
```

4. 停止服務
sc stop {服務名稱}
```
sc stop CheckIn
```

3. 移除服務
sc delete {服務名稱}
```
sc delete CheckIn
```


## 佈署IIS服務方式

請參考此網站說明 https://docs.microsoft.com/zh-tw/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1

注意如下幾點

* 要安裝`IIS`
* 要安裝` .NET Core 裝載套件組合安裝程式`
* 佈署程式碼
> * 用 `佈署win服務方式`產生部署檔案
> * appsetting.json 修改 "Server.Runtime": "IIS" 
* 應用程式方式開啟
* 應用程式區設定
> * .NET CLR: 沒有Managed 程式碼
> * Managed管線模式: 整合式
















