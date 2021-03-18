#此指令會初始化新的建置階段，並設定其餘指令的基底映像。
FROM registry.ecloud-qa.cht.com.tw:5000/app/dotnet/dotnet-31-runtime-rhel7

#設定內部port
EXPOSE 8080

# 設定docker內容
ADD bin/Release/netcoreapp3.1/rhel.7-x64/publish/ .

RUN ls

# 執行服務
CMD ["dotnet", "CheckIn.dll"]
