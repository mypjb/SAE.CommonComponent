#!/bin/sh

dotnet run --project src/SAE.CommonComponent.Master/SAE.CommonComponent.Master.csproj -c Debug --urls "http://0.0.0.0:8080" -e "ASPNETCORE_ENVIRONMENT=Development" &
