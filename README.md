# 📚 API de Gestión de Libros - Proyecto de Calidad de Software

API RESTful desarrollada en .NET 8 con CI/CD automatizado a Azure, siguiendo especificación JSend y mejores prácticas de desarrollo.

> 🌐 **API en Producción**: https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net
>
> 📖 **Documentación Swagger**: https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net/swagger/index.html

---

## 🚀 Características

### Arquitectura y Patrones

- ✅ **Clean Architecture** (Repository + Service Pattern)
- ✅ **DTOs** con AutoMapper para separación de capas
- ✅ **Dependency Injection** en toda la aplicación
- ✅ **JSend Specification** para respuestas HTTP estandarizadas

### Calidad y Testing

- ✅ **Tests Unitarios** con NUnit + Moq (>70% cobertura)
- ✅ **FluentValidation** para validaciones declarativas
- ✅ **CI/CD** automatizado con GitHub Actions
- ✅ **Code Coverage Reports** en cada build
- ✅ **Pipeline**: Build → Test → Deploy

### Seguridad y Configuración

- ✅ **Variables de Entorno** con DotNetEnv
- ✅ **Rate Limiting** (100 req/min) para protección DoS
- ✅ **CORS** configurado para frontend
- ✅ **HTTPS** redirection
- ✅ **Secrets Management** con GitHub Secrets

### Funcionalidades

- ✅ **CRUD completo** de libros
- ✅ **Paginación** en listados
- ✅ **Logging estructurado** con Serilog
- ✅ **Health Checks** para monitoreo
- ✅ **Global Exception Handling** con JSend
- ✅ **Swagger/OpenAPI** para documentación interactiva

---

## 🛠️ Stack Tecnológico

### Backend

- **Framework**: .NET 8.0
- **ORM**: Entity Framework Core 9.0
- **Validación**: FluentValidation 11.3.0
- **Mapping**: AutoMapper 12.0.1
- **Logging**: Serilog 8.0.0

### Base de Datos

- **Azure SQL Database** (PaaS)
- **Conexión**: Variables de entorno con DotNetEnv 3.1.1

### Testing

- **Framework**: NUnit 3.14.0
- **Mocking**: Moq 4.20.72
- **Coverage**: XPlat Code Coverage + ReportGenerator

### DevOps & Cloud

- **Cloud Provider**: Microsoft Azure
- **Hosting**: Azure App Service (Chile Central)
- **CI/CD**: GitHub Actions
- **Deployment**: Automatizado en cada push a master

### Documentación

- **API Docs**: Swagger/OpenAPI 3.0
- **Package**: Swashbuckle.AspNetCore 6.6.2

### Seguridad & Monitoreo

- **Health Checks**: AspNetCore.HealthChecks.SqlServer
- **Rate Limiting**: ASP.NET Core Rate Limiting
- **Environment Variables**: DotNetEnv 3.1.1

---

## 🌐 Deployment en Azure

### Infraestructura

| Componente        | Servicio Azure     | Región        |
| ----------------- | ------------------ | ------------- |
| **API**           | Azure App Service  | Chile Central |
| **Base de Datos** | Azure SQL Database | Chile Central |
| **Plan**          | Free/Student       | F1            |
| **Runtime**       | .NET 8.0           | Windows       |

### URL de Producción

- **Base URL**: https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net
- **Swagger UI**: https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net/swagger/index.html
- **Health Check**: https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net/health

### Pipeline de CI/CD

```
Push to master → GitHub Actions
        ↓
┌────────────────────────────────┐
│  Job 1: BUILD                  │
│  - Checkout código             │
│  - Setup .NET 8                │
│  - Build (Release)             │
│  - Publish                     │
│  - Upload artifact             │
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  Job 2: TEST                   │
│  - Checkout código             │
│  - Setup .NET 8                │
│  - Install ReportGenerator     │
│  - Run tests con coverage      │
│  - Generate HTML report        │
│  - Upload test results (TRX)   │
│  - Upload coverage report      │
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  Job 3: DEPLOY                 │
│  - Download build artifact     │
│  - Login to Azure              │
│  - Deploy to App Service       │
└────────────────────────────────┘
        ↓
    ✅ API en Producción
```

### Variables de Entorno en Azure

Configuradas en Azure App Service → Configuration → Application settings:

- `AZURE_SQL_SERVER`
- `AZURE_SQL_DATABASE`
- `AZURE_SQL_USER`
- `AZURE_SQL_PASSWORD`
- `ASPNETCORE_ENVIRONMENT=Production`

---

## 📡 Endpoints de la API

### Base URL

```
https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net
```

### Documentación y Estado

| Endpoint   | Método | Descripción                   |
| ---------- | ------ | ----------------------------- |
| `/`        | GET    | Información de la API (JSend) |
| `/status`  | GET    | Estado del servidor (JSend)   |
| `/health`  | GET    | Health check de servicios     |
| `/swagger` | GET    | Documentación interactiva     |

### Gestión de Libros (CRUD)

| Endpoint           | Método | Descripción              | Formato |
| ------------------ | ------ | ------------------------ | ------- |
| `/api/Libros`      | GET    | Listar libros (paginado) | JSend   |
| `/api/Libros/{id}` | GET    | Obtener libro por ID     | JSend   |
| `/api/Libros`      | POST   | Crear nuevo libro        | JSend   |
| `/api/Libros/{id}` | PUT    | Actualizar libro         | JSend   |
| `/api/Libros/{id}` | DELETE | Eliminar libro           | JSend   |

### Ejemplos de Uso en Producción

#### Listar libros con paginación

```bash
curl "https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net/api/Libros?page=1&pageSize=10"
```

**Respuesta JSend:**

```json
{
  "status": "success",
  "data": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPreviousPage": false,
    "items": [
      {
        "id": 1,
        "titulo": "Clean Code",
        "autor": "Robert C. Martin",
        "anio": 2008,
        "genero": "Software"
      }
    ]
  }
}
```

#### Crear un libro

```bash
curl -X POST "https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net/api/Libros" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Clean Code",
    "autor": "Robert C. Martin",
    "anio": 2008,
    "genero": "Software"
  }'
```

**Respuesta JSend (201 Created):**

```json
{
  "status": "success",
  "data": {
    "id": 1,
    "titulo": "Clean Code",
    "autor": "Robert C. Martin",
    "anio": 2008,
    "genero": "Software"
  }
}
```

#### Obtener información de la API

```bash
curl "https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net/"
```

---

## 📄 Formato de Respuestas (JSend)

Todas las respuestas de la API siguen el estándar [JSend](https://github.com/omniti-labs/jsend):

### Success (2xx)

```json
{
  "status": "success",
  "data": { ... }
}
```

### Fail - Validación (4xx)

```json
{
  "status": "fail",
  "data": {
    "titulo": "El título es requerido",
    "anio": "El año debe estar entre 1500 y 2025"
  }
}
```

### Error - Servidor (5xx)

```json
{
  "status": "error",
  "message": "Error interno del servidor",
  "code": 500
}
```

---

## 🧪 Testing y Calidad

### Tests Implementados

| Tipo        | Proyecto   | Framework           | Cobertura |
| ----------- | ---------- | ------------------- | --------- |
| Unitarios   | Controller | NUnit + Moq         | >70%      |
| Unitarios   | Service    | NUnit + Moq         | >70%      |
| Integración | Repository | NUnit + InMemory DB | >70%      |

### Ejecutar Tests Localmente

```bash
# Ejecutar todos los tests
dotnet test APINet.sln

# Con reporte de cobertura
dotnet test APINet.sln --collect:"XPlat Code Coverage"

# Con reporte HTML
dotnet test APINet.sln --collect:"XPlat Code Coverage"
reportgenerator -reports:TestResults/**/coverage.cobertura.xml -targetdir:TestResults/CoverageReport -reporttypes:Html
```

### Code Coverage en CI/CD

Cada push a master genera automáticamente:

- ✅ Test Results (.trx)
- ✅ Coverage Report (HTML)
- ✅ Artifacts descargables en GitHub Actions

---

## 👨‍💻 Arquitectura del Proyecto

### Clean Architecture - Capas

```
┌─────────────────────────────────────────┐
│         Controllers (API Layer)         │  ← Endpoints HTTP
│  - LibrosController                     │
│  - HomeController                       │
└─────────────────────────────────────────┘
              ↓ (DTOs)
┌─────────────────────────────────────────┐
│      Services (Business Logic)          │  ← Lógica de negocio
│  - LibroService                         │
│  - Validaciones de duplicados           │
└─────────────────────────────────────────┘
              ↓ (Models)
┌─────────────────────────────────────────┐
│    Repositories (Data Access)           │  ← Acceso a datos
│  - LibroRepository                      │
│  - Entity Framework Core                │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│         Azure SQL Database              │  ← Persistencia
└─────────────────────────────────────────┘
```

### Estructura de Directorios

```
APINet/
├── APINet/                        # Proyecto principal
│   ├── Controllers/               # API Endpoints
│   │   ├── LibrosController.cs
│   │   └── HomeController.cs
│   ├── Service/                   # Business Logic
│   │   ├── ILibroService.cs
│   │   └── LibroService.cs
│   ├── Repositories/              # Data Access
│   │   ├── ILibroRepository.cs
│   │   └── LibroRepository.cs
│   ├── Models/                    # Domain Models
│   │   ├── Libro.cs
│   │   ├── JSendResponse.cs
│   │   └── PagedResult.cs
│   ├── DTOs/                      # Data Transfer Objects
│   │   ├── CreateLibroDto.cs
│   │   ├── UpdateLibroDto.cs
│   │   └── LibroResponseDto.cs
│   ├── Validators/                # FluentValidation
│   │   ├── CreateLibroValidator.cs
│   │   └── UpdateLibroValidator.cs
│   ├── Mappings/                  # AutoMapper Profiles
│   │   └── LibroMappingProfile.cs
│   ├── Middleware/                # Custom Middleware
│   │   └── GlobalExceptionHandler.cs
│   ├── Data/                      # EF Core DbContext
│   │   └── AppDbContext.cs
│   ├── .env                       # Variables de entorno (local)
│   ├── .env.example               # Plantilla
│   └── Program.cs                 # Startup & Configuration
│
├── Test/                          # Proyecto de Tests
│   ├── GlobalExceptionHandlerTests.cs
│   ├── HomeControllerTests.cs
│   ├── LibroRepositoryTests.cs
│   ├── LibrosControllerTests.cs
│   ├── LibroServiceTests.cs
│   ├── MappingTests.cs
│   └── ValidatorsTests.cs
│
└── .github/
    └── workflows/
        └── master_api-net.yml     # CI/CD Pipeline
```

---

## 🔧 Características Técnicas Destacadas

### 1. JSend Standard

- Respuestas HTTP estandarizadas
- 3 tipos: `success`, `fail`, `error`
- Facilita integración con frontend

### 2. Paginación

- Parámetros: `?page=1&pageSize=10`
- Metadatos: totalCount, totalPages, hasNext, hasPrevious
- Performance optimizada para grandes datasets

### 3. Exception Handling Global

- Middleware centralizado
- Mapeo de excepciones a códigos HTTP
- Respuestas JSend consistentes

### 4. Validaciones en Capas

- **FluentValidation**: Validaciones de entrada (Controller)
- **Service**: Lógica de negocio (duplicados, reglas complejas)

### 5. Seguridad

- Rate Limiting (100 req/min)
- CORS configurado
- Variables de entorno protegidas
- HTTPS enforced

---

## 📊 Monitoreo en Producción

### Health Checks

```bash
curl https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net/health
```

### Application Insights (Azure)

- Disponibilidad
- Performance
- Excepciones
- Request rate

### Logs

- **Serilog** configurado
- Output: Console + Archivos
- Logs estructurados en JSON

---

## 🎓 Proyecto Académico

Este proyecto fue desarrollado como parte del curso de **Calidad de Software** en la Universidad Peruana del Norte (UPN), demostrando:

✅ Clean Architecture y SOLID
✅ Testing automatizado (>70% coverage)
✅ CI/CD con GitHub Actions
✅ Cloud deployment en Azure
✅ API RESTful con mejores prácticas
✅ Documentación técnica completa

---

## 👤 Autor

**Erick**
📧 n00340097@upn.pe
🎓 Universidad Peruana del Norte (UPN)
📅 2024

---

## 🔗 Enlaces Útiles

- **API en Producción**: https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net
- **Swagger UI**: https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net/swagger/index.html
- **Repositorio GitHub**: [YamiDarknezz/api-net](https://github.com/YamiDarknezz)
- **JSend Specification**: https://github.com/omniti-labs/jsend

---

⭐ **Proyecto Académico - UPN 2025** ⭐

_Desarrollado con .NET 8, Azure, GitHub Actions y las mejores prácticas de Calidad de Software_
