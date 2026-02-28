<p align="center">
  <img src="docs/banner.png" alt="AWM.Service Banner" width="100%">
</p>

<h1 align="center">🏛️ AWM.Service</h1>

<p align="center">
  <strong>Robust Academic Workflow Management Backend</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512bd4?style=for-the-badge&logo=dotnet" alt=".NET 8">
  <img src="https://img.shields.io/badge/Entity%20Framework-Core-512bd4?style=for-the-badge&logo=dotnet" alt="EF Core">
  <img src="https://img.shields.io/badge/Architecture-Clean-brightgreen?style=for-the-badge" alt="Clean Architecture">
  <img src="https://img.shields.io/badge/Status-Development-orange?style=for-the-badge" alt="Status">
</p>

---

## 📖 Overview

**AWM.Service** is a comprehensive backend solution designed for managing academic workflows within educational institutions. Built with **.NET 8** and following **Clean Architecture** principles, it provides a scalable, maintainable, and secure foundation for handling students, staff, academic programs, thesis topics, and the entire defense process.

## ✨ Key Features

- 🎓 **Academic Lifecycle**: Manage Institutes, Departments, and Programs.
- 👥 **User Management**: Unified authentication (JWT) and Role-Based Access Control (RBAC).
- 📝 **Topic Management**: Streamlined application and approval workflow for thesis topics.
- ✅ **Quality Control**: Integrated plagiarism and compliance checks.
- 📅 **Defense Scheduling**: Automated scheduling for pre-defenses and final thesis defenses.
- 🔔 **Real-time Notifications**: Keep students and staff updated with an integrated notification system.

## 🛠️ Tech Stack

- **Framework**: [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Database**: Entity Framework Core with SQL Server/PostgreSQL.
- **Security**: JWT Authentication & Context-Aware Authorization.
- **Documentation**: Swagger/OpenAPI (v1).
- **Mapping**: [Mapster](https://github.com/MapsterMapper/Mapster) for efficient object-to-object mapping.
- **Validation**: [FluentValidation](https://fluentvalidation.net/) for robust input validation.
- **Observation**: Problem Details & Global Exception Handling.

## 🏗️ Architecture

The project follows **Clean Architecture** to ensure separation of concerns and testability:

1.  **Domain**: Core entities, value objects, and business rules.
2.  **Application**: Application logic, interfaces, DTOs, and command/query handlers.
3.  **Infrastructure**: External concerns like Persistence (EF Core), Identity, and File Services.
4.  **WebAPI**: Entry point, Controllers, Middleware, and API configuration.

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (or configured database provider)
- IDE (VS 2022, JetBrains Rider, or VS Code)

### Installation & Run

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/AWM.Service.git
   ```
2. Navigate to the WebAPI directory:
   ```bash
   cd src/AWM.Service.WebAPI
   ```
3. Update `appsettings.json` with your connection string.
4. Run the application:
   ```bash
   dotnet run
   ```
5. Open Swagger at: `https://localhost:[PORT]/swagger`

## 🛣️ API Documentation

Detailed endpoint documentation can be found in our [Endpoint Table](endpoint_table.md).

### Main Controllers

| Controller | Responsibility |
| :--- | :--- |
| `AuthController` | Registration, Login, and Identity Management. |
| `TopicsController` | Management of thesis topics and approvals. |
| `StudentWorksController`| Management of student thesis and projects. |
| `DefenseScheduleController` | Handling of defense slots and assignments. |

---

<p align="center">
  Made with ❤️ for Academic Excellence.
</p>