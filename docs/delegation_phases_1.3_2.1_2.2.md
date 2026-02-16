# 📋 Делегация задач: Фазы 1.3, 2.1, 2.2

> **Дата создания:** 2026-02-08  
> **Проект:** AWM.Service (ETMS v4.0)  
> **Статус:** К выполнению

---

## 📌 Контекст проекта

### О проекте

**AWM.Service** — это бэкенд-сервис для системы управления выпускными квалификационными работами (Electronic Thesis Management System). Архитектура построена на принципах Clean Architecture с CQRS паттерном.

### Структура проекта

```
src/
├── AWM.Service.Domain/        # Доменные сущности, интерфейсы
├── AWM.Service.Application/   # CQRS: Commands, Queries, Handlers, DTOs
├── AWM.Service.Infrastructure/# Реализации репозиториев, EF Core
└── AWM.Service.WebAPI/        # Контроллеры, Contracts, DI
```

### Технологии

- **.NET 8** (ASP.NET Core Web API)
- **Entity Framework Core** (SQL Server)
- **MediatR** (CQRS)
- **FluentValidation** (валидация)
- **ASP.NET Core Identity** (аутентификация)

### Примеры уже реализованного кода

Для референса смотрите уже реализованные компоненты в Фазе 1.1:

- `Features/Org/Commands/Institutes/` — примеры Commands/Handlers
- `Features/Org/Queries/` — примеры Queries
- `Controllers/v1/InstitutesController.cs` — пример контроллера
- `Common/Contracts/` — примеры Request/Response контрактов

---

## 📁 Фаза 1.3: Управление Студентами и Сотрудниками

### Описание

Реализация CRUD операций для профилей студентов и сотрудников (включая научных руководителей). Студенты привязаны к кафедрам и образовательным программам. Сотрудники могут иметь роль научного руководителя.

### Зависимости

- **Требует:** Фаза 1.1 (Institutes, Departments), Фаза 1.2 (AcademicPrograms)
- Необходимо наличие `Department` и `AcademicProgram` в базе данных

### Доменные сущности (уже существуют)

```
Domain/Edu/
├── Student.cs          # Сущность студента
└── Employee.cs         # Сущность сотрудника (Staff)
```

---

### 📂 Application Layer

**Путь:** `src/AWM.Service.Application/Features/Edu/`

#### Commands — Students

| Файл | Описание |
|------|----------|
| `Commands/Students/CreateStudent/CreateStudentCommand.cs` | Record с полями: `UserId`, `DepartmentId`, `AcademicProgramId`, `EnrollmentYear`, `GroupName` |
| `Commands/Students/CreateStudent/CreateStudentCommandHandler.cs` | Создает профиль студента, связывает с User, Department, AcademicProgram |
| `Commands/Students/CreateStudent/CreateStudentCommandValidator.cs` | Валидация: обязательные поля, существование связанных сущностей |
| `Commands/Students/UpdateStudent/UpdateStudentCommand.cs` | Record с полями для обновления |
| `Commands/Students/UpdateStudent/UpdateStudentCommandHandler.cs` | Обновление профиля студента |
| `Commands/Students/UpdateStudent/UpdateStudentCommandValidator.cs` | Валидация обновления |

#### Commands — Staff

| Файл | Описание |
|------|----------|
| `Commands/Staff/CreateStaff/CreateStaffCommand.cs` | Record с полями: `UserId`, `DepartmentId`, `Position`, `IsSupervisor`, `MaxStudents` |
| `Commands/Staff/CreateStaff/CreateStaffCommandHandler.cs` | Создает профиль сотрудника |
| `Commands/Staff/CreateStaff/CreateStaffCommandValidator.cs` | Валидация |
| `Commands/Staff/UpdateStaff/UpdateStaffCommand.cs` | Обновление профиля |
| `Commands/Staff/UpdateStaff/UpdateStaffCommandHandler.cs` | Хендлер обновления |
| `Commands/Staff/UpdateStaff/UpdateStaffCommandValidator.cs` | Валидация |

#### Queries

| Файл | Описание |
|------|----------|
| `Queries/Students/GetStudentById/GetStudentByIdQuery.cs` | Получение студента по ID |
| `Queries/Students/GetStudentsByDepartment/GetStudentsByDepartmentQuery.cs` | Список студентов кафедры (с пагинацией) |
| `Queries/Staff/GetStaffByDepartment/GetStaffByDepartmentQuery.cs` | Список сотрудников кафедры |
| `Queries/Staff/GetSupervisors/GetSupervisorsQuery.cs` | Только НР с фильтром `IsSupervisor = true` |

#### DTOs

| Файл | Описание |
|------|----------|
| `DTOs/StudentDto.cs` | `Id`, `FullName`, `GroupName`, `DepartmentName`, `ProgramName` |
| `DTOs/StaffDto.cs` | `Id`, `FullName`, `Position`, `DepartmentName`, `IsSupervisor` |

---

### 📂 WebAPI Layer

**Путь:** `src/AWM.Service.WebAPI/`

#### Controllers

| Файл | Endpoints |
|------|-----------|
| `Controllers/v1/StudentsController.cs` | `GET /api/v1/students`, `GET /{id}`, `POST`, `PUT /{id}` |
| `Controllers/v1/StaffController.cs` | `GET /api/v1/staff`, `GET /supervisors`, `POST`, `PUT /{id}` |

#### Contracts — Requests

| Файл | Поля |
|------|------|
| `Common/Contracts/Requests/Edu/CreateStudentRequest.cs` | `DepartmentId`, `AcademicProgramId`, `EnrollmentYear`, `GroupName` |
| `Common/Contracts/Requests/Edu/UpdateStudentRequest.cs` | `AcademicProgramId?`, `GroupName?` |
| `Common/Contracts/Requests/Edu/CreateStaffRequest.cs` | `DepartmentId`, `Position`, `IsSupervisor`, `MaxStudents?` |
| `Common/Contracts/Requests/Edu/UpdateStaffRequest.cs` | `Position?`, `IsSupervisor?`, `MaxStudents?` |

#### Contracts — Responses

| Файл | Поля |
|------|------|
| `Common/Contracts/Responses/Edu/StudentResponse.cs` | `Id`, `UserId`, `FullName`, `Email`, `GroupName`, `Department`, `Program` |
| `Common/Contracts/Responses/Edu/StaffResponse.cs` | `Id`, `UserId`, `FullName`, `Email`, `Position`, `Department`, `IsSupervisor` |

---

## 📁 Фаза 2.1: Управление периодами

### Описание

Периоды определяют временные рамки для подачи заявок, выбора тем, предзащит и защит. Каждая кафедра имеет свои периоды. Периоды содержат даты начала и окончания различных этапов.

### Зависимости

- **Требует:** Фаза 1.1 (Departments)

### Доменная сущность (уже существует)

```
Domain/Common/Period.cs   # Сущность периода
```

**Ключевые поля Period:**

- `DepartmentId` — привязка к кафедре
- `TopicSubmissionStart/End` — период подачи тем
- `ApplicationStart/End` — период подачи заявок
- `PreDefenseStart/End` — период предзащит
- `DefenseStart/End` — период защит
- `IsActive` — активен ли период

---

### 📂 Application Layer

**Путь:** `src/AWM.Service.Application/Features/Common/`

#### Commands

| Файл | Описание |
|------|----------|
| `Commands/Periods/CreatePeriod/CreatePeriodCommand.cs` | Все даты периода + `DepartmentId` |
| `Commands/Periods/CreatePeriod/CreatePeriodCommandHandler.cs` | Создание периода, валидация пересечений |
| `Commands/Periods/CreatePeriod/CreatePeriodCommandValidator.cs` | Валидация: даты, логическая последовательность |
| `Commands/Periods/UpdatePeriod/UpdatePeriodCommand.cs` | Обновление дат периода |
| `Commands/Periods/UpdatePeriod/UpdatePeriodCommandHandler.cs` | Хендлер с проверкой состояния |
| `Commands/Periods/UpdatePeriod/UpdatePeriodCommandValidator.cs` | Валидация обновления |

#### Queries

| Файл | Описание |
|------|----------|
| `Queries/Periods/GetPeriodsByDepartment/GetPeriodsByDepartmentQuery.cs` | Все периоды кафедры |
| `Queries/Periods/GetActivePeriod/GetActivePeriodQuery.cs` | Текущий активный период кафедры |

#### Services

| Файл | Описание |
|------|----------|
| `Services/IPeriodValidationService.cs` | Интерфейс (поместить в Domain) |
| `Services/PeriodValidationService.cs` | Проверка допустимости действий в текущем периоде |

#### DTOs

| Файл | Описание |
|------|----------|
| `DTOs/PeriodDto.cs` | Все поля периода для отображения |

---

### 📂 WebAPI Layer

#### Controller

| Файл | Endpoints |
|------|-----------|
| `Controllers/v1/PeriodsController.cs` | `GET /departments/{deptId}/periods`, `GET /active`, `POST`, `PUT /{id}` |

#### Contracts

| Файл | Описание |
|------|----------|
| `Common/Contracts/Requests/Common/CreatePeriodRequest.cs` | Все даты периода |
| `Common/Contracts/Requests/Common/UpdatePeriodRequest.cs` | Обновляемые поля |
| `Common/Contracts/Responses/Common/PeriodResponse.cs` | Полный ответ с датами и статусом |

---

## 📁 Фаза 2.2: Машина состояний (Workflow Engine)

### Описание

Сервис для управления переходами состояний сущностей (темы, заявки, работы). Проверяет допустимость переходов на основе текущего состояния, роли пользователя и бизнес-правил.

### Зависимости

- **Требует:** Понимание состояний из Domain моделей
- Используется всеми последующими фазами (3-6)

### Доменные сущности состояний (уже существуют)

```
Domain/Wf/
├── WorkflowState.cs       # Enum состояний
└── Interfaces/            # Интерфейсы workflow
```

**Примеры состояний:**

- `Draft` → `Submitted` → `Approved` / `Rejected` / `RevisionRequested`

---

### 📂 Application Layer

**Путь:** `src/AWM.Service.Application/Features/Workflow/`

#### Services

| Файл | Описание |
|------|----------|
| `Services/IWorkflowService.cs` | Интерфейс: `CanTransition()`, `Transition()`, `GetAvailableTransitions()` |
| `Services/WorkflowService.cs` | Реализация: матрица переходов, проверка ролей |

#### Commands

| Файл | Описание |
|------|----------|
| `Commands/TransitionState/TransitionStateCommand.cs` | `EntityType`, `EntityId`, `TargetState`, `Comment?` |
| `Commands/TransitionState/TransitionStateCommandHandler.cs` | Валидация перехода, применение, событие |
| `Commands/TransitionState/TransitionStateCommandValidator.cs` | Базовая валидация входных данных |

#### Queries

| Файл | Описание |
|------|----------|
| `Queries/GetAvailableTransitions/GetAvailableTransitionsQuery.cs` | Доступные переходы для сущности и пользователя |

#### DTOs

| Файл | Описание |
|------|----------|
| `DTOs/StateDto.cs` | `Code`, `DisplayName`, `Color` |
| `DTOs/TransitionDto.cs` | `FromState`, `ToState`, `ActionName`, `RequiredRole` |

---

### 📂 Domain Layer

**Путь:** `src/AWM.Service.Domain/Wf/`

| Файл | Описание |
|------|----------|
| `Interfaces/IWorkflowService.cs` | Интерфейс для DI (если еще не создан) |

---

### 📂 WebAPI Layer

#### Contracts

| Файл | Описание |
|------|----------|
| `Common/Contracts/Requests/Workflow/TransitionStateRequest.cs` | `TargetState`, `Comment?` |
| `Common/Contracts/Responses/Workflow/StateResponse.cs` | Информация о состоянии |
| `Common/Contracts/Responses/Workflow/TransitionResponse.cs` | Информация о переходе |

> **Примечание:** Workflow endpoints интегрируются в контроллеры сущностей (например, `DirectionsController`), а не в отдельный контроллер.

---

## ✅ Чеклист выполнения

### Фаза 1.3

- [ ] `CreateStudentCommand` + Handler + Validator
- [ ] `UpdateStudentCommand` + Handler + Validator
- [ ] `CreateStaffCommand` + Handler + Validator
- [ ] `UpdateStaffCommand` + Handler + Validator
- [ ] `GetStudentByIdQuery` + Handler
- [ ] `GetStudentsByDepartmentQuery` + Handler
- [ ] `GetStaffByDepartmentQuery` + Handler
- [ ] `GetSupervisorsQuery` + Handler
- [ ] `StudentsController.cs`
- [ ] `StaffController.cs`
- [ ] Request/Response контракты

### Фаза 2.1

- [ ] `CreatePeriodCommand` + Handler + Validator
- [ ] `UpdatePeriodCommand` + Handler + Validator
- [ ] `GetPeriodsByDepartmentQuery` + Handler
- [ ] `GetActivePeriodQuery` + Handler
- [ ] `IPeriodValidationService` + реализация
- [ ] `PeriodsController.cs`
- [ ] Request/Response контракты

### Фаза 2.2

- [ ] `IWorkflowService` интерфейс
- [ ] `WorkflowService` реализация
- [ ] `TransitionStateCommand` + Handler + Validator
- [ ] `GetAvailableTransitionsQuery` + Handler
- [ ] DTOs: `StateDto`, `TransitionDto`
- [ ] Workflow контракты

---

## 📎 Полезные ссылки

- **Detailed Backlog:** `docs/detailed_backlog.md`
- **Примеры кода:** `src/AWM.Service.Application/Features/Org/`
- **Domain entities:** `src/AWM.Service.Domain/`

---

## ❓ Вопросы для уточнения

изучите реализованные примеры в `Features/Org/` и `Features/Auth/`, `/controllers/v1` и т.д.  
