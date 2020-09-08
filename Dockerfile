FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder

WORKDIR /code

RUN pwd

COPY . .

RUN ["dotnet","nuget","add","source","https://nuget.cdn.azure.cn/v3/index.json","-n","azure.org"]


RUN ["dotnet","publish","CommonComponent/src/SAE.CommonComponent.Routing/SAE.CommonComponent.Routing.csproj","-o","/app"]

RUN ls 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

COPY --from=builder /app .

RUN ls 

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet","SAE.CommonComponent.Routing.dll"]
