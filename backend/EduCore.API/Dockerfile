# 1. Build Aşaması (SDK kullanarak derleme)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Tüm .csproj dosyalarını tek tek kopyalıyoruz (Restore hızı için)
COPY ["EduCore.API/EduCore.API.csproj", "EduCore.API/"]
COPY ["EduCore.Business/EduCore.Business.csproj", "EduCore.Business/"]
COPY ["EduCore.DataAccess/EduCore.DataAccess.csproj", "EduCore.DataAccess/"]
COPY ["EduCore.Entity/EduCore.Entity.csproj", "EduCore.Entity/"]

# Restore işlemi
RUN dotnet restore "EduCore.API/EduCore.API.csproj"

# Kalan tüm dosyaları kopyala
COPY . .
WORKDIR "/src/EduCore.API"

# Projeyi derle
RUN dotnet build "EduCore.API.csproj" -c Release -o /app/build

# 2. Publish Aşaması
FROM build AS publish
RUN dotnet publish "EduCore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Final (Runtime) Aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# wwwroot klasörünü hazırla (Dosya yüklemeleri için)
RUN mkdir -p wwwroot/uploads/images && mkdir -p wwwroot/uploads/videos

ENTRYPOINT ["dotnet", "EduCore.API.dll"]