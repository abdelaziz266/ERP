# ERP System

äÙÇã ERP ãÊßÇãá ÈÜ .NET 10 æ ASP.NET Core ãÚ ÏÚã ßÇãá ááãÕÇÏŞÉ æÇáÊÍßã ÈÇáÃÏæÇÑ.

## ?? ÇáããíÒÇÊ

### ÇáãÕÇÏŞÉ æÇáÊİæíÖ
- ? ãÕÇÏŞÉ JWT ãÚ Token Claims
- ? äÙÇã ÇáÃÏæÇÑ (Roles) ãÚ Identity Framework
- ? ÑÈØ ÇáãÓÊÎÏãíä ÈÇáÃÏæÇÑ (User Roles)
- ? ÇÓÊÎÑÇÌ ÈíÇäÇÊ ÇáãÓÊÎÏã ãä Token

### ÇáÊÚÏÏíÉ ÇááÛæíÉ
- ? ÏÚã ÇááÛÉ ÇáÚÑÈíÉ æÇáÅäÌáíÒíÉ
- ? ÇáÑÓÇÆá ÊÊÍÏÏ ÍÓÈ áÛÉ ÇáãÓÊÎÏã ãä Token
- ? Localization Service ááÑÓÇÆá ÇáÏíäÇãíßíÉ

### æÇÌåÇÊ ÈÑãÌíÉ ŞæíÉ
- ? CRUD ßÇãá ááãÓÊÎÏãíä æÇáÃÏæÇÑ
- ? Pagination ãÚ Search æ Row Count
- ? ãÚÇáÌÉ ÔÇãáÉ ááÃÎØÇÁ (Exception Handling)
- ? Swagger UI ãÚ JWT Authentication

### ÇáåäÏÓÉ ÇáãÚãÇÑíÉ
- ? Clean Architecture
- ? Domain-Driven Design
- ? Repository Pattern
- ? Unit of Work Pattern

## ?? ÇáãÊØáÈÇÊ

- .NET 10 SDK
- SQL Server (Ãæ Ãí ŞÇÚÏÉ ÈíÇäÇÊ ãÏÚæãÉ)
- Visual Studio 2022 Ãæ VS Code

## ?? ÇáÊËÈíÊ

### 1. ÇÓÊäÓÇÎ ÇáãÔÑæÚ

```bash
git clone https://github.com/abdelaziz266/ERP.git
cd ERP
```

### 2. ÇÓÊÚÇÏÉ ÇáÍÒã

```bash
dotnet restore
```

### 3. ÊÍÏíË ŞÇÚÏÉ ÇáÈíÇäÇÊ

```bash
dotnet ef database update
```

### 4. ÊÔÛíá ÇáãÔÑæÚ

```bash
dotnet run --project ERPProject/ERP.Api.csproj
```

ÓÊßæä ÇáÜ API ãÊÇÍÉ Úáì: `https://localhost:7000`
ÓÊßæä Swagger UI ãÊÇÍÉ Úáì: `https://localhost:7000/swagger`

## ?? ÇáÜ Endpoints ÇáÑÆíÓíÉ

### Authentication
```
POST   /api/auth/login                 - ÊÓÌíá ÇáÏÎæá
```

### Users
```
GET    /api/users                      - ÇáÍÕæá Úáì ÌãíÚ ÇáãÓÊÎÏãíä
GET    /api/users/{id}                 - ÇáÍÕæá Úáì ãÓÊÎÏã ÈÜ ID
POST   /api/users                      - ÅäÔÇÁ ãÓÊÎÏã
PUT    /api/users/{id}                 - ÊÍÏíË ãÓÊÎÏã
PUT    /api/users/language             - ÊÍÏíË áÛÉ ÇáãÓÊÎÏã
DELETE /api/users/{id}                 - ÍĞİ ãÓÊÎÏã
```

### Roles
```
GET    /api/roles?pageNumber=1&pageSize=10&searchTerm=&language=en
                                        - ÇáÍÕæá Úáì ÇáÃÏæÇÑ ãÚ Pagination
GET    /api/roles/{id}                 - ÇáÍÕæá Úáì ÏæÑ ÈÜ ID
POST   /api/roles                      - ÅäÔÇÁ ÏæÑ
PUT    /api/roles/{id}                 - ÊÍÏíË ÏæÑ
DELETE /api/roles/{id}                 - ÍĞİ ÏæÑ
```

## ?? ÈíÇäÇÊ ÇáÏÎæá ÇáÇİÊÑÇÖíÉ

```
Username: superadmin
Password: SuperAdmin@123
```

## ??? åíßá ÇáãÔÑæÚ

```
ERP/
??? ERP.SharedKernel/              # ÇáÜ Shared Code æÇáÜ DTOs
??? ERP.Modules.Users.Domain/      # Domain Entities æÇáÜ Repositories
??? ERP.Modules.Users.Application/ # Business Logic æÇáÜ Services
??? ERP.Modules.Users.Infrastructure/
?   ??? Data/                      # DbContext æ Migrations
?   ??? Repositories/              # Repository Implementation
?   ??? UnitOfWork/                # Unit of Work Pattern
??? ERPProject/                    # API Project
    ??? Controllers/               # API Controllers
```

## ?? ÇáãÊÛíÑÇÊ ÇáÈíÆíÉ

ÊÍÏíË `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ERPDb;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "ERPProject",
    "Audience": "ERPProjectApi",
    "ExpirationMinutes": 60
  }
}
```

## ?? ÇáãíÒÇÊ ÇáŞÇÏãÉ

- [ ] Module Management System
- [ ] Permission-based Authorization
- [ ] Audit Logging
- [ ] Data Encryption
- [ ] API Rate Limiting
- [ ] Caching Strategy

## ?? ÇáãÓÇåãÉ

äÑÍÈ ÈÇáãÓÇåãÇÊ! íÑÌì:

1. Fork ÇáãÔÑæÚ
2. ÅäÔÇÁ branch ÌÏíÏ (`git checkout -b feature/amazing-feature`)
3. Commit ÇáÊÛííÑÇÊ (`git commit -m 'Add amazing feature'`)
4. Push Åáì ÇáÜ Branch (`git push origin feature/amazing-feature`)
5. İÊÍ Pull Request

## ?? ÇáÊÑÎíÕ

åĞÇ ÇáãÔÑæÚ ãÑÎÕ ÊÍÊ MIT License - ÇäÙÑ ãáİ LICENSE ááÊİÇÕíá.

## ?? ÇáÊæÇÕá

ááÃÓÆáÉ Ãæ ÇáÇŞÊÑÇÍÇÊ¡ íÑÌì İÊÍ Issue İí GitHub.

---

Êã ÇáÊØæíÑ ÈæÇÓØÉ: Abdelaziz Morsy
ÂÎÑ ÊÍÏíË: 2024
