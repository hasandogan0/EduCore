# EduCore - Online Eğitim Platformu 

EduCore, modern web teknolojileri ile geliştirilmiş kapsamlı bir online eğitim (LMS) platformudur. Eğitmenlerin kurs oluşturabildiği, öğrencilerin ise bu kurslara kayıt olup dersleri takip edebildiği bir ekosistem sunar.

## Mimari Yapı

Proje, **Monorepo** yapısında geliştirilmiştir:
- **backend/**: .NET 8, Web API, Entity Framework Core ve Fluent Validation ile geliştirilmiş katmanlı mimari.
- **frontend/**: Html,Css ve Vanilla Js ile geliştirilmiş modern kullanıcı arayüzü.

## Kullanılan Teknolojiler

### Backend
- **Framework:** .NET 8 (Web API)
- **Veritabanı:** MySql
- **ORM:** Entity Framework Core
- **Doğrulama:** Fluent Validation (DTO tabanlı)
- **Mapping:** AutoMapper
- **Güvenlik:** JWT Authentication & Identity API

## Proje Klasör Yapısı

```text
EduCore/
├── backend/             # Tüm API ve Business mantığı
│   ├── EduCore.API      # Sunum Katmanı
│   ├── EduCore.Business # İş Mantığı ve Validasyonlar
│   ├── EduCore.DataAccess # Veritabanı İşlemleri
│   └── EduCore.Entity   # Veritabanı Modelleri
├── frontend/            # Kullanıcı arayüzü dosyaları
└── README.md            # Proje dokümantasyonu


Projeyi yerel bilgisayarınızda çalıştırmak için aşağıdaki adımları izleyin:

1- Depoyu Klonlayın

git clone [https://github.com/hasandogan0/EduCore.git](https://github.com/hasandogan0/EduCore.git)
cd EduCore

2- Backend'i Çalıştırın

cd backend
dotnet restore
dotnet run --project EduCore.API

3- Frontend'i Çalıştırın
    Frontend herhangi bir kurulum gerektirmez. frontend/ klasörüne gidin ve favori tarayıcınızla index.html dosyasını açın.

    Öneri: Geliştirme sırasında verilerin doğru yüklenmesi için Live Server (VS Code eklentisi) kullanmanız tavsiye edilir.

DTO Tabanlı Validasyon: Fluent Validation ile güvenli veri girişi.

Katmanlı Mimari: Sorumlulukların net ayrımı (Separation of Concerns).

Gelişmiş Rol Yönetimi: Öğrenci ve Eğitmen rolleri.

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!
