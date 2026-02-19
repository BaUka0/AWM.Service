# Детальный технический бэклог: AWM.Service (ETMS v4.0)

> **Дата создания:** 2026-02-03  
> **Версия:** 1.0  
> **Статус:** Утверждено для распределения

---

## 📁 Фаза 1: Организационная структура (Org/Edu)

### 1.1 Управление Институтами и Кафедрами ✅

**📂 Application Layer** (`src/AWM.Service.Application/Features/Org/`)

| Файл | Описание |
|------|----------|
| `Commands/Institutes/CreateInstitute/CreateInstituteCommand.cs` | Команда создания института |
| `Commands/Institutes/CreateInstitute/CreateInstituteCommandHandler.cs` | Хендлер |
| `Commands/Institutes/CreateInstitute/CreateInstituteCommandValidator.cs` | FluentValidation валидатор |
| `Commands/Institutes/UpdateInstitute/UpdateInstituteCommand.cs` | Обновление |
| `Commands/Institutes/UpdateInstitute/UpdateInstituteCommandHandler.cs` | Хендлер |
| `Commands/Institutes/UpdateInstitute/UpdateInstituteCommandValidator.cs` | FluentValidation валидатор |
| `Commands/Institutes/DeleteInstitute/DeleteInstituteCommand.cs` | Мягкое удаление |
| `Commands/Departments/CreateDepartment/CreateDepartmentCommand.cs` | Команда создания кафедры |
| `Commands/Departments/CreateDepartment/CreateDepartmentCommandHandler.cs` | Хендлер |
| `Commands/Departments/CreateDepartment/CreateDepartmentCommandValidator.cs` | FluentValidation валидатор |
| `Commands/Departments/UpdateDepartment/UpdateDepartmentCommand.cs` | Обновление |
| `Commands/Departments/UpdateDepartment/UpdateDepartmentCommandHandler.cs` | Хендлер |
| `Commands/Departments/UpdateDepartment/UpdateDepartmentCommandValidator.cs` | FluentValidation валидатор |
| `Queries/Institutes/GetAllInstitutes/GetAllInstitutesQuery.cs` | Список институтов |
| `Queries/Institutes/GetInstituteById/GetInstituteByIdQuery.cs` | По ID |
| `Queries/Departments/GetDepartmentsByInstitute/...` | Кафедры института |
| `DTOs/InstituteDto.cs` | DTO для ответов |
| `DTOs/DepartmentDto.cs` | DTO для ответов |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/InstitutesController.cs` | `GET /api/v1/institutes`, `GET /{id}`, `POST`, `PUT`, `DELETE` |
| `Controllers/v1/DepartmentsController.cs` | `GET /api/v1/departments`, `GET /institutes/{id}/departments`, `POST`, `PUT`, `DELETE` |
| `Common/Contracts/Requests/Org/CreateInstituteRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Org/UpdateInstituteRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Org/CreateDepartmentRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Org/UpdateDepartmentRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Org/InstituteResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Org/DepartmentResponse.cs` | Response контракт |

---

### 1.2 Образовательные программы и Уровни степеней ✅

**📂 Application Layer** (`src/AWM.Service.Application/Features/Edu/`)

| Файл | Описание |
|------|----------|
| `Commands/AcademicPrograms/CreateAcademicProgram/CreateAcademicProgramCommand.cs` | Команда создания |
| `Commands/AcademicPrograms/CreateAcademicProgram/CreateAcademicProgramCommandHandler.cs` | Хендлер |
| `Commands/AcademicPrograms/CreateAcademicProgram/CreateAcademicProgramCommandValidator.cs` | FluentValidation валидатор |
| `Commands/AcademicPrograms/UpdateAcademicProgram/UpdateAcademicProgramCommand.cs` | Команда обновления |
| `Commands/AcademicPrograms/UpdateAcademicProgram/UpdateAcademicProgramCommandHandler.cs` | Хендлер |
| `Commands/AcademicPrograms/UpdateAcademicProgram/UpdateAcademicProgramCommandValidator.cs` | FluentValidation валидатор |
| `Commands/DegreeLevels/CreateDegreeLevel/CreateDegreeLevelCommand.cs` | Команда создания |
| `Commands/DegreeLevels/CreateDegreeLevel/CreateDegreeLevelCommandHandler.cs` | Хендлер |
| `Commands/DegreeLevels/CreateDegreeLevel/CreateDegreeLevelCommandValidator.cs` | FluentValidation валидатор |
| `Queries/AcademicPrograms/GetAcademicPrograms/GetAcademicProgramsQuery.cs` | С фильтрацией |
| `Queries/DegreeLevels/GetDegreeLevels/GetDegreeLevelsQuery.cs` | Список степеней |
| `DTOs/AcademicProgramDto.cs` | DTO для ответов |
| `DTOs/DegreeLevelDto.cs` | DTO для ответов |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/AcademicProgramsController.cs` | `GET /api/v1/academic-programs`, `POST`, `PUT` |
| `Controllers/v1/DegreeLevelsController.cs` | `GET /api/v1/degree-levels`, `POST` |
| `Common/Contracts/Requests/Edu/CreateAcademicProgramRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Edu/UpdateAcademicProgramRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Edu/CreateDegreeLevelRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Edu/AcademicProgramResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Edu/DegreeLevelResponse.cs` | Response контракт |

---

### 1.3 Управление Студентами и Сотрудниками ✅

**📂 Application Layer** (`src/AWM.Service.Application/Features/Edu/`)

| Файл | Описание |
|------|----------|
| `Commands/Students/CreateStudent/CreateStudentCommand.cs` | Создание профиля студента |
| `Commands/Students/CreateStudent/CreateStudentCommandHandler.cs` | Хендлер |
| `Commands/Students/CreateStudent/CreateStudentCommandValidator.cs` | FluentValidation валидатор |
| `Commands/Students/UpdateStudent/UpdateStudentCommand.cs` | Обновление |
| `Commands/Students/UpdateStudent/UpdateStudentCommandHandler.cs` | Хендлер |
| `Commands/Students/UpdateStudent/UpdateStudentCommandValidator.cs` | FluentValidation валидатор |
| `Commands/Staff/CreateStaff/CreateStaffCommand.cs` | Профиль сотрудника |
| `Commands/Staff/CreateStaff/CreateStaffCommandHandler.cs` | Хендлер |
| `Commands/Staff/CreateStaff/CreateStaffCommandValidator.cs` | FluentValidation валидатор |
| `Commands/Staff/UpdateStaff/UpdateStaffCommand.cs` | Обновление |
| `Commands/Staff/UpdateStaff/UpdateStaffCommandHandler.cs` | Хендлер |
| `Commands/Staff/UpdateStaff/UpdateStaffCommandValidator.cs` | FluentValidation валидатор |
| `Queries/Students/GetStudentById/GetStudentByIdQuery.cs` | Студент по ID |
| `Queries/Students/GetStudentsByDepartment/GetStudentsByDepartmentQuery.cs` | Студенты кафедры |
| `Queries/Staff/GetStaffByDepartment/GetStaffByDepartmentQuery.cs` | Сотрудники кафедры |
| `Queries/Staff/GetSupervisors/GetSupervisorsQuery.cs` | Только научные руководители |
| `DTOs/StudentDto.cs` | DTO для ответов |
| `DTOs/StaffDto.cs` | DTO для ответов |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/StudentsController.cs` | `GET /api/v1/students`, `GET /{id}`, `POST`, `PUT` |
| `Controllers/v1/StaffController.cs` | `GET /api/v1/staff`, `GET /supervisors`, `POST`, `PUT` |
| `Common/Contracts/Requests/Edu/CreateStudentRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Edu/UpdateStudentRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Edu/CreateStaffRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Edu/UpdateStaffRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Edu/StudentResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Edu/StaffResponse.cs` | Response контракт |

---

## 📁 Фаза 2: Периоды и Воркфлоу ✅

### 2.1 Управление периодами ✅

**📂 Application Layer** (`src/AWM.Service.Application/Features/Common/`)

| Файл | Описание |
|------|----------|
| `Commands/Periods/CreatePeriod/CreatePeriodCommand.cs` | Команда создания периода |
| `Commands/Periods/CreatePeriod/CreatePeriodCommandHandler.cs` | Хендлер |
| `Commands/Periods/CreatePeriod/CreatePeriodCommandValidator.cs` | FluentValidation валидатор |
| `Commands/Periods/UpdatePeriod/UpdatePeriodCommand.cs` | Команда обновления |
| `Commands/Periods/UpdatePeriod/UpdatePeriodCommandHandler.cs` | Хендлер |
| `Commands/Periods/UpdatePeriod/UpdatePeriodCommandValidator.cs` | FluentValidation валидатор |
| `Queries/Periods/GetPeriodsByDepartment/GetPeriodsByDepartmentQuery.cs` | Периоды кафедры |
| `Queries/Periods/GetActivePeriod/GetActivePeriodQuery.cs` | Текущий активный период |
| `Services/IPeriodValidationService.cs` | Интерфейс (в Domain) |
| `Services/PeriodValidationService.cs` | Реализация |
| `DTOs/PeriodDto.cs` | DTO для ответов |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/PeriodsController.cs` | `GET /departments/{deptId}/periods`, `POST`, `PUT` |
| `Common/Contracts/Requests/Common/CreatePeriodRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Common/UpdatePeriodRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Common/PeriodResponse.cs` | Response контракт |

---

### 2.2 Машина состояний (Workflow Engine) ✅

**📂 Application Layer** (`src/AWM.Service.Application/Features/Workflow/`)

| Файл | Описание |
|------|----------|
| `Services/IWorkflowService.cs` | Интерфейс машины состояний |
| `Services/WorkflowService.cs` | Реализация переходов |
| `Commands/TransitionState/TransitionStateCommand.cs` | Перевод сущности в новое состояние |
| `Commands/TransitionState/TransitionStateCommandHandler.cs` | Хендлер |
| `Commands/TransitionState/TransitionStateCommandValidator.cs` | FluentValidation валидатор |
| `Queries/GetAvailableTransitions/GetAvailableTransitionsQuery.cs` | Доступные переходы для текущего пользователя |
| `DTOs/StateDto.cs` | DTO для состояний |
| `DTOs/TransitionDto.cs` | DTO для переходов |

**📂 Domain Layer** (`src/AWM.Service.Domain/Wf/`)

| Файл | Описание |
|------|----------|
| `Interfaces/IWorkflowService.cs` | Интерфейс для DI |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Common/Contracts/Requests/Workflow/TransitionStateRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Workflow/StateResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Workflow/TransitionResponse.cs` | Response контракт |

---

## 📁 Фаза 3: Направления и Темы ✅

### 3.1 Управление направлениями (Directions)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Directions/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateDirection/CreateDirectionCommand.cs` | НР создает направление |
| `Commands/CreateDirection/CreateDirectionCommandHandler.cs` | Хендлер |
| `Commands/CreateDirection/CreateDirectionCommandValidator.cs` | FluentValidation валидатор |
| `Commands/UpdateDirection/UpdateDirectionCommand.cs` | Редактирование |
| `Commands/UpdateDirection/UpdateDirectionCommandHandler.cs` | Хендлер |
| `Commands/UpdateDirection/UpdateDirectionCommandValidator.cs` | FluentValidation валидатор |
| `Commands/SubmitDirection/SubmitDirectionCommand.cs` | Отправка на рассмотрение кафедрой |
| `Commands/SubmitDirection/SubmitDirectionCommandHandler.cs` | Хендлер |
| `Commands/ApproveDirection/ApproveDirectionCommand.cs` | Кафедра утверждает |
| `Commands/ApproveDirection/ApproveDirectionCommandHandler.cs` | Хендлер |
| `Commands/RejectDirection/RejectDirectionCommand.cs` | Кафедра отклоняет |
| `Commands/RejectDirection/RejectDirectionCommandHandler.cs` | Хендлер |
| `Commands/RejectDirection/RejectDirectionCommandValidator.cs` | FluentValidation (причина отклонения) |
| `Commands/RequestRevision/RequestRevisionCommand.cs` | На доработку |
| `Commands/RequestRevision/RequestRevisionCommandHandler.cs` | Хендлер |
| `Commands/RequestRevision/RequestRevisionCommandValidator.cs` | FluentValidation (комментарий) |
| `Queries/GetDirectionsByDepartment/GetDirectionsByDepartmentQuery.cs` | Все направления кафедры |
| `Queries/GetDirectionsBySupervisor/GetDirectionsBySupervisorQuery.cs` | Направления НР |
| `Queries/GetDirectionById/GetDirectionByIdQuery.cs` | Направление по ID |
| `DTOs/DirectionDto.cs` | DTO для списка |
| `DTOs/DirectionDetailDto.cs` | DTO с полной информацией |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/DirectionsController.cs` | CRUD + workflow endpoints |
| `Common/Contracts/Requests/Thesis/CreateDirectionRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Thesis/UpdateDirectionRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Thesis/RejectDirectionRequest.cs` | Request с причиной |
| `Common/Contracts/Requests/Thesis/RequestRevisionRequest.cs` | Request с комментарием |
| `Common/Contracts/Responses/Thesis/DirectionResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Thesis/DirectionDetailResponse.cs` | Response с деталями |

---

### 3.2 Управление темами (Topics)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Topics/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateTopic/CreateTopicCommand.cs` | НР создает тему |
| `Commands/CreateTopic/CreateTopicCommandHandler.cs` | Хендлер |
| `Commands/CreateTopic/CreateTopicCommandValidator.cs` | FluentValidation валидатор |
| `Commands/UpdateTopic/UpdateTopicCommand.cs` | Обновление темы |
| `Commands/UpdateTopic/UpdateTopicCommandHandler.cs` | Хендлер |
| `Commands/UpdateTopic/UpdateTopicCommandValidator.cs` | FluentValidation валидатор |
| `Commands/ApproveTopic/ApproveTopicCommand.cs` | Кафедра утверждает |
| `Commands/ApproveTopic/ApproveTopicCommandHandler.cs` | Хендлер |
| `Commands/CloseTopic/CloseTopicCommand.cs` | Закрытие темы |
| `Commands/CloseTopic/CloseTopicCommandHandler.cs` | Хендлер |
| `Queries/GetTopicsByDirection/GetTopicsByDirectionQuery.cs` | Темы направления |
| `Queries/GetAvailableTopics/GetAvailableTopicsQuery.cs` | Доступные для выбора студентами |
| `Queries/GetTopicById/GetTopicByIdQuery.cs` | Тема по ID |
| `DTOs/TopicDto.cs` | DTO для списка |
| `DTOs/TopicDetailDto.cs` | DTO с деталями |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/TopicsController.cs` | CRUD + workflow endpoints |
| `Common/Contracts/Requests/Thesis/CreateTopicRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Thesis/UpdateTopicRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Thesis/TopicResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Thesis/TopicDetailResponse.cs` | Response с деталями |

---

## 📁 Фаза 4: Заявки и Выбор тем Kenessary

### 4.1 Заявки студентов (Topic Applications)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Applications/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateApplication/CreateApplicationCommand.cs` | Студент подает заявку |
| `Commands/CreateApplication/CreateApplicationCommandHandler.cs` | Хендлер |
| `Commands/CreateApplication/CreateApplicationCommandValidator.cs` | FluentValidation валидатор |
| `Commands/AcceptApplication/AcceptApplicationCommand.cs` | НР принимает |
| `Commands/AcceptApplication/AcceptApplicationCommandHandler.cs` | Хендлер |
| `Commands/RejectApplication/RejectApplicationCommand.cs` | НР отклоняет |
| `Commands/RejectApplication/RejectApplicationCommandHandler.cs` | Хендлер |
| `Commands/RejectApplication/RejectApplicationCommandValidator.cs` | FluentValidation (причина) |
| `Commands/WithdrawApplication/WithdrawApplicationCommand.cs` | Студент отзывает |
| `Commands/WithdrawApplication/WithdrawApplicationCommandHandler.cs` | Хендлер |
| `Queries/GetApplicationsByTopic/GetApplicationsByTopicQuery.cs` | Заявки на тему (для НР) |
| `Queries/GetApplicationsByStudent/GetApplicationsByStudentQuery.cs` | Заявки студента |
| `DTOs/TopicApplicationDto.cs` | DTO для ответов |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/TopicApplicationsController.cs` | CRUD + workflow endpoints |
| `Common/Contracts/Requests/Thesis/CreateApplicationRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Thesis/RejectApplicationRequest.cs` | Request с причиной |
| `Common/Contracts/Responses/Thesis/TopicApplicationResponse.cs` | Response контракт |

---

### 4.2 Создание работ и участников

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Works/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateStudentWork/CreateStudentWorkCommand.cs` | Автоматически при принятии заявки |
| `Commands/CreateStudentWork/CreateStudentWorkCommandHandler.cs` | Хендлер |
| `Commands/AddParticipant/AddParticipantCommand.cs` | Добавление участника в команду |
| `Commands/AddParticipant/AddParticipantCommandHandler.cs` | Хендлер |
| `Commands/AddParticipant/AddParticipantCommandValidator.cs` | FluentValidation валидатор |
| `Commands/RemoveParticipant/RemoveParticipantCommand.cs` | Удаление участника |
| `Commands/RemoveParticipant/RemoveParticipantCommandHandler.cs` | Хендлер |
| `Queries/GetStudentWorkById/GetStudentWorkByIdQuery.cs` | Работа по ID |
| `Queries/GetStudentWorksByDepartment/GetStudentWorksByDepartmentQuery.cs` | Для кафедры |
| `Queries/GetStudentWorksBySupervisor/GetStudentWorksBySupervisorQuery.cs` | Для НР |
| `Queries/GetMyWork/GetMyWorkQuery.cs` | Для текущего студента |
| `DTOs/StudentWorkDto.cs` | DTO для списка |
| `DTOs/StudentWorkDetailDto.cs` | DTO с деталями |
| `DTOs/WorkParticipantDto.cs` | DTO участника |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/StudentWorksController.cs` | CRUD endpoints |
| `Common/Contracts/Requests/Thesis/AddParticipantRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Thesis/StudentWorkResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Thesis/StudentWorkDetailResponse.cs` | Response с деталями |
| `Common/Contracts/Responses/Thesis/WorkParticipantResponse.cs` | Response участника |

---

## 📁 Фаза 5: Предзащита и проверки Zhan

### 5.1 Комиссии

**📂 Application Layer** (`src/AWM.Service.Application/Features/Defense/Commissions/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateCommission/CreateCommissionCommand.cs` | Создание комиссии |
| `Commands/CreateCommission/CreateCommissionCommandHandler.cs` | Хендлер |
| `Commands/CreateCommission/CreateCommissionCommandValidator.cs` | FluentValidation валидатор |
| `Commands/UpdateCommission/UpdateCommissionCommand.cs` | Обновление комиссии |
| `Commands/UpdateCommission/UpdateCommissionCommandHandler.cs` | Хендлер |
| `Commands/UpdateCommission/UpdateCommissionCommandValidator.cs` | FluentValidation валидатор |
| `Commands/AddCommissionMember/AddCommissionMemberCommand.cs` | Добавление члена |
| `Commands/AddCommissionMember/AddCommissionMemberCommandHandler.cs` | Хендлер |
| `Commands/AddCommissionMember/AddCommissionMemberCommandValidator.cs` | FluentValidation валидатор |
| `Commands/RemoveCommissionMember/RemoveCommissionMemberCommand.cs` | Удаление члена |
| `Commands/RemoveCommissionMember/RemoveCommissionMemberCommandHandler.cs` | Хендлер |
| `Queries/GetCommissionsByDepartment/GetCommissionsByDepartmentQuery.cs` | Комиссии кафедры |
| `Queries/GetCommissionById/GetCommissionByIdQuery.cs` | Комиссия по ID |
| `DTOs/CommissionDto.cs` | DTO для списка |
| `DTOs/CommissionDetailDto.cs` | DTO с деталями |
| `DTOs/CommissionMemberDto.cs` | DTO члена комиссии |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/CommissionsController.cs` | CRUD + members endpoints |
| `Common/Contracts/Requests/Defense/CreateCommissionRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Defense/UpdateCommissionRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Defense/AddCommissionMemberRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Defense/CommissionResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Defense/CommissionDetailResponse.cs` | Response с деталями |
| `Common/Contracts/Responses/Defense/CommissionMemberResponse.cs` | Response члена |

---

### 5.2 Проверки качества (Quality Checks)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/QualityChecks/`)

| Файл | Описание |
|------|----------|
| `Commands/SubmitForCheck/SubmitForCheckCommand.cs` | Студент сдает на проверку |
| `Commands/SubmitForCheck/SubmitForCheckCommandHandler.cs` | Хендлер |
| `Commands/SubmitForCheck/SubmitForCheckCommandValidator.cs` | FluentValidation валидатор |
| `Commands/RecordCheckResult/RecordCheckResultCommand.cs` | Эксперт фиксирует результат |
| `Commands/RecordCheckResult/RecordCheckResultCommandHandler.cs` | Хендлер |
| `Commands/RecordCheckResult/RecordCheckResultCommandValidator.cs` | FluentValidation валидатор |
| `Queries/GetChecksByWork/GetChecksByWorkQuery.cs` | История проверок работы |
| `Queries/GetPendingChecks/GetPendingChecksQuery.cs` | Ожидающие проверки (для эксперта) |
| `DTOs/QualityCheckDto.cs` | DTO для ответов |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/QualityChecksController.cs` | Endpoints проверок |
| `Common/Contracts/Requests/Thesis/SubmitForCheckRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Thesis/RecordCheckResultRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Thesis/QualityCheckResponse.cs` | Response контракт |

---

### 5.3 Предзащиты

**📂 Application Layer** (`src/AWM.Service.Application/Features/Defense/PreDefense/`)

| Файл | Описание |
|------|----------|
| `Commands/SchedulePreDefense/SchedulePreDefenseCommand.cs` | Назначение даты |
| `Commands/SchedulePreDefense/SchedulePreDefenseCommandHandler.cs` | Хендлер |
| `Commands/SchedulePreDefense/SchedulePreDefenseCommandValidator.cs` | FluentValidation валидатор |
| `Commands/RecordAttendance/RecordAttendanceCommand.cs` | Отметка явки |
| `Commands/RecordAttendance/RecordAttendanceCommandHandler.cs` | Хендлер |
| `Commands/RecordAttendance/RecordAttendanceCommandValidator.cs` | FluentValidation валидатор |
| `Commands/SubmitPreDefenseGrade/SubmitPreDefenseGradeCommand.cs` | Член комиссии ставит оценку |
| `Commands/SubmitPreDefenseGrade/SubmitPreDefenseGradeCommandHandler.cs` | Хендлер |
| `Commands/SubmitPreDefenseGrade/SubmitPreDefenseGradeCommandValidator.cs` | FluentValidation валидатор |
| `Commands/FinalizePreDefense/FinalizePreDefenseCommand.cs` | Секретарь финализирует |
| `Commands/FinalizePreDefense/FinalizePreDefenseCommandHandler.cs` | Хендлер |
| `Queries/GetPreDefenseSchedule/GetPreDefenseScheduleQuery.cs` | Расписание предзащит |
| `Queries/GetPreDefenseAttempts/GetPreDefenseAttemptsQuery.cs` | Попытки предзащиты |
| `DTOs/PreDefenseAttemptDto.cs` | DTO попытки |
| `DTOs/PreDefenseScheduleDto.cs` | DTO расписания |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/PreDefenseController.cs` | Endpoints предзащит |
| `Common/Contracts/Requests/Defense/SchedulePreDefenseRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Defense/RecordAttendanceRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Defense/SubmitPreDefenseGradeRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Defense/PreDefenseAttemptResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Defense/PreDefenseScheduleResponse.cs` | Response контракт |

---

## 📁 Фаза 6: Финальная защита

### 6.1 Расписание защит

**📂 Application Layer** (`src/AWM.Service.Application/Features/Defense/Schedule/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateDefenseSchedule/CreateDefenseScheduleCommand.cs` | Создание расписания |
| `Commands/CreateDefenseSchedule/CreateDefenseScheduleCommandHandler.cs` | Хендлер |
| `Commands/CreateDefenseSchedule/CreateDefenseScheduleCommandValidator.cs` | FluentValidation валидатор |
| `Commands/UpdateDefenseSchedule/UpdateDefenseScheduleCommand.cs` | Обновление расписания |
| `Commands/UpdateDefenseSchedule/UpdateDefenseScheduleCommandHandler.cs` | Хендлер |
| `Commands/UpdateDefenseSchedule/UpdateDefenseScheduleCommandValidator.cs` | FluentValidation валидатор |
| `Commands/AssignWorkToSlot/AssignWorkToSlotCommand.cs` | Назначение работы на слот |
| `Commands/AssignWorkToSlot/AssignWorkToSlotCommandHandler.cs` | Хендлер |
| `Commands/AssignWorkToSlot/AssignWorkToSlotCommandValidator.cs` | FluentValidation валидатор |
| `Queries/GetDefenseSchedule/GetDefenseScheduleQuery.cs` | Расписание защит |
| `Queries/GetDefenseSlotById/GetDefenseSlotByIdQuery.cs` | Слот по ID |
| `DTOs/ScheduleDto.cs` | DTO расписания |
| `DTOs/DefenseSlotDto.cs` | DTO слота |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/DefenseScheduleController.cs` | CRUD endpoints |
| `Common/Contracts/Requests/Defense/CreateDefenseScheduleRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Defense/UpdateDefenseScheduleRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Defense/AssignWorkToSlotRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Defense/ScheduleResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Defense/DefenseSlotResponse.cs` | Response слота |

---

### 6.2 Оценивание и Протоколы

**📂 Application Layer** (`src/AWM.Service.Application/Features/Defense/Evaluation/`)

| Файл | Описание |
|------|----------|
| `Commands/SubmitGrade/SubmitGradeCommand.cs` | Член ГАК ставит оценку |
| `Commands/SubmitGrade/SubmitGradeCommandHandler.cs` | Хендлер |
| `Commands/SubmitGrade/SubmitGradeCommandValidator.cs` | FluentValidation валидатор |
| `Commands/FinalizeDefense/FinalizeDefenseCommand.cs` | Секретарь финализирует |
| `Commands/FinalizeDefense/FinalizeDefenseCommandHandler.cs` | Хендлер |
| `Commands/GenerateProtocol/GenerateProtocolCommand.cs` | Генерация протокола |
| `Commands/GenerateProtocol/GenerateProtocolCommandHandler.cs` | Хендлер |
| `Queries/GetEvaluationCriteria/GetEvaluationCriteriaQuery.cs` | Критерии оценки |
| `Queries/GetGradesByWork/GetGradesByWorkQuery.cs` | Оценки работы |
| `Queries/GetProtocol/GetProtocolQuery.cs` | Протокол по ID |
| `DTOs/GradeDto.cs` | DTO оценки |
| `DTOs/ProtocolDto.cs` | DTO протокола |
| `DTOs/EvaluationCriteriaDto.cs` | DTO критериев |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/EvaluationController.cs` | Endpoints оценивания |
| `Controllers/v1/ProtocolsController.cs` | Endpoints протоколов |
| `Common/Contracts/Requests/Defense/SubmitGradeRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Defense/GenerateProtocolRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Defense/GradeResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Defense/EvaluationCriteriaResponse.cs` | Response критериев |
| `Common/Contracts/Responses/Defense/ProtocolResponse.cs` | Response контракт |

---

## 📁 Фаза 7: Сквозные сервисы

### 7.1 Уведомления

**📂 Application Layer** (`src/AWM.Service.Application/Features/Common/Notifications/`)

| Файл | Описание |
|------|----------|
| `Services/INotificationService.cs` | Интерфейс |
| `Services/NotificationService.cs` | Реализация |
| `Commands/MarkAsRead/MarkAsReadCommand.cs` | Отметить как прочитанное |
| `Commands/MarkAsRead/MarkAsReadCommandHandler.cs` | Хендлер |
| `Commands/MarkAllAsRead/MarkAllAsReadCommand.cs` | Отметить все как прочитанные |
| `Commands/MarkAllAsRead/MarkAllAsReadCommandHandler.cs` | Хендлер |
| `Queries/GetMyNotifications/GetMyNotificationsQuery.cs` | Уведомления пользователя |
| `DTOs/NotificationDto.cs` | DTO для ответов |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/NotificationsController.cs` | CRUD endpoints |
| `Common/Contracts/Responses/Common/NotificationResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Common/NotificationListResponse.cs` | Response списка |

---

### 7.2 Файловое хранилище (Attachments)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Attachments/`)

| Файл | Описание |
|------|----------|
| `Services/IAttachmentService.cs` | Интерфейс |
| `Commands/UploadAttachment/UploadAttachmentCommand.cs` | Загрузка файла |
| `Commands/UploadAttachment/UploadAttachmentCommandHandler.cs` | Хендлер |
| `Commands/UploadAttachment/UploadAttachmentCommandValidator.cs` | FluentValidation (размер, тип файла) |
| `Commands/DeleteAttachment/DeleteAttachmentCommand.cs` | Удаление файла |
| `Commands/DeleteAttachment/DeleteAttachmentCommandHandler.cs` | Хендлер |
| `Queries/GetAttachmentsByWork/GetAttachmentsByWorkQuery.cs` | Файлы работы |
| `Queries/GetAttachmentById/GetAttachmentByIdQuery.cs` | Файл по ID |
| `DTOs/AttachmentDto.cs` | DTO для ответов |

**📂 Infrastructure Layer** (`src/AWM.Service.Infrastructure/FileStorage/`)

| Файл | Описание |
|------|----------|
| `LocalFileStorageService.cs` | Локальное хранилище |
| `S3FileStorageService.cs` | AWS S3 (опционально) |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/AttachmentsController.cs` | CRUD + download endpoints |
| `Common/Contracts/Requests/Thesis/UploadAttachmentRequest.cs` | Request контракт (multipart/form-data) |
| `Common/Contracts/Responses/Thesis/AttachmentResponse.cs` | Response контракт |

---

### 7.3 Отзывы и Рецензии

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Reviews/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateSupervisorReview/CreateSupervisorReviewCommand.cs` | Отзыв НР |
| `Commands/CreateSupervisorReview/CreateSupervisorReviewCommandHandler.cs` | Хендлер |
| `Commands/CreateSupervisorReview/CreateSupervisorReviewCommandValidator.cs` | FluentValidation валидатор |
| `Commands/UploadReview/UploadReviewCommand.cs` | Загрузка рецензии |
| `Commands/UploadReview/UploadReviewCommandHandler.cs` | Хендлер |
| `Commands/UploadReview/UploadReviewCommandValidator.cs` | FluentValidation валидатор |
| `Queries/GetReviewsByWork/GetReviewsByWorkQuery.cs` | Отзывы/рецензии работы |
| `DTOs/SupervisorReviewDto.cs` | DTO отзыва НР |
| `DTOs/ReviewDto.cs` | DTO рецензии |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/`)

| Файл | Описание |
|------|----------|
| `Controllers/v1/ReviewsController.cs` | CRUD endpoints |
| `Common/Contracts/Requests/Thesis/CreateSupervisorReviewRequest.cs` | Request контракт |
| `Common/Contracts/Requests/Thesis/UploadReviewRequest.cs` | Request контракт |
| `Common/Contracts/Responses/Thesis/SupervisorReviewResponse.cs` | Response контракт |
| `Common/Contracts/Responses/Thesis/ReviewResponse.cs` | Response контракт |

---

## 📊 Сводка по контроллерам

| Контроллер | Фаза | Ответственная группа |
|------------|------|----------------------|
| `InstitutesController.cs` | 1 | Группа А |
| `DepartmentsController.cs` | 1 | Группа А |
| `AcademicProgramsController.cs` | 1 | Группа А |
| `DegreeLevelsController.cs` | 1 | Группа А |
| `StudentsController.cs` | 1 | Группа А |
| `StaffController.cs` | 1 | Группа А |
| `PeriodsController.cs` | 2 | Группа А |
| `DirectionsController.cs` | 3 | Группа Б |
| `TopicsController.cs` | 3 | Группа Б |
| `TopicApplicationsController.cs` | 4 | Группа В |
| `StudentWorksController.cs` | 4 | Группа В |
| `CommissionsController.cs` | 5 | Группа Б |
| `QualityChecksController.cs` | 5 | Группа Б |
| `PreDefenseController.cs` | 5 | Группа Б |
| `DefenseScheduleController.cs` | 6 | Группа В |
| `EvaluationController.cs` | 6 | Группа В |
| `ProtocolsController.cs` | 6 | Группа В |
| `NotificationsController.cs` | 7 | Группа А |
| `AttachmentsController.cs` | 7 | Группа А |
| `ReviewsController.cs` | 7 | Группа Б |

---

## 📋 Распределение по командам

### Группа А (Инфраструктура/Ядро)

- **Фазы:** 1, 2, 7 (частично)
- **Контроллеры:** 8 шт.
- **Задачи:**
  - CRUD для организационной структуры
  - Управление периодами
  - Система уведомлений
  - Файловое хранилище

### Группа Б (Воркфлоу/Руководитель)

- **Фазы:** 3, 5, 7 (частично)
- **Контроллеры:** 6 шт.
- **Задачи:**
  - Направления и темы
  - Комиссии и проверки качества
  - Предзащиты
  - Отзывы и рецензии

### Группа В (Студент/Оценка)

- **Фазы:** 4, 6
- **Контроллеры:** 5 шт.
- **Задачи:**
  - Заявки студентов
  - Работы и участники
  - Расписание защит
  - Оценивание и протоколы

---

## 🔗 Зависимости между фазами

```
Фаза 1 ──► Фаза 2 ──► Фаза 3 ──► Фаза 4 ──► Фаза 5 ──► Фаза 6
   │          │          │          │          │          │
   └──────────┴──────────┴──────────┴──────────┴──────────┘
                              │
                          Фаза 7 (параллельно)
```

- **Фаза 1** должна быть завершена первой (справочники)
- **Фаза 2** зависит от Фазы 1 (периоды привязаны к кафедрам)
- **Фаза 3** зависит от Фазы 2 (воркфлоу)
- **Фаза 4** зависит от Фазы 3 (темы должны существовать)
- **Фаза 5** зависит от Фазы 4 (работы должны быть созданы)
- **Фаза 6** зависит от Фазы 5 (предзащиты пройдены)
- **Фаза 7** может разрабатываться параллельно с другими фазами

---

## ⏱️ Примерные трудозатраты

| Фаза | Сложность | Story Points |
|------|-----------|--------------|
| Фаза 1 | Низкая | 13 |
| Фаза 2 | Средняя | 21 |
| Фаза 3 | Средняя | 21 |
| Фаза 4 | Средняя | 18 |
| Фаза 5 | Высокая | 34 |
| Фаза 6 | Высокая | 26 |
| Фаза 7 | Средняя | 21 |
| **Итого** | | **154** |
