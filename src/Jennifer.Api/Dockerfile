FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Jennifer.Api/Jennifer.Api.csproj", "Jennifer.Api/"]
COPY ["Jennifer.SharedKernel/Jennifer.SharedKernel.csproj", "Jennifer.SharedKernel/"]
COPY ["Jennifer.Infrastructure/Jennifer.Infrastructure.csproj", "Jennifer.Infrastructure/"]
COPY ["Jennifer.External.OAuth/Jennifer.External.OAuth.csproj", "Jennifer.External.OAuth/"]
COPY ["Jennifer.Account/Jennifer.Account.csproj", "Jennifer.Account/"]
RUN dotnet restore "Jennifer.Api/Jennifer.Api.csproj"
COPY . .
WORKDIR "/src/Jennifer.Api"
RUN dotnet build "./Jennifer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Jennifer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jennifer.Api.dll"]
