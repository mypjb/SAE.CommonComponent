$dir = Split-Path -Parent $MyInvocation.MyCommand.Definition
cd $dir
dotnet publish src/SAE.CommonComponent.Application -o plugin/Application
dotnet publish src/SAE.CommonComponent.Authorize -o plugin/Authorize
dotnet publish src/SAE.CommonComponent.ConfigServer -o plugin/ConfigServer
dotnet publish src/SAE.CommonComponent.Identity -o plugin/Identity
dotnet publish src/SAE.CommonComponent.Routing -o plugin/Routing
dotnet publish src/SAE.CommonComponent.User -o plugin/User
dotnet publish src/SAE.CommonComponent.OAuth -o plugin/OAuth
#Start-Job -ScriptBlock { dotnet publish src/SAE.CommonComponent.Application -o plugin/Application } -Name "Application"
#Start-Job -ScriptBlock { dotnet publish src/SAE.CommonComponent.Authorize -o plugin/Authorize } -Name "Authorize"
#Start-Job -ScriptBlock { dotnet publish src/SAE.CommonComponent.ConfigServer -o plugin/ConfigServer } -Name "ConfigServer"
#Start-Job -ScriptBlock { dotnet publish src/SAE.CommonComponent.Identity -o plugin/Identity } -Name "Identity"
#Start-Job -ScriptBlock { dotnet publish src/SAE.CommonComponent.Routing -o plugin/Routing } -Name "Routing"
#Start-Job -ScriptBlock { dotnet publish src/SAE.CommonComponent.User -o plugin/User } -Name "User"
#Get-Job | Wait-Job