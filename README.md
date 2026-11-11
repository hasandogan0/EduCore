# EduCore - Online Learning Platform / Online Eğitim Platformu

[English](#english) | [Türkçe](#türkçe)

---

## English

EduCore is a Learning Management System (LMS) where instructors can create
courses and students can enroll in courses and follow their lessons.

### Architecture

EduCore is organized as a monorepo:

- **backend/**: .NET 8 Web API with layered architecture, Entity Framework
  Core, DTOs, FluentValidation, AutoMapper, JWT authentication, and ASP.NET
  Core Identity.
- **frontend/**: HTML5, CSS3, and TypeScript-based vanilla UI.
- **backend/Tests/**: Unit and integration tests.

### Tech Stack

- .NET 8 and ASP.NET Core Web API
- Microsoft SQL Server and Entity Framework Core
- FluentValidation and AutoMapper
- JWT authentication and ASP.NET Core Identity
- HTML5, CSS3, TypeScript, and Fetch API
- Docker and Docker Compose

### Requirements

- .NET 8 SDK for local development
- Docker Desktop or Docker Engine with Docker Compose
- Git

### Local Development

Clone the repository and restore/build the backend:

```bash
git clone https://github.com/hasandogan0/EduCore.git
cd EduCore/backend
dotnet restore EduCore.sln
dotnet build EduCore.sln
dotnet test EduCore.sln
```

Run the API:

```bash
dotnet run --project EduCore.API
```

For local frontend development, install Node.js, then build the TypeScript
sources:

```bash
cd frontend
npm install
npm run typecheck
npm run build
```

After the build, serve `frontend/` with a static server such as the VS Code
Live Server extension. The frontend communicates with the API through the
typed Fetch API client in `frontend/src/api.ts`.

### Docker Compose

The root `docker-compose.yml` starts:

- **db**: Microsoft SQL Server 2022 on port `1433`
- **api**: the .NET 8 API on `http://localhost:8080`
- **frontend**: the Nginx-hosted frontend on `http://localhost:3000`

Create a local `.env` file in the repository root. Do not commit this file:

```env
SQL_SA_PASSWORD=StrongPassword123!
SQL_DATABASE=EduCoreDb
JWT_KEY=change-this-to-a-long-secret-key
```

The SQL Server password must comply with SQL Server password requirements, and
the JWT key should be replaced with a long, unique secret.

Start the services:

```bash
docker compose up --build -d
```

Open the application at `http://localhost:3000`. The frontend container
proxies `/api` and `/uploads` requests to the API, so Live Server or the .NET
SDK is not required on the machine running the Docker deployment.

The database uses the named `mssql_data` volume, and the API waits for the
database health check before starting. Check the service status with:

```bash
docker compose ps
```

Stop the services with:

```bash
docker compose down
```

To remove the persisted database volume as well:

```bash
docker compose down -v
```

### CI/CD

The `.github/workflows/ci-cd.yml` workflow runs on pushes to `main` and on
pull requests targeting `main`. It:

1. Restores, builds, and tests `backend/EduCore.sln`.
2. Builds and publishes the Docker image after a successful `main` branch
   push.

The published image is available from GitHub Container Registry:

```text
ghcr.io/hasandogan0/educore:latest
```

---

## Türkçe

EduCore, eğitmenlerin kurs oluşturabildiği ve öğrencilerin kurslara kayıt olup
dersleri takip edebildiği bir Öğrenme Yönetim Sistemi (LMS) platformudur.

### Mimari Yapı

EduCore bir monorepo olarak düzenlenmiştir:

- **backend/**: .NET 8 Web API, katmanlı mimari, Entity Framework Core, DTO,
  FluentValidation, AutoMapper, JWT ve ASP.NET Core Identity.
- **frontend/**: HTML5, CSS3 ve TypeScript tabanlı vanilla kullanıcı arayüzü.
- **backend/Tests/**: Birim ve entegrasyon testleri.

### Kullanılan Teknolojiler

- .NET 8 ve ASP.NET Core Web API
- Microsoft SQL Server ve Entity Framework Core
- FluentValidation ve AutoMapper
- JWT kimlik doğrulama ve ASP.NET Core Identity
- HTML5, CSS3, TypeScript ve Fetch API
- Docker ve Docker Compose

### Gereksinimler

- Yerel geliştirme için .NET 8 SDK
- Docker Desktop veya Docker Compose destekli Docker Engine
- Git

### Yerel Geliştirme

Projeyi klonlayıp backend bağımlılıklarını yükleyin:

```bash
git clone https://github.com/hasandogan0/EduCore.git
cd EduCore/backend
dotnet restore EduCore.sln
dotnet build EduCore.sln
dotnet test EduCore.sln
```

API'yi çalıştırın:

```bash
dotnet run --project EduCore.API
```

Yerel frontend geliştirmesi için Node.js yükleyin ve TypeScript kaynaklarını
derleyin:

```bash
cd frontend
npm install
npm run typecheck
npm run build
```

Derleme sonrasında `frontend/` klasörünü VS Code Live Server gibi bir statik
sunucu ile çalıştırabilirsiniz. Frontend, backend ile uyumlu tipli Fetch API
istemcisi (`frontend/src/api.ts`) üzerinden haberleşir.

### Docker Compose

Kök dizindeki `docker-compose.yml` aşağıdaki servisleri başlatır:

- **db**: `1433` portunda Microsoft SQL Server 2022
- **api**: `http://localhost:8080` adresinde .NET 8 API
- **frontend**: Nginx üzerinden `http://localhost:3000` adresinde frontend

Proje kök dizininde yerel bir `.env` dosyası oluşturun. Bu dosyayı commit
etmeyin:

```env
SQL_SA_PASSWORD=StrongPassword123!
SQL_DATABASE=EduCoreDb
JWT_KEY=change-this-to-a-long-secret-key
```

SQL Server parolası SQL Server gereksinimlerini karşılamalı, JWT anahtarı ise
uzun ve benzersiz bir gizli değerle değiştirilmelidir.

Servisleri başlatın:

```bash
docker compose up --build -d
```

Uygulamayı `http://localhost:3000` adresinden açın. Frontend container'ı
`/api` ve `/uploads` isteklerini API'ye yönlendirir; Docker ile dağıtım yapılan
makinede Live Server veya .NET SDK gerekmez.

Veritabanı `mssql_data` isimli volume üzerinde saklanır ve API, veritabanı
health check başarılı olduktan sonra başlatılır. Servis durumunu kontrol etmek
için:

```bash
docker compose ps
```

Servisleri durdurmak için:

```bash
docker compose down
```

Kalıcı veritabanı volume'ünü de silmek için:

```bash
docker compose down -v
```

### CI/CD

`.github/workflows/ci-cd.yml` workflow'u `main` branch'ine yapılan push'larda
ve `main` hedefli pull request'lerde çalışır. Workflow:

1. `backend/EduCore.sln` için restore, build ve test adımlarını çalıştırır.
2. `main` branch'ine başarılı bir push sonrasında Docker image'ını oluşturup
   yayınlar.

Yayınlanan image GitHub Container Registry üzerinden kullanılabilir:

```text
ghcr.io/hasandogan0/educore:latest
```

---

## Project Structure / Proje Yapısı

```text
EduCore/
├── backend/
│   ├── EduCore.API/          # HTTP API and Docker entry point
│   ├── EduCore.Business/     # DTOs, validation, mapping, services
│   ├── EduCore.DataAccess/   # EF Core, repositories, migrations
│   ├── EduCore.Entity/       # Domain entities
│   └── Tests/                # Unit and integration tests
├── frontend/                 # Web UI
├── .github/workflows/        # CI/CD workflow
├── docker-compose.yml        # SQL Server and API services
└── README.md                 # Documentation / Dokümantasyon
```
