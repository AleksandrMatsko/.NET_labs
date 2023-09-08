FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["dot_NET_labs/dot_NET_labs.csproj", "dot_NET_labs/"]
RUN dotnet restore "dot_NET_labs/dot_NET_labs.csproj"
COPY . .
WORKDIR "/src/dot_NET_labs"
RUN dotnet build "dot_NET_labs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dot_NET_labs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dot_NET_labs.dll"]
