FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/PicoNet.Api/PicoNet.Api.csproj", "src/PicoNet.Api/"]
COPY ["src/PicoNet.ServiceDefaults/PicoNet.ServiceDefaults.csproj", "src/PicoNet.ServiceDefaults/"]
COPY ["src/PicoNet.Application/PicoNet.Application.csproj", "src/PicoNet.Application/"]
COPY ["src/PicoNet.Contracts/PicoNet.Contracts.csproj", "src/PicoNet.Contracts/"]
COPY ["src/PicoNet.Domain/PicoNet.Domain.csproj", "src/PicoNet.Domain/"]
COPY ["src/PicoNet.Infrastructure/PicoNet.Infrastructure.csproj", "src/PicoNet.Infrastructure/"]
RUN dotnet restore "src/PicoNet.Api/PicoNet.Api.csproj"
COPY . .
WORKDIR "/src/src/PicoNet.Api"
RUN dotnet build "./PicoNet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PicoNet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PicoNet.Api.dll"]
