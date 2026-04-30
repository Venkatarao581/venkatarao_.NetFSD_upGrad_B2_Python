=======================================================
   EMPLOYEE MANAGEMENT SYSTEM - MINI PROJECT 2
   Student: P. Venkatarao
   Date: April 2026
=======================================================

1. HOW TO RUN THE APP
-----------------------
Step 1: Double-click index.html
Step 2: Login with:
        Username : Venkatarao
        Password : Venkat@123

2. HOW TO RUN THE TESTS
------------------------
Step 1:cd EMS.API ---- VS code 
       dotnet run
Step 2: cd EMS.API ----VS code 
        dotnet test
Expected Result:
        Tests: 15 passed, 15 total
Publish Result:
        dotnet publish -c Release -o ./publish
        cd publish
        dotnet EMS.API.dll

## Student Details

| Field        | Details                          |
|--------------|----------------------------------|
| Name         | P. VENKATARAO                    |
| Project      | Mini Project 2 — EMS Full Stack  |
| Technology   | .NET 8 Web API + SQL Server 2021 |
| Frontend     | HTML5, CSS3, Bootstrap 5, jQuery |
| Tests        | NUnit + Moq (15/15 Passed )    |

---

##  Project Overview

Mini Project 2 extends the Mini Project 1 frontend (Employee Management Dashboard)
into a true full-stack application by replacing the static in-memory data layer with
a real .NET 8 Web API backed by SQL Server 2022, managed through Entity Framework
Core (Code First). The frontend architecture from Mini Project 1 required zero
structural changes — only storageService.js and authService.js changed.

---

##  Technology Stack

| Layer              | Technology                        | Version     |
|--------------------|-----------------------------------|-------------|
| Backend Framework  | .NET Web API                      | 8.0 (LTS)   |
| ORM                | Entity Framework Core             | Code First  |
| Database           | SQL Server                        | 2021 (local)|
| Password Hashing   | BCrypt.Net-Next                   | 4.0.3       |
| API Documentation  | Swagger / Swashbuckle             | OpenAPI 3.0 |
| Authentication     | JWT Bearer Token                  | 8.0.0       |
| Frontend           | HTML5, CSS3, Bootstrap 5          | No changes  |
| Frontend JS        | Vanilla JS (ES6+), jQuery 3.x     | Same as MP1 |
| Unit Testing       | NUnit + Moq                       | 4.x         |

---

##  Project Structure

```
Employee Management System/
│
├── EMS.API/                          ← .NET 8 Web API (Backend)
│   ├── Controllers/
│   │   ├── EmployeesController.cs    ← CRUD + dashboard + pagination
│   │   └── AuthController.cs         ← /api/auth/register + /api/auth/login
│   ├── Data/
│   │   └── AppDbContext.cs           ← EF Core DbContext + seed data
│   ├── DTOs/
│   │   └── EmployeeDtos.cs           ← Request/Response/Paged/Auth DTOs
│   ├── Models/
│   │   ├── Employee.cs               ← Employee entity
│   │   └── AppUser.cs                ← AppUser entity (Admin/Viewer)
│   ├── Services/
│   │   ├── IEmployeeRepository.cs    ← Data access interface (enables Moq)
│   │   ├── EmployeeRepository.cs     ← EF Core implementation
│   │   ├── EmployeeService.cs        ← Business logic + SQL filter/sort/page
│   │   └── AuthService.cs            ← BCrypt hashing + JWT generation
│   ├── appsettings.json              ← Connection string + JWT config
│   ├── Program.cs                    ← DI, CORS, Swagger, JWT middleware
│   └── EMS.API.csproj
│
├── EMS.Tests/                        ← NUnit + Moq Backend Tests
│   ├── Services/
│   │   ├── EmployeeServiceTests.cs   ← 8 unit tests for EmployeeService
│   │   └── AuthServiceTests.cs       ← 7 unit tests for AuthService
│   └── EMS.Tests.csproj
│
└── frontend/                         ← Updated Mini Project 1 Frontend
    ├── index.html                    ← Single page (all views + modals)
    ├── css/
    │   └── styles.css                ← Custom styles + pagination
    └── js/
        ├── config.js                 ← NEW: API_BASE_URL constant
        ├── storageService.js         ← REPLACED: real fetch() API calls
        ├── authService.js            ← UPDATED: JWT token management
        ├── employeeService.js        ← UPDATED: async/await delegates
        ├── validationService.js      ← MINOR: mapServerErrors() added
        ├── dashboardService.js       ← UPDATED: single API call
        ├── uiService.js              ← UPDATED: pagination + role UI
        └── app.js                    ← UPDATED: async/await + pagination
```

---

##  Database Design

### Employee Table
| Column      | C# Type  | SQL Type        | Constraints                              |
|-------------|----------|-----------------|------------------------------------------|
| Id          | int      | INT IDENTITY    | Primary Key, auto-increment              |
| FirstName   | string   | NVARCHAR(100)   | Required                                 |
| LastName    | string   | NVARCHAR(100)   | Required                                 |
| Email       | string   | NVARCHAR(200)   | Required, Unique index                   |
| Phone       | string   | NVARCHAR(15)    | Required, 10-digit validated by API      |
| Department  | string   | NVARCHAR(50)    | Required                                 |
| Designation | string   | NVARCHAR(100)   | Required                                 |
| Salary      | decimal  | DECIMAL(18,2)   | Required, must be positive               |
| JoinDate    | DateTime | DATETIME2       | Required                                 |
| Status      | string   | NVARCHAR(10)    | 'Active' or 'Inactive'                   |
| CreatedAt   | DateTime | DATETIME2       | Auto-set on insert                       |
| UpdatedAt   | DateTime | DATETIME2       | Auto-updated on every PUT                |

### AppUser Table
| Column       | C# Type  | SQL Type       | Constraints                              |
|--------------|----------|----------------|------------------------------------------|
| Id           | int      | INT IDENTITY   | Primary Key                              |
| Username     | string   | NVARCHAR(100)  | Required, Unique index                   |
| PasswordHash | string   | NVARCHAR(MAX)  | BCrypt hash — never plain text           |
| Role         | string   | NVARCHAR(20)   | 'Admin' or 'Viewer'                      |
| CreatedAt    | DateTime | DATETIME2      | Auto-set on registration                 |

### Seed Data (auto-applied via HasData migration)
- **15 employees** across 5 departments (Engineering, Marketing, HR, Finance, Operations)
- **2 default users:**
  - `admin` / `admin123` → Role: Admin (full CRUD)
  - `viewer` / `viewer123` → Role: Viewer (read-only)

---

##  REST API Endpoints

### Authentication Endpoints
| Method | Endpoint             | Auth Required | Description                            |
|--------|----------------------|---------------|----------------------------------------|
| POST   | /api/auth/register   | None          | Register new Admin or Viewer account   |
| POST   | /api/auth/login      | None          | Login, returns JWT token + role        |

### Employee Endpoints
| Method | Endpoint                    | Auth Required  | Description                        |
|--------|-----------------------------|----------------|------------------------------------|
| GET    | /api/employees              | Admin + Viewer | Paginated, filtered, sorted list   |
| GET    | /api/employees/dashboard    | Admin + Viewer | KPIs + dept breakdown + recent 5   |
| GET    | /api/employees/{id}         | Admin + Viewer | Single employee by ID              |
| POST   | /api/employees              | Admin only     | Create new employee                |
| PUT    | /api/employees/{id}         | Admin only     | Update existing employee           |
| DELETE | /api/employees/{id}         | Admin only     | Delete employee                    |

### Query Parameters for GET /api/employees
| Parameter  | Type   | Default | Description                              |
|------------|--------|---------|------------------------------------------|
| search     | string | (none)  | Case-insensitive LIKE on name + email    |
| department | string | (none)  | Exact match filter                       |
| status     | string | (none)  | Active or Inactive                       |
| sortBy     | string | name    | name, salary, or joindate                |
| sortDir    | string | asc     | asc or desc                              |
| page       | int    | 1       | 1-based page number                      |
| pageSize   | int    | 10      | Records per page (max 100)               |

---

##  JWT Authentication Flow

1. Client sends `POST /api/auth/login` with username + password
2. API validates password via `BCrypt.Verify()`
3. On success, API generates signed JWT containing userId, username, role
4. Frontend stores JWT **in-memory** (never localStorage — XSS risk)
5. Every subsequent API request sends `Authorization: Bearer <token>`
6. ASP.NET Core JWT middleware validates token on every request automatically
7. `[Authorize(Roles = "Admin")]` returns 403 for Viewer JWT on write endpoints

---

##  Unit Tests — Results

```
Test summary: total: 15, failed: 0, succeeded: 15 
```

### EmployeeServiceTests (8 tests — NUnit + Moq)
| Test Name                                  | Result |
|--------------------------------------------|--------|
| GetByIdAsync_ValidId_ReturnsMappedDto       |  Pass |
| GetByIdAsync_InvalidId_ReturnsNull          |  Pass |
| AddAsync_UniqueEmail_CreatesEmployee        |  Pass |
| AddAsync_DuplicateEmail_ReturnsNull         |  Pass |
| UpdateAsync_ValidId_ReturnsUpdatedDto       |  Pass |
| UpdateAsync_NotFound_ReturnsNull            |  Pass |
| DeleteAsync_ValidId_ReturnsTrue             |  Pass |
| DeleteAsync_InvalidId_ReturnsFalse          |  Pass |

### AuthServiceTests (7 tests — NUnit + Moq + InMemory EF Core)
| Test Name                                       | Result |
|-------------------------------------------------|--------|
| RegisterAsync_NewUser_ReturnsSuccessWithToken    |  Pass |
| RegisterAsync_DuplicateUsername_ReturnsFailure   | Pass |
| LoginAsync_ValidCredentials_ReturnsToken         |  Pass |
| LoginAsync_WrongPassword_ReturnsFailure          |  Pass |
| LoginAsync_NonExistentUser_ReturnsFailure        |  Pass |
| GenerateToken_ValidUser_ReturnsNonEmptyString    |  Pass |
| RegisterAsync_ShortPassword_IsHandledByCaller    |  Pass |

---

##  Prerequisites

| Tool              | Version      | Download Link                            |
|-------------------|--------------|------------------------------------------|
| .NET SDK          | 8.0+         | https://dotnet.microsoft.com/download    |
| SQL Server        | 2021 Express | Microsoft website (free)                 |
| dotnet-ef CLI     | Latest       | `dotnet tool install -g dotnet-ef`       |
| VS Code           | Any          | https://code.visualstudio.com            |
---

##  How to Run the Application

### Step 1 — Clone / Extract the Project
Extract the ZIP to your desired location, e.g.:
```
C:\Users\YourName\Employee Management System\
```

### Step 2 — Configure the Database Connection
Open `EMS.API/appsettings.json` and verify:
```json
"DefaultConnection": "Server=localhost;Database=EMSDashboard;Trusted_Connection=True;TrustServerCertificate=True;"
```
For named SQL Server instance (e.g. SQLEXPRESS):
```json
"Server=localhost\\SQLEXPRESS;Database=EMSDashboard;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Step 3 — Install dotnet-ef Tool (first time only)
```bash
dotnet tool install --global dotnet-ef
```

### Step 4 — Restore NuGet Packages
```bash
cd EMS.API
dotnet restore
```

### Step 5 — Run Database Migrations (REQUIRED — first time only)
```bash
cd EMS.API
dotnet ef database update
```
This creates the `EMSDashboard` database, both tables, and seeds all 15 employees
and 2 user accounts automatically.

### Step 6 — Start the API
```bash
cd EMS.API
dotnet run
```
Expected output:
```
info: Now listening on:  http://localhost:59723
info: Application started. Press Ctrl+C to shut down.
```

### Step 7 — Open Swagger UI (optional — API testing)
Open browser and navigate to:
```
http:// http://localhost:59723/swagger
```

### Step 8 — Open the Frontend
Open VS Code → Open the `frontend` folder → right-click `index.html`
→ **"Open with Live Server"**

Frontend opens at: `http:// http://localhost:59723/index.html`

### Step 9 — Run NUnit Tests
Open a second terminal:
```bash
cd EMS.Tests
dotnet restore
dotnet test --verbosity normal
```

---

##  Default Login Credentials

| Username | Password  | Role   | Access Level              |
|----------|-----------|--------|---------------------------|
|Venkatarao| Venkat@123| Admin  | Full CRUD on all employees|


---

##  Features Implemented

### Backend
- [x] .NET 8 Web API with clean layered architecture
- [x] Entity Framework Core Code First with migrations
- [x] SQL Server 2022 with seed data (15 employees + 2 users)
- [x] BCrypt password hashing (12 rounds) — never stores plain text
- [x] JWT Bearer token authentication
- [x] Role-based authorization (Admin / Viewer)
- [x] Server-side search, filter, sort, and pagination (SQL Skip/Take)
- [x] Swagger / OpenAPI documentation with JWT Bearer support
- [x] CORS configured for Live Server (port 5500)
- [x] Repository pattern (IEmployeeRepository) for testability
- [x] 409 Conflict on duplicate email / username

### Frontend
- [x] Login and Signup pages with validation
- [x] JWT token stored in-memory (not localStorage)
- [x] Role-based UI (Admin/Viewer badge, hide write buttons for Viewer)
- [x] Dashboard with 4 KPI cards, department breakdown, recent employees
- [x] Employee list with search, filter, sort, and pagination
- [x] Add, Edit, View, Delete employees via Bootstrap modals
- [x] Bootstrap Toast notifications for all CRUD operations
- [x] Responsive layout (desktop, tablet, mobile)
- [x] 350ms debounced search

### Testing
- [x] 15 NUnit unit tests — all passing (0 failures)
- [x] Moq used to mock IEmployeeRepository (no real DB in unit tests)
- [x] In-memory EF Core used for AuthService integration tests
- [x] Happy path + failure/edge case per service

---

##  Architecture — Service Interaction Flow

```
app.js (orchestrator)
    │
    ├── AuthService.js      → POST /api/auth/login
    │                          POST /api/auth/register
    │
    ├── StorageService.js   → All fetch() calls with Authorization: Bearer token
    │       │
    │       ├── GET  /api/employees          (paginated list)
    │       ├── GET  /api/employees/dashboard
    │       ├── GET  /api/employees/{id}
    │       ├── POST /api/employees
    │       ├── PUT  /api/employees/{id}
    │       └── DELETE /api/employees/{id}
    │
    ├── EmployeeService.js  → delegates to StorageService (async/await)
    ├── DashboardService.js → delegates to StorageService.getDashboard()
    ├── ValidationService.js → client-side + server error mapping
    └── UIService.js        → all DOM rendering (table, cards, modals, pagination)