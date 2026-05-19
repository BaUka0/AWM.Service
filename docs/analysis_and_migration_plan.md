# AWM v6 — Domain Layer + University Integration

## 1. UNIVERSITY FOUNDATION (Read-Only Master)

AWM использует **12 таблиц** из университетской БД. DDL: `docs/DB_scheam_university.sql`.

| # | Таблица | PK | Entity | Зачем |
|---|---------|-----|--------|-------|
| 1 | `Edu_Users` | ID | `University.User` | ФИО, ИИН, Email. Все FK UserId → сюда |
| 2 | `Edu_Students` | StudentID | `University.Student` | Eligibility, GPA, Year |
| 3 | `Edu_Employees` | ID | `University.Employee` | IsAdvisor = куратор |
| 4 | `Edu_EmployeePositions` | ID | `University.EmployeePosition` | Должность, кафедра, ставка |
| 5 | `Edu_OrgUnits` | ID | `University.OrgUnit` | TypeID=1 → кафедра, TypeID=2 → институт |
| 6 | `Edu_OrgUnitTypes` | ID | `University.OrgUnitType` | Типы оргюнитов |
| 7 | `Edu_Semesters` | ID | `University.Semester` | Семестры (StudyYear, StartsOn, EndsOn) |
| 8 | `Edu_SemesterTypes` | ID | `University.SemesterType` | Весна/Лето/Осень/Зима |
| 9 | `Edu_Specialities` | ID | `University.Speciality` | YearsOfStudy для eligibility |
| 10 | `Edu_SpecialityLevels` | ID | `University.SpecialityLevel` | Бакалавриат/Магистратура/PhD |
| 11 | `Edu_StudentStatuses` | ID | `University.StudentStatus` | Учится/Отчислен/Академ |
| 12 | `Edu_Positions` | ID | `University.Position` | Должности преподавателей |

---

## 2. ARCHITECTURE: Two DbContext

```
UniversityDbContext (Read-Only)     ApplicationDbContext (Read-Write)
─────────────────────────────       ──────────────────────────────────
12 Edu_* tables                     Auth: LocalAccounts, UserIdentities, RoleAccess, ...
NoTracking by default               Auth: RoleOperation, RoleActionType, RoleOperationAction
No migrations                       Auth: UserAccess, UserAccessHistory, 3 Views
EnsureCreated() for local dev      Wf: WorkTypes, States, Transitions
                                    Common: WorkflowStages, Stages, Notifications, ...
                                    Thesis: Directions, Topics, StudentWorks, ...
                                    Defense: Commissions, Schedules, Grades, ...
```

**Один connection string** на локальную БД. Для production: UniversityDbContext → university БД (read-only).

---

## 3. CURRENT DOMAIN STRUCTURE

```
Domain/
├── Common/              # Entity<TId>, AggregateRoot<TId>, IAuditable, ISoftDeletable, IDomainEvent, ValueObject
├── Primitives/          # MultilingualText, FileHash, DateRange
├── Auth/                # RBAC+ (RoleAccess, RoleOperation, UserAccess, ...) + Interfaces
│   ├── Entities/        # 6 entities
│   ├── Repositories/    # 6 interfaces
│   ├── ViewModels/      # 3 views
│   └── Interfaces/      # IJwtTokenService, IPasswordHasher
├── University/          # 12 read-only entities
├── Wf/                  # WorkType, State, Transition
├── Thesis/              # 12 entities + 4 reference + events + services
├── Defense/             # 7 entities + 3 reference
├── CommonDomain/        # Stage, Notification, WorkflowStage, NotificationTemplate
└── Repositories/        # IUnitOfWork + all repo interfaces
```

---

## 4. FK REMAPPING (DONE)

Addon таблицы ссылаются на University entities через **integer FK**:

| Addon Entity | Property | → University Table | Notes |
|-------------|----------|-------------------|-------|
| Thesis.Direction | `OrgUnitId` | Edu_OrgUnits.ID | Было DepartmentId |
| Thesis.Direction | `EmployeeId` | Edu_Employees.ID | Было SupervisorId |
| Thesis.Direction | `SemesterId` | Edu_Semesters.ID | Было AcademicYearId |
| Thesis.Topic | `OrgUnitId` | Edu_OrgUnits.ID | Было DepartmentId |
| Thesis.Topic | `EmployeeId` | Edu_Employees.ID | Было SupervisorId |
| Thesis.Topic | `SemesterId` | Edu_Semesters.ID | Было AcademicYearId |
| Thesis.StudentWork | `OrgUnitId` | Edu_OrgUnits.ID | Было DepartmentId |
| Thesis.StudentWork | `SemesterId` | Edu_Semesters.ID | Было AcademicYearId |
| Thesis.Expert | `OrgUnitId` | Edu_OrgUnits.ID | Было DepartmentId |
| Thesis.SupervisorReview | `EmployeeId` | Edu_Employees.ID | Было SupervisorId |
| Defense.Commission | `OrgUnitId` | Edu_OrgUnits.ID | Было DepartmentId |
| Defense.Commission | `SemesterId` | Edu_Semesters.ID | Было AcademicYearId |
| Defense.EvaluationCriteria | `OrgUnitId` | Edu_OrgUnits.ID | Было DepartmentId |
| Common.Stage | `OrgUnitId` | Edu_OrgUnits.ID | Было DepartmentId |
| Wf.WorkType | `SpecialityLevelId` | Edu_SpecialityLevels.ID | Было DegreeLevelId |
| Wf.Transition | `RoleAccessId` | Auth.RoleAccess.Id | Было AllowedRoleId |
| Auth.UserAccess | `UserId` | Edu_Users.ID | — |
| Thesis.WorkflowHistory | `UserId` | Edu_Users.ID | — |
| Thesis.Expert | `UserId` | Edu_Users.ID | — |
| Defense.CommissionMember | `UserId` | Edu_Users.ID | — |
| Common.Notification | `UserId` | Edu_Users.ID | — |

**DB column names** сохранены через `.HasColumnName()` — миграция не переименовывает колонки.

---

## 5. СТАТУС МИГРАЦИИ

### ✅ DONE

| Этап | Описание | Файлов | Статус |
|------|----------|--------|--------|
| 0 | Подготовка (аудит, DDL, заморозка) | — | Выполнено |
| 1 | University entities + DbContext + repos | ~36 | Выполнено |
| 2 | Удаление дубликатов (11 entity, 11 configs, DbSeeder) | ~50 обновлено | Выполнено |
| 3 | FK remapping (15 renames, ~350+ ссылок) | ~110 | Выполнено |
| 4 | DbContext + конфигурации | ~30 | Выполнено |
| 4.5 | Сгенерировать и применить миграцию `AddV6UniversityIntegration` | 1 | Выполнено (БД обновлена, схемы `Edu`/`Org` удалены) |
| 6 | Infrastructure repos + DI | ~15 | Выполнено |
| — | Auth/RbacPlus → Auth/ (упразднение папки) | ~37 | Выполнено |

### 🔲 TODO

| Этап | Описание | Статус | Файлов |
|-----|----------|--------|--------|
| 5 | Переписать 34 заглушённых Application handlers | **В процессе анализа** | ~34 |
| 7 | Обновить WebAPI controllers | **Не начат** | ~15 |
| 8 | Создать новый DbSeeder (RBAC+, Workflow, Reference) | **Не начат** | 1 |
| 9 | Скрипты миграции данных (если production) | **Не начат** | ~6 |
| 10 | Тестирование | **Не начат** | ~20 |

---

## 6. STAGE 5: APPLICATION HANDLERS (DELEGATION MAP)

34 handler'а заглушены (возвращают "Not implemented"). Разделены на 5 групп для параллельной работы.

### 5A. Auth Handlers (4 файла)
**Ответственный**: _________
**Зависимости**: `IUserRepository`, `Auth.LocalAccounts`, `Auth.UserIdentities`

| # | Handler | Тип | Что делать |
|---|---------|-----|-----------|
| 1 | `Auth/Commands/Login/LoginCommandHandler.cs` | Command | Реализовать через `LocalAccounts` + `UserIdentities` |
| 2 | `Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs` | Command | Обновить токен |
| 3 | `Auth/Commands/Register/RegisterUserCommandHandler.cs` | Command | Создать `LocalAccount` + `UserIdentity` |
| 4 | `Auth/Queries/GetCurrentUserProfile/GetCurrentUserProfileQueryHandler.cs` | Query | Читать из `University.User` + `UserAccess` |

### 5B. Admin User Handlers (6 файлов)
**Ответственный**: _________
**Зависимости**: `IUserRepository`, `IOrganizationLookupRepository`, `IRoleAccessRepository`

| # | Handler | Тип | Что делать |
|---|---------|-----|-----------|
| 1 | `Admin/Users/Queries/GetAllUsers/GetAllUsersQueryHandler.cs` | Query | Читать `University.User` + пагинация |
| 2 | `Admin/Users/Queries/GetUserById/GetUserByIdQueryHandler.cs` | Query | Читать `University.User` по ID |
| 3 | `Admin/Roles/Queries/GetAllRoles/GetAllRolesQueryHandler.cs` | Query | Читать `Auth.RoleAccess` |
| 4 | `Admin/Users/Commands/CreateUser/CreateUserCommandHandler.cs` | Command | Создать `LocalAccount` (не University.User!) |
| 5 | `Admin/Users/Commands/UpdateUser/UpdateUserCommandHandler.cs` | Command | Обновить `LocalAccount` |
| 6 | `Admin/Users/Commands/ToggleUserStatus/ToggleUserStatusCommandHandler.cs` | Command | Деактивировать `LocalAccount` |

### 5C. Org/Edu Query Handlers (6 файлов)
**Ответственный**: _________
**Зависимости**: `IOrganizationLookupRepository`, `IStudentRepository`, `IStaffRepository`, `IAcademicProgramRepository`, `IDegreeLevelRepository`

| # | Handler | Тип | Что делать |
|---|---------|-----|-----------|
| 1 | `Org/Institutes/Queries/GetAllInstitutes/GetAllInstitutesQueryHandler.cs` | Query | Уже работает ✅ |
| 2 | `Org/Institutes/Queries/GetInstituteById/GetInstituteByIdQueryHandler.cs` | Query | Уже работает ✅ |
| 3 | `Org/Departments/Queries/GetAllDepartments/GetAllDepartmentsQueryHandler.cs` | Query | Уже работает ✅ |
| 4 | `Org/Departments/Queries/GetDepartmentsByInstitute/...` | Query | Уже работает ✅ |
| 5 | `Edu/Students/Queries/GetStudentsByProgram/...` | Query | Читать `University.Student` по SpecialityId |
| 6 | `Edu/Staff/Queries/GetStaffByDepartment/...` | Query | Читать `University.Employee` + `EmployeePosition` |

### 5D. Org/Edu Command Handlers (12 файлов)
**Ответственный**: Antigravity
**Статус**: **Выполнено** ✅ (Все 12 обработчиков команд физически удалены, соответствующие эндпоинты убраны из API контроллеров)

### 5E. Edu Staff/Student Handlers (6 файлов)
**Ответственный**: Antigravity
**Статус**: **В процессе** (Все 6 обработчиков команд физически удалены и убраны из контроллеров. Запросы `GetSupervisors` и `GetStaffByDepartment` требуют реализации)

### 5F. Common Period Handler (1 файл)
**Ответственный**: _________

| # | Handler | Тип | Что делать |
|---|---------|-----|-----------|
| 1 | `Common/Periods/Commands/ApproveInitialPeriods/...` | Command | Обновить — использует `Common.Stages` |

---

## 7. STAGE 7: WEBAPI CONTROLLERS (DELEGATION MAP)

**Ответственный**: _________
**Зависимости**: Stage 5

| Controller | Действие |
|-----------|----------|
| `UsersController` | Читать `University.User` |
| `StudentsController` | Читать `University.Student` |
| `StaffController` | Читать `University.Employee` |
| `InstitutesController` → `OrgUnitsController` | Переименовать, TypeID=2 |
| `DepartmentsController` → `OrgUnitsController` | Объединить, TypeID=1 |
| `AcademicProgramsController` → `SpecialitiesController` | Переименовать |
| `DegreeLevelsController` → `SpecialityLevelsController` | Переименовать |
| `CurrentUserProvider` | EduUserId из JWT |

---

## 8. STAGE 8: NEW DBSEEDER

**Ответственный**: _________
**Зависимости**: Stage 4.5 (миграция)

| Блок | Данные |
|------|--------|
| RBAC+ | 10 ролей, 21 операция, 4 действия, ~150 матричных записей |
| Workflow | 4 WorkTypes, 16 States каждый, Transitions |
| Reference | ApplicationStatuses(3), ParticipantRoles(2), AttachmentTypes(6), CheckTypes(3), CommissionTypes(2), CommissionRoles(3), AttendanceStatuses(3), WorkflowStages(7) |
| Notifications | 7 шаблонов × 3 языка |

---

## 9. RISKS

| Риск | Вероятность | Митигация |
|------|-------------|-----------|
| University schema изменится | Низкая | `UniversityDbContext` read-only, изменения только в маппинге |
| Edu_Users не содержит нужных пользователей | Средняя | `Auth.LocalAccounts` для изолированных аккаунтов |
| Edu_OrgUnits не содержит нужных кафедр | Средняя | Seed скрипт для создания недостающих записей |
| Производительность JOIN'ов | Низкая | Индексы на FK, кеширование справочников |

---

## 10. KEY DECISIONS

| Решение | Обоснование |
|---------|-------------|
| University entities read-only | AWM не модифицирует master data |
| OrgUnit вместо Institute+Department | Единая иерархия через TypeId |
| SemesterId вместо AcademicYearId | Точная привязка к семестру |
| `.HasColumnName()` для FK renames | БД колонки не переименовываются |
| Auth.RbacPlus → Auth | Упрощение структуры |
| Удалить CRUD commands для University entities | Read-only — команды не нужны |

---

## 11. АНАЛИЗ ТЕКУЩЕГО СОСТОЯНИЯ И ПРОБЛЕМЫ АУТЕНТИФИКАЦИИ

На текущем этапе (после успешного наката миграции `AddV6UniversityIntegration` и очистки локальной БД от схем `Edu` и `Org`) выявлены следующие архитектурные вопросы, требующие решения перед реализацией обработчиков (Stage 5):

### 1. Проблема локальной аутентификации (Auth/Login/Register)
* **Текущее состояние**: Таблица `Auth.Users` удалена, так как пользователи перенесены в read-only таблицу `Edu_Users`. При этом сущности `LocalAccounts` и `UserIdentities`, упомянутые в архитектурном плане, отсутствуют в доменном слое и в `ApplicationDbContext`.
* **Предлагаемое решение**: 
  Необходимо создать новую сущность `LocalAccount` в доменном слое (`Domain/Auth/Entities/LocalAccount.cs`):
  ```csharp
  public class LocalAccount : Entity<int>, IAuditable
  {
      public int UserId { get; private set; } // FK -> Edu_Users.ID
      public string PasswordHash { get; private set; } = null!;
      public string? RefreshToken { get; private set; }
      public DateTime? RefreshTokenExpiryTime { get; private set; }
      // ... методы обновления токена и смены пароля
  }
  ```
  И добавить её конфигурацию в `ApplicationDbContext` с генерацией новой миграции `AddLocalAccountsTable`. Это позволит реализовать `LoginCommandHandler` и `RegisterUserCommandHandler`.

### 2. Физическое удаление устаревших обработчиков (Stage 5D и 5E)
* **Текущее состояние**: В кодовой базе проекта `AWM.Service.Application` всё ещё присутствуют файлы команд создания/обновления/удаления для `Institutes`, `Departments`, `AcademicPrograms`, `DegreeLevels`, `Staff`, `Students`. Они возвращают `NotImplemented` заглушки.
* **Предлагаемое решение**: Физически удалить данные папки и файлы из проекта `Application`, а также удалить соответствующие Endpoint'ы из API контроллеров, чтобы кодовая база соответствовала концепции Read-Only Master для университетских данных.

### 3. Переименование контрактов и контроллеров (Stage 7)
* В соответствии с маппингом из Section 6, необходимо провести рефакторинг API контроллеров и DTO-моделей:
  - `AcademicProgramsController` -> `SpecialitiesController`
  - `DegreeLevelsController` -> `SpecialityLevelsController`
  - `DepartmentsController` & `InstitutesController` -> объединены в `OrgUnitsController`

