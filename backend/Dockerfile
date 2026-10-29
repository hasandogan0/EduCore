# Ubuntu 22.04 tabanli .NET 8 build image
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src

COPY ["EduCore.API/EduCore.API.csproj", "EduCore.API/"]
COPY ["EduCore.Business/EduCore.Business.csproj", "EduCore.Business/"]
COPY ["EduCore.DataAccess/EduCore.DataAccess.csproj", "EduCore.DataAccess/"]
COPY ["EduCore.Entity/EduCore.Entity.csproj", "EduCore.Entity/"]

RUN dotnet restore "EduCore.API/EduCore.API.csproj"

COPY . .
WORKDIR /src/EduCore.API
RUN dotnet publish "EduCore.API.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Ubuntu 22.04 tabanli, production runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
RUN mkdir -p wwwroot/uploads/images wwwroot/uploads/videos \
    && chown -R $APP_UID:$APP_UID /app

USER $APP_UID
ENTRYPOINT ["dotnet", "EduCore.API.dll"]
