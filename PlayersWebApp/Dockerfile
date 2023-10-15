FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PlayersWebApp/PlayersWebApp.csproj", "PlayersWebApp/"]
RUN dotnet restore "PlayersWebApp/PlayersWebApp.csproj"
COPY . .
WORKDIR "/src/PlayersWebApp"
RUN dotnet build "PlayersWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlayersWebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlayersWebApp.dll"]
