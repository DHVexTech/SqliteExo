FROM mcr.microsoft.com/dotnet/core/runtime:2.2
FROM mcr.microsoft.com/dotnet/core/sdk:2.2

COPY bin/Release/netcoreapp2.2/publish/ app/

ENTRYPOINT ["dotnet", "app/ShakespeareSqlite.dll"]