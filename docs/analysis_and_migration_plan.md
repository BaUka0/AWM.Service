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

| Этап | Описание | Файлов |
|------|----------|--------|
| 0 | Подготовка (аудит, DDL, заморозка) | — |
| 1 | University entities + DbContext + repos | ~36 |
| 2 | Удаление дубликатов (11 entity, 11 configs, DbSeeder) | ~50 обновлено |
| 3 | FK remapping (15 renames, ~350+ ссылок) | ~110 |
| 4 | DbContext + конфигурации | ~30 |
| 6 | Infrastructure repos + DI | ~15 |
| — | Auth/RbacPlus → Auth/ (упразднение папки) | ~37 |

### 🔲 TODO

| Этап | Описание | Статус | Файлов |
|-----|----------|--------|--------|
| 4.5 | Сгенерировать миграцию `AddV6UniversityIntegration` | Готов к генерации | 1 |
| 5 | Переписать 34 заглушённых Application handlers | **Не начат** | ~34 |
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
**Ответственный**: _________
**Зависимости**: Решение — удалять или переделать?
**Примечание**: University entities read-only. Эти команды НЕ МОГУТ создавать/изменять University данные.

| # | Handler | Тип | Рекомендация |
|---|---------|-----|-------------|
| 1 | `Org/Institutes/Commands/CreateInstitute/...` | Command | **Удалить** — создаётся в University БД |
| 2 | `Org/Institutes/Commands/UpdateInstitute/...` | Command | **Удалить** |
| 3 | `Org/Institutes/Commands/DeleteInstitute/...` | Command | **Удалить** |
| 4 | `Org/Departments/Commands/CreateDepartment/...` | Command | **Удалить** |
| 5 | `Org/Departments/Commands/UpdateDepartment/...` | Command | **Удалить** |
| 6 | `Org/Departments/Commands/DeleteDepartment/...` | Command | **Удалить** |
| 7 | `Edu/AcademicPrograms/Commands/CreateAcademicProgram/...` | Command | **Удалить** |
| 8 | `Edu/AcademicPrograms/Commands/UpdateAcademicProgram/...` | Command | **Удалить** |
| 9 | `Edu/AcademicPrograms/Commands/DeleteAcademicProgram/...` | Command | **Удалить** |
| 10 | `Edu/DegreeLevels/Commands/CreateDegreeLevel/...` | Command | **Удалить** |
| 11 | `Edu/DegreeLevels/Commands/UpdateDegreeLevel/...` | Command | **Удалить** |
| 12 | `Edu/DegreeLevels/Commands/DeleteDegreeLevel/...` | Command | **Удалить** |

### 5E. Edu Staff/Student Handlers (6 файлов)
**Ответственный**: _________
**Зависимости**: `IStudentRepository`, `IStaffRepository`

| # | Handler | Тип | Что делать |
|---|---------|-----|-----------|
| 1 | `Edu/Staff/Queries/GetSupervisors/...` | Query | Читать `Employee` где `IsAdvisor=true` |
| 2 | `Edu/Staff/Commands/CreateStaff/...` | Command | **Удалить** — read-only |
| 3 | `Edu/Staff/Commands/UpdateStaff/...` | Command | **Удалить** — read-only |
| 4 | `Edu/Staff/Commands/UpdateStaffWorkload/...` | Command | **Удалить** — read-only |
| 5 | `Edu/Staff/Commands/ApproveSupervisors/...` | Command | **Удалить** — read-only |
| 6 | `Edu/Students/Commands/CreateStudent/...` | Command | **Удалить** — read-only |
| 7 | `Edu/Students/Commands/UpdateStudent/...` | Command | **Удалить** — read-only |

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
