# .NetCoreAndXunit
.NetCore + Xunit practice

## 注意

測試前要先把`src/ApiProxy/appsettings.json` 覆寫到 `test/UniTests/bin/Debug/netcoreapp2.0`

## execute

> dotnet test

測試列表

> dotnet test -t

測試類別為API的test case

> dotnet test --filter Category=API

留下紀錄

> dotnet test --logger:trx


測試類別為API的test case 並留下紀錄
> dotnet test --filter Category=API --logger:trx
> dotnet test --filter Category=RestAPI --logger:trx


> dotnet test --filter Category=mongo

測試涵蓋率
> 安裝Coverlet指令: dotnet add package coverlet.msbuild
> dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
> dotnet test /p:CollectCoverage=true --logger:trx
> dotnet watch test /p:CollectCoverage=true --logger:trx /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info 
> dotnet test /p:CollectCoverage=true --logger:trx /p:CoverletOutput=./TestResults/coverage.json


指令執行後會產生TestResults資料夾
資料夾中的有xxxx.trx檔，可藉由visual studio 2017打開