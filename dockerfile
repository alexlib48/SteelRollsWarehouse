FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SteelRollsWarehouse.API/SteelRollsWarehouse.API.csproj", "SteelRollsWarehouse.API/"]
COPY ["SteelRollsWarehouse.Application/SteelRollsWarehouse.Application.csproj", "SteelRollsWarehouse.Application/"]
COPY ["SteelRollsWarehouse.Domain/SteelRollsWarehouse.Domain.csproj", "SteelRollsWarehouse.Domain/"]
COPY ["SteelRollsWarehouse.Infrastructure/SteelRollsWarehouse.Infrastructure.csproj", "SteelRollsWarehouse.Infrastructure/"]
RUN dotnet restore "SteelRollsWarehouse.API/SteelRollsWarehouse.API.csproj"
COPY . .
WORKDIR "/src/SteelRollsWarehouse.API"
RUN dotnet build "SteelRollsWarehouse.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SteelRollsWarehouse.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SteelRollsWarehouse.API.dll"]