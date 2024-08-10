FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AwsSqsMicroserviceTemplate.csproj", "./"]
RUN dotnet restore "AwsSqsMicroserviceTemplate.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "AwsSqsMicroserviceTemplate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AwsSqsMicroserviceTemplate.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AwsSqsMicroserviceTemplate.dll"]
