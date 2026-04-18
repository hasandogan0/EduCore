# EduCore - Online Learning Platform / Online Eğitim Platformu

[English](#english) | [Türkçe](#türkçe)

---

## English

EduCore is a comprehensive Learning Management System (LMS) built with modern web technologies. It provides a robust ecosystem where instructors can create courses and students can enroll and follow their lessons seamlessly.

### Architecture
The project follows a **Monorepo** structure:
- **backend/**: Layered architecture built with .NET 8, Web API, Entity Framework Core, and DTO-based Fluent Validation.
- **frontend/**: Modern UI developed with HTML5, CSS3, and Vanilla JavaScript.

### Tech Stack
- **Backend:** .NET 8, MySQL, EF Core, Fluent Validation, AutoMapper, JWT & Identity API.
- **Frontend:** HTML5, CSS3, Vanilla JS (Fetch API).

### Getting Started
1. **Clone:** `git clone https://github.com/hasandogan0/EduCore.git`
2. **Backend:** `cd backend && dotnet run --project EduCore.API`
3. **Frontend:** Open `frontend/index.html` (Live Server recommended).

---

## Türkçe

EduCore, modern web teknolojileri ile geliştirilmiş kapsamlı bir online eğitim (LMS) platformudur. Eğitmenlerin kurs oluşturabildiği, öğrencilerin ise bu kurslara kayıt olup dersleri takip edebildiği bir ekosistem sunar.

### Mimari Yapı
Proje, **Monorepo** yapısında geliştirilmiştir:
- **backend/**: .NET 8, Web API, EF Core ve Fluent Validation ile geliştirilmiş katmanlı mimari.
- **frontend/**: HTML5, CSS3 ve Vanilla JS ile geliştirilmiş modern kullanıcı arayüzü.

### Kullanılan Teknolojiler
- **Backend:** .NET 8, MySQL, EF Core, Fluent Validation, AutoMapper, JWT & Identity API.
- **Frontend:** HTML5, CSS3, Vanilla JS (Fetch API).

### Başlangıç
1. **Klonla:** `git clone https://github.com/hasandogan0/EduCore.git`
2. **Backend:** `cd backend && dotnet run --project EduCore.API`
3. **Frontend:** `frontend/index.html` dosyasını tarayıcıda açın (Live Server önerilir).

---

## Project Structure / Proje Yapısı

```text
EduCore/
├── backend/             # API & Business Logic
├── frontend/            # UI Files / Arayüz Dosyaları
└── README.md            # Documentation / Dokümantasyon