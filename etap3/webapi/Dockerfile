FROM mcr.microsoft.com/dotnet/core/sdk:2.2
WORKDIR /app
COPY * /app/
RUN dotnet restore
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/etap2.dll"]