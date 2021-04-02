#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Teleport/Teleport.csproj", "Teleport/"]
RUN dotnet restore "Teleport/Teleport.csproj"
COPY . .
WORKDIR "/src/Teleport"
RUN dotnet build "Teleport.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Teleport.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir Database Database/Transaction Database/StockInfo Database/Customer Database/PttArticles
# ENTRYPOINT ["dotnet", "Teleport.dll"]

# Use the following instead for Heroku
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Teleport.dll
