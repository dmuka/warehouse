FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Warehouse.sln", "."]
COPY ["src/Warehouse.Core/Warehouse.Core.csproj", "src/Warehouse.Core/"]
COPY ["src/Warehouse.Domain/Warehouse.Domain.csproj", "src/Warehouse.Domain/"]
COPY ["src/Warehouse.Infrastructure/Warehouse.Infrastructure.csproj", "src/Warehouse.Infrastructure/"]
COPY ["src/Warehouse.Application/Warehouse.Application.csproj", "src/Warehouse.Application/"]
COPY ["src/Warehouse.Presentation/Warehouse.Presentation.csproj", "src/Warehouse.Presentation/"]
COPY ["tests/Warehouse.IntegrationTests/Warehouse.IntegrationTests.csproj", "tests/Warehouse.IntegrationTests/"]
COPY ["tests/Warehouse.UnitTests/Warehouse.UnitTests.csproj", "tests/Warehouse.UnitTests/"]

RUN dotnet restore "Warehouse.sln"

COPY . .
RUN dotnet build "Warehouse.sln" -c Release -p:ExcludeTests=true

FROM build AS publish
RUN dotnet publish "src/Warehouse.Presentation/Warehouse.Presentation.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Warehouse.Presentation.dll"]