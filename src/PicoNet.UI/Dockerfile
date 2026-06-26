FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/PicoNet.UI/PicoNet.UI.csproj", "src/PicoNet.UI/"]
COPY ["src/PicoNet.Contracts/PicoNet.Contracts.csproj", "src/PicoNet.Contracts/"]
COPY ["src/PicoNet.Domain/PicoNet.Domain.csproj", "src/PicoNet.Domain/"]
COPY ["src/PicoNet.ServiceDefaults/PicoNet.ServiceDefaults.csproj", "src/PicoNet.ServiceDefaults/"]
RUN dotnet restore "src/PicoNet.UI/PicoNet.UI.csproj"
COPY . .
WORKDIR "/src/src/PicoNet.UI"
RUN dotnet build "./PicoNet.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PicoNet.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PicoNet.UI.dll"]
