# TaskFlow — Sistema Interno de Gestión de Tareas

**TaskFlow** es una aplicación web interna para administrar el trabajo de un equipo. Permite registrar usuarios, crear tareas, asignarlas a un colaborador y seguir su avance mediante tres estados: **Pendiente**, **En progreso** y **Completada**. Incluye filtros por usuario y estado, orden por fecha de creación y validaciones de negocio en el backend y en la interfaz.

> **Desarrollado por:** Ing. GAAE
> **Repositorio público:** <https://github.com/galzate/taskflow>
> **Contexto:** Prueba técnica Fullstack Semisenior · noviembre de 2026

| Capa | Tecnología |
|---|---|
| Backend | .NET 10 Web API (C#) · EF Core 10 · xUnit · Scalar (OpenAPI) |
| Frontend | Angular 22 (standalone components, signals, reactive forms) · TypeScript · Vitest |
| Base de datos | Microsoft SQL Server 2022 (contenedor Docker) |
| Arquitectura | Controller → Service → Repository · DTOs · SOLID |

> **Idioma:** el código fuente, los identificadores y el modelo de datos están en inglés. La interfaz de usuario y los mensajes visibles están en español; los estados se muestran como **Pendiente**, **En progreso** y **Completada**, conservando `Pending`, `InProgress` y `Done` en la API y la base de datos.

---

## 1. Descripción del sistema

TaskFlow centraliza la gestión de tareas de un equipo interno:

1. **Usuarios:** se registran con nombre y correo electrónico único.
2. **Tareas:** se crean con título, descripción y un usuario asignado obligatorio. Opcionalmente aceptan metadata JSON (prioridad, etiquetas, fecha límite, etc.).
3. **Seguimiento:** cada tarea avanza por los estados `Pending` → `InProgress` → `Done`, con reglas que impiden saltos inválidos.
4. **Consulta:** el listado se puede filtrar por usuario y por estado, y ordenar por fecha de creación (ascendente o descendente).

La interfaz está completamente en español, con cabecera fija, mensajes de éxito/error que se cierran solos a los 2 segundos y desplegables propios que muestran las opciones justo debajo del control.

### Reglas de negocio implementadas

1. El **título** y la **descripción** de la tarea son obligatorios.
2. Toda tarea debe tener un **usuario asignado existente**.
3. **No se permite pasar directamente de `Pending` a `Done`** (HTTP 422).
4. `Done` tampoco puede volver directamente a `Pending`; se debe reabrir pasando por `InProgress`.
5. Cambiar al mismo estado es idempotente (HTTP 200).

Transiciones permitidas:

```
Pending ──► InProgress ──► Done
   ▲             │            │
   └─────────────┘            │
                 ◄────────────┘
```

---

## 2. Tecnologías utilizadas

| Capa | Tecnología | Uso |
|---|---|---|
| Backend | .NET 10 / ASP.NET Core Web API | Endpoints REST, validación, inyección de dependencias |
| Backend | EF Core 10 | Acceso a datos y mapeo del modelo |
| Backend | xUnit | Pruebas unitarias de las reglas de negocio |
| Backend | Scalar + OpenAPI | Documentación interactiva de la API |
| Frontend | Angular 22 | SPA con componentes standalone y `signals` |
| Frontend | Reactive Forms | Formularios de usuario y de tarea |
| Frontend | Vitest | Pruebas de humo del frontend |
| Base de datos | SQL Server 2022 | Modelo relacional, índices, JSON nativo |
| Infraestructura | Docker Compose | SQL Server reproducible en local |
| Control de versiones | Git + GitHub | Repositorio público del proyecto |

Versiones probadas: .NET SDK 10.0.404 · Angular CLI 22.2.6 · Node.js 26 · npm 11 · Docker Desktop 4.x.

---

## 3. Funcionalidades

### Usuarios

- Crear usuarios (nombre + email único).
- Listar usuarios.
- Consultar un usuario por id.

### Tareas

- Crear tareas (título y descripción obligatorios, usuario asignado obligatorio).
- Listar tareas con filtros por **usuario** y **estado**, ordenadas por **fecha de creación** (`asc` / `desc`).
- Cambiar el estado de una tarea: `Pending`, `InProgress`, `Done`.
- Metadata JSON opcional por tarea (prioridad, tags, fecha límite, etc.).

### Interfaz

- Cabecera fija: se mantiene visible mientras el resto del contenido se desplaza.
- Mensajes de éxito y error en español que se cierran automáticamente a los 2 segundos.
- Desplegables propios que muestran las opciones justo debajo del control, con navegación por teclado (↑, ↓, Inicio, Fin, Enter, Esc).
- Contador resumen del listado: `1 tarea - 0 pendientes - 0 en progreso - 1 completada`.
- Prioridad por defecto **Alta** al abrir o reiniciar el formulario de tarea.

---

## 4. Estructura del repositorio

```
app_taskflow/
├── backend/
│   ├── TaskFlow.slnx
│   ├── src/TaskFlow.Api/
│   │   ├── Controllers/     # Endpoints REST (Users, Tasks)
│   │   ├── Services/        # Reglas de negocio + interfaces
│   │   ├── Repositories/    # Acceso a datos con EF Core + interfaces
│   │   ├── DTOs/            # Contratos de entrada/salida
│   │   ├── Entities/        # User, TaskItem
│   │   ├── Enums/           # TaskItemStatus
│   │   ├── Data/            # DbContext + configuraciones EF Core
│   │   ├── Exceptions/      # Excepciones de dominio
│   │   ├── Middleware/      # Manejo global de errores (ProblemDetails)
│   │   ├── Properties/      # launchSettings.json
│   │   └── Program.cs       # Bootstrap, DI, CORS, OpenAPI/Scalar
│   └── tests/TaskFlow.Api.Tests/  # Pruebas unitarias (xUnit)
├── frontend/
│   ├── src/app/
│   │   ├── core/            # modelos, servicios HTTP, configuración
│   │   ├── shared/          # componentes reutilizables (select, alert)
│   │   └── features/
│   │       ├── users/       # formulario de usuarios
│   │       └── tasks/       # formulario y lista de tareas
│   └── README.md            # referencia al README principal
├── database/
│   ├── 01_schema.sql        # Tablas, PK, FK, CHECK constraints
│   ├── 02_indexes.sql       # Índices
│   ├── 03_queries.sql       # Consulta obligatoria (usuario + estado + orden)
│   ├── 04_json_queries.sql  # JSON nativo de SQL Server
│   ├── 05_seed.sql          # Datos demo
│   └── init-db.sh           # Ejecuta 01 + 02 + 05 en el contenedor
├── docker-compose.yml       # SQL Server 2022
├── .gitignore
└── README.md
```

---

## 5. Requisitos previos

| Herramienta | Versión recomendada |
|---|---|
| Docker Desktop | 4.x con Compose v2 |
| .NET SDK | 10.0 |
| Node.js | 20+ (probado con 26) |
| npm | 10+ (probado con 11) |

> **macOS con Apple Silicon (M1–M4):** SQL Server no tiene build nativo para ARM64. La imagen oficial es `linux/amd64` y se ejecuta con **emulación Rosetta 2**. En Docker Desktop activa:
> `Settings → General → “Use Rosetta for x86_64/amd64 emulation on Apple Silicon”`.
> Con Rosetta el contenedor arranca en ~10–20 segundos.

---

## 6. Puesta en marcha

### 6.1 Base de datos (SQL Server 2022)

```bash
# 1) Levantar el contenedor y esperar a que el healthcheck esté en verde
docker compose up -d --wait

# 2) Crear el esquema, los índices y los datos demo
./database/init-db.sh
```

> `init-db.sh` reintenta la conexión hasta que SQL Server responde, así que también funciona si el contenedor tarda en arrancar (por ejemplo, con emulación Rosetta).

Si prefieres iniciar con la base **sin datos demo**, ejecuta solo el esquema y los índices:

```bash
docker exec -i taskflow-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'TaskFlow@2026!' -C -b -i /scripts/01_schema.sql
docker exec -i taskflow-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'TaskFlow@2026!' -C -b -i /scripts/02_indexes.sql
```

Credenciales locales (definidas en `docker-compose.yml` y `appsettings.Development.json`):

```
Servidor : localhost,1433
Usuario  : sa
Password : TaskFlow@2026!
Base     : TaskFlowDb
```

### 6.2 Backend (.NET Web API)

```bash
cd backend/src/TaskFlow.Api
dotnet restore
dotnet run
```

- API: <http://localhost:5080>
- Documentación interactiva (Scalar): <http://localhost:5080/scalar/v1>
- OpenAPI JSON: <http://localhost:5080/openapi/v1.json>

La cadena de conexión está en `appsettings.Development.json`. También puede sobreescribirse con la variable de entorno:

```bash
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=TaskFlowDb;User Id=sa;Password=TaskFlow@2026!;TrustServerCertificate=True;Encrypt=True;"
```

### 6.3 Frontend (Angular)

```bash
cd frontend
npm install
npm start
```

Aplicación: <http://localhost:4200>

La URL base de la API se configura en `frontend/src/app/core/config/api.config.ts`.

### 6.4 Orden recomendado

1. Base de datos → 2. Backend → 3. Frontend. Con los tres servicios activos, abre <http://localhost:4200>, crea un usuario y luego una tarea asignada a ese usuario.

---

## 7. Endpoints de la API

| Método | Ruta | Descripción | Códigos |
|---|---|---|---|
| `POST` | `/api/users` | Crea un usuario | `201`, `400`, `422` |
| `GET` | `/api/users` | Lista usuarios | `200` |
| `GET` | `/api/users/{id}` | Usuario por id | `200`, `404` |
| `POST` | `/api/tasks` | Crea una tarea asignada | `201`, `400`, `404` |
| `GET` | `/api/tasks` | Lista tareas (`userId`, `status`, `sort=asc\|desc`) | `200` |
| `GET` | `/api/tasks/{id}` | Tarea por id | `200`, `404` |
| `PUT` | `/api/tasks/{id}/status` | Cambia el estado | `200`, `400`, `404`, `422` |

Ejemplos:

```bash
# Crear usuario
curl -X POST http://localhost:5080/api/users \
  -H 'Content-Type: application/json' \
  -d '{"name":"Ana Torres","email":"ana.torres@company.com"}'

# Crear tarea con descripción y metadata JSON
curl -X POST http://localhost:5080/api/tasks \
  -H 'Content-Type: application/json' \
  -d '{"title":"Prepare report","description":"Consolidate the monthly KPIs","assignedUserId":1,"metadata":{"priority":"High","tags":["finance"]}}'

# Filtrar tareas por estado y ordenar por fecha ascendente
curl "http://localhost:5080/api/tasks?status=Pending&sort=asc"

# Cambiar estado (Pending -> InProgress)
curl -X PUT http://localhost:5080/api/tasks/1/status \
  -H 'Content-Type: application/json' \
  -d '{"status":"InProgress"}'
```

Los errores de negocio y validación se devuelven como **ProblemDetails** (`application/problem+json`):

```json
{
  "title": "Regla de negocio incumplida",
  "status": 422,
  "detail": "Una tarea no puede pasar directamente de Pendiente a Completada. Primero muévela a En progreso.",
  "instance": "/api/tasks/7/status"
}
```

---

## 8. Base de datos

### 8.1 Modelo

**Users**

| Columna | Tipo | Restricción |
|---|---|---|
| `Id` | `int identity` | **PK** `PK_Users` |
| `Name` | `nvarchar(100)` | `NOT NULL` |
| `Email` | `nvarchar(150)` | `NOT NULL`, **UNIQUE** `UQ_Users_Email` |
| `CreatedAt` | `datetime2(0)` | `DEFAULT SYSUTCDATETIME()` |

**Tasks**

| Columna | Tipo | Restricción |
|---|---|---|
| `Id` | `int identity` | **PK** `PK_Tasks` |
| `Title` | `nvarchar(200)` | `NOT NULL` |
| `Description` | `nvarchar(1000)` | `NOT NULL` |
| `Status` | `varchar(20)` | `NOT NULL`, `CHECK IN ('Pending','InProgress','Done')` |
| `AssignedUserId` | `int` | `NOT NULL`, **FK** `FK_Tasks_Users → Users(Id)` |
| `Metadata` | `nvarchar(max)` | `NULL`, `CHECK ISJSON(Metadata) = 1` |
| `CreatedAt` | `datetime2(0)` | `DEFAULT SYSUTCDATETIME()` |
| `UpdatedAt` | `datetime2(0)` | `NULL` |

### 8.2 Índices

| Índice | Tabla | Columnas |
|---|---|---|
| `PK_Users` / `PK_Tasks` | Users / Tasks | `Id` (clustered) |
| `UQ_Users_Email` | Users | `Email` (unique) |
| `IX_Tasks_AssignedUserId_Status_CreatedAt` | Tasks | `(AssignedUserId, Status, CreatedAt DESC)` + INCLUDE |
| `IX_Tasks_Status_CreatedAt` | Tasks | `(Status, CreatedAt DESC)` + INCLUDE |

### 8.3 Consulta obligatoria

En `database/03_queries.sql`:

```sql
SELECT t.Id, t.Title, t.Status, t.AssignedUserId, u.Name AS AssignedUserName,
       t.Metadata, t.CreatedAt, t.UpdatedAt
FROM dbo.Tasks AS t
INNER JOIN dbo.Users AS u ON u.Id = t.AssignedUserId
WHERE t.AssignedUserId = @assignedUserId
  AND (@status IS NULL OR t.Status = @status)
ORDER BY
    CASE WHEN @sortDirection = 'ASC'  THEN t.CreatedAt END ASC,
    CASE WHEN @sortDirection = 'DESC' THEN t.CreatedAt END DESC;
```

Cubre los tres requisitos: **tareas por usuario**, **filtro por estado** y **orden por fecha de creación**.

### 8.4 JSON nativo de SQL Server

En `database/04_json_queries.sql` se demuestran las funciones nativas sobre `Tasks.Metadata`:

| Función | Uso |
|---|---|
| `ISJSON` | Valida que el contenido sea JSON (también con `CK_Tasks_Metadata_IsJson`) |
| `JSON_VALUE` | Extrae valores escalares (`$.priority`, `$.dueDate`) |
| `OPENJSON` | Convierte el arreglo `$.tags` en filas |
| `FOR JSON PATH` | Devuelve el resultado como documento JSON |

La API acepta y devuelve la metadata como objeto JSON, y la almacena validada en la columna `Metadata`.

---

## 9. Pruebas automatizadas

### Backend (xUnit)

```bash
cd backend
dotnet test
```

Cobertura actual (9 pruebas):

- `Pending → Done` directo lanza `BusinessRuleException`.
- `Pending → InProgress` y `InProgress → Done` actualizan el estado.
- Un valor numérico fuera del enum lanza `ValidationException`.
- Una tarea inexistente lanza `NotFoundException`.
- Crear una tarea sin descripción es inválido (validación del DTO).
- Crear una tarea con un usuario inexistente lanza `NotFoundException`.
- Unos metadatos que no son un objeto JSON lanzan `ValidationException`.
- Crear una tarea válida inicia en estado `Pending`.

### Frontend (Vitest)

```bash
cd frontend
npm test -- --watch=false
```

Pruebas de humo del componente raíz: creación del componente y render del encabezado `TaskFlow`.

---

## 10. Decisiones técnicas

1. **Arquitectura por capas + SOLID.** `Controller` (HTTP) → `Service` (reglas de negocio) → `Repository` (datos). Cada capa depende de interfaces (`IUserService`, `ITaskRepository`, etc.) registradas con inyección de dependencias, cumpliendo el principio de inversión de dependencias.
2. **DTOs separados de las entidades.** Los contratos de la API no exponen directamente el modelo de EF Core.
3. **Scripts SQL como fuente de verdad del esquema.** El entregable exige un script de creación, así que el esquema se crea con `database/01_schema.sql` y no con migraciones. EF Core se usa solo para consultas y escrituras.
4. **Validación en dos niveles.** `DataAnnotations` en los DTOs (400 automático de `[ApiController]`) y reglas de negocio en los servicios (422 mediante `BusinessRuleException`).
5. **Manejo global de errores.** Middleware que traduce excepciones de dominio a **ProblemDetails** con códigos 400 / 404 / 422 / 500.
6. **Estado como string con CHECK.** Facilita la lectura en la base y en la API; el enum se serializa como texto (`JsonStringEnumConverter`) y EF Core lo mapea con `HasConversion<string>()`.
7. **JSON en base de datos y en la API.** Columna `Metadata nvarchar(max)` validada con `ISJSON`, expuesta como `JsonElement` en los DTOs.
8. **Frontend con Angular moderno.** Componentes standalone, `signals`, formularios reactivos, `HttpClient` con `provideHttpClient()` y manejo de errores basado en ProblemDetails.
9. **CORS acotado** al origen de desarrollo de Angular (`http://localhost:4200`).
10. **Documentación de API con Scalar** sobre el OpenAPI nativo de .NET 10.
11. **Comentarios con el formato solicitado** en código y scripts: `dd-mm-YYYY Construido por: Ing GAAE - Descripción del cambio`.
12. **SQL Server en Docker.** Reproducible en cualquier máquina; en Apple Silicon se ejecuta con emulación Rosetta (documentado arriba).

---

## 11. Funcionalidades pendientes

- Ampliar la cobertura: pruebas de integración con `WebApplicationFactory`, pruebas de componentes/servicios del frontend y pruebas E2E.
- Autenticación y autorización (JWT) y roles.
- Paginación y búsqueda por texto en el listado de tareas.
- Edición y eliminación de usuarios y tareas.
- Actualización de la metadata de una tarea existente.
- Migraciones EF Core como alternativa a los scripts SQL.
- Dockerfiles y `docker compose` completo para backend + frontend.
- Pipeline de CI/CD y análisis estático.
- Internacionalización (ES/EN) de la interfaz.
- Mejoras de accesibilidad (ARIA, navegación por teclado) y modo oscuro.
- Despliegue en un entorno productivo (el proyecto está preparado para ejecución local).

---

## 12. Evidencia de pruebas realizadas

| Caso | Resultado |
|---|---|
| Crear usuario | `201` |
| Email duplicado | `422` |
| Email inválido | `400` con detalle de validación |
| Crear tarea con metadata JSON | `201` |
| `Pending → InProgress` | `200` |
| `InProgress → Done` | `200` |
| Tarea sin título o sin descripción | `400` |
| Usuario inexistente al asignar | `404` |
| `Pending → Done` directo | `422` (regla de negocio) |
| Tarea inexistente | `404` |
| Filtro por estado + orden por fecha | `200` |
| Filtro por usuario | `200` |
| Pruebas unitarias backend (xUnit) | `9/9` correctas |
| Smoke tests frontend (Vitest) | `2/2` correctas |
| `JSON_VALUE` / `OPENJSON` / `FOR JSON` / `ISJSON` | Ejecutados correctamente |

---

## 13. Publicación del repositorio

El proyecto está publicado como repositorio **público** en GitHub:

**<https://github.com/galzate/taskflow>**

### Cómo se publicó

```bash
# 1) Crear en GitHub un repositorio público vacío (sin README ni .gitignore).
# 2) Desde la raíz del proyecto:
git init
git add .
git commit -m "feat: initial TaskFlow fullstack implementation"
git branch -M main
git remote add origin https://github.com/galzate/taskflow.git
git push -u origin main
```

### Cómo publicar una copia propia

1. Crea un repositorio público vacío en GitHub.
2. Clona o copia este proyecto y ejecuta:

```bash
git remote set-url origin https://github.com/<tu-usuario>/<tu-repositorio>.git
git push -u origin main
```

3. Si GitHub pide autenticación, usa un **Personal Access Token** (scope `repo`) o `gh auth login`.
4. No subas archivos locales: `.gitignore` ya excluye `node_modules`, `dist`, `.angular`, `bin`, `obj`, `.env` y archivos del sistema.

> Las credenciales de SQL Server incluidas son de desarrollo local. En un entorno real deben reemplazarse por variables de entorno o un gestor de secretos.

---

## 14. Autor

**Ing. GAAE** — desarrollo completo del sistema (backend, frontend, base de datos, pruebas y documentación).

Prueba técnica Fullstack Semisenior · noviembre de 2026.
