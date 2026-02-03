# Детальный технический бэклог: AWM.Service (ETMS v4.0)

> **Дата создания:** 2026-02-03  
> **Версия:** 1.0  
> **Статус:** Утверждено для распределения

---

## 📁 Фаза 1: Организационная структура (Org/Edu)

### 1.1 Управление Институтами и Кафедрами

**📂 Application Layer** (`src/AWM.Service.Application/Features/Org/`)

| Файл | Описание |
|------|----------|
| `Commands/Institutes/CreateInstitute/CreateInstituteCommand.cs` | Команда создания института |
| `Commands/Institutes/CreateInstitute/CreateInstituteCommandHandler.cs` | Хендлер |
| `Commands/Institutes/UpdateInstitute/UpdateInstituteCommand.cs` | Обновление |
| `Commands/Institutes/UpdateInstitute/UpdateInstituteCommandHandler.cs` | Хендлер |
| `Commands/Institutes/DeleteInstitute/DeleteInstituteCommand.cs` | Мягкое удаление |
| `Commands/Departments/CreateDepartment/CreateDepartmentCommand.cs` | Аналогично |
| `Commands/Departments/UpdateDepartment/...` | |
| `Queries/Institutes/GetAllInstitutes/GetAllInstitutesQuery.cs` | Список институтов |
| `Queries/Institutes/GetInstituteById/GetInstituteByIdQuery.cs` | По ID |
| `Queries/Departments/GetDepartmentsByInstitute/...` | Кафедры института |
| `DTOs/InstituteDto.cs` | DTO для ответов |
| `DTOs/DepartmentDto.cs` | |

**📂 WebAPI Layer** (`src/AWM.Service.WebAPI/Controllers/v1/`)

| Файл | Эндпоинты |
|------|-----------|
| `InstitutesController.cs` | `GET /api/v1/institutes`, `GET /api/v1/institutes/{id}`, `POST`, `PUT`, `DELETE` |
| `DepartmentsController.cs` | `GET /api/v1/departments`, `GET /api/v1/institutes/{instituteId}/departments`, `POST`, `PUT`, `DELETE` |

---

### 1.2 Образовательные программы и Уровни степеней

**📂 Application Layer** (`src/AWM.Service.Application/Features/Edu/`)

| Файл | Описание |
|------|----------|
| `Commands/AcademicPrograms/CreateAcademicProgram/CreateAcademicProgramCommand.cs` | |
| `Commands/AcademicPrograms/CreateAcademicProgram/CreateAcademicProgramCommandHandler.cs` | |
| `Commands/DegreeLevels/CreateDegreeLevel/...` | |
| `Queries/AcademicPrograms/GetAcademicPrograms/GetAcademicProgramsQuery.cs` | С фильтрацией |
| `Queries/DegreeLevels/GetDegreeLevels/GetDegreeLevelsQuery.cs` | |
| `DTOs/AcademicProgramDto.cs` | |
| `DTOs/DegreeLevelDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `AcademicProgramsController.cs` | `GET /api/v1/academic-programs`, `POST`, `PUT` |
| `DegreeLevelsController.cs` | `GET /api/v1/degree-levels` |

---

### 1.3 Управление Студентами и Сотрудниками

**📂 Application Layer** (`src/AWM.Service.Application/Features/Edu/`)

| Файл | Описание |
|------|----------|
| `Commands/Students/CreateStudent/CreateStudentCommand.cs` | Создание профиля студента |
| `Commands/Students/UpdateStudent/UpdateStudentCommand.cs` | Обновление |
| `Commands/Staff/CreateStaff/CreateStaffCommand.cs` | Профиль сотрудника |
| `Commands/Staff/UpdateStaff/UpdateStaffCommand.cs` | |
| `Queries/Students/GetStudentById/GetStudentByIdQuery.cs` | |
| `Queries/Students/GetStudentsByDepartment/GetStudentsByDepartmentQuery.cs` | |
| `Queries/Staff/GetStaffByDepartment/GetStaffByDepartmentQuery.cs` | |
| `Queries/Staff/GetSupervisors/GetSupervisorsQuery.cs` | Только научные руководители |
| `DTOs/StudentDto.cs` | |
| `DTOs/StaffDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `StudentsController.cs` | `GET /api/v1/students`, `GET /api/v1/students/{id}`, `POST`, `PUT` |
| `StaffController.cs` | `GET /api/v1/staff`, `GET /api/v1/staff/supervisors`, `POST`, `PUT` |

---

## 📁 Фаза 2: Периоды и Воркфлоу

### 2.1 Управление периодами

**📂 Application Layer** (`src/AWM.Service.Application/Features/Common/`)

| Файл | Описание |
|------|----------|
| `Commands/Periods/CreatePeriod/CreatePeriodCommand.cs` | |
| `Commands/Periods/CreatePeriod/CreatePeriodCommandHandler.cs` | |
| `Commands/Periods/UpdatePeriod/UpdatePeriodCommand.cs` | |
| `Queries/Periods/GetPeriodsByDepartment/GetPeriodsByDepartmentQuery.cs` | |
| `Queries/Periods/GetActivePeriod/GetActivePeriodQuery.cs` | Текущий активный период |
| `Services/IPeriodValidationService.cs` | Интерфейс (в Domain) |
| `Services/PeriodValidationService.cs` | Реализация |
| `DTOs/PeriodDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `PeriodsController.cs` | `GET /api/v1/departments/{deptId}/periods`, `POST`, `PUT` |

---

### 2.2 Машина состояний (Workflow Engine)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Workflow/`)

| Файл | Описание |
|------|----------|
| `Services/IWorkflowService.cs` | Интерфейс машины состояний |
| `Services/WorkflowService.cs` | Реализация переходов |
| `Commands/TransitionState/TransitionStateCommand.cs` | Перевод сущности в новое состояние |
| `Commands/TransitionState/TransitionStateCommandHandler.cs` | |
| `Queries/GetAvailableTransitions/GetAvailableTransitionsQuery.cs` | Доступные переходы для текущего пользователя |
| `DTOs/StateDto.cs` | |
| `DTOs/TransitionDto.cs` | |

**📂 Domain Layer** (`src/AWM.Service.Domain/Wf/`)

| Файл | Описание |
|------|----------|
| `Interfaces/IWorkflowService.cs` | Интерфейс для DI |

---

## 📁 Фаза 3: Направления и Темы

### 3.1 Управление направлениями (Directions)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Directions/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateDirection/CreateDirectionCommand.cs` | НР создает направление |
| `Commands/CreateDirection/CreateDirectionCommandHandler.cs` | |
| `Commands/UpdateDirection/UpdateDirectionCommand.cs` | Редактирование |
| `Commands/SubmitDirection/SubmitDirectionCommand.cs` | Отправка на рассмотрение кафедрой |
| `Commands/ApproveDirection/ApproveDirectionCommand.cs` | Кафедра утверждает |
| `Commands/RejectDirection/RejectDirectionCommand.cs` | Кафедра отклоняет |
| `Commands/RequestRevision/RequestRevisionCommand.cs` | На доработку |
| `Queries/GetDirectionsByDepartment/GetDirectionsByDepartmentQuery.cs` | Все направления кафедры |
| `Queries/GetDirectionsBySupervisor/GetDirectionsBySupervisorQuery.cs` | Направления НР |
| `Queries/GetDirectionById/GetDirectionByIdQuery.cs` | |
| `DTOs/DirectionDto.cs` | |
| `DTOs/DirectionDetailDto.cs` | С полной информацией |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `DirectionsController.cs` | `GET /api/v1/directions`, `GET /api/v1/directions/{id}`, `POST`, `PUT`, `POST /api/v1/directions/{id}/submit`, `POST /api/v1/directions/{id}/approve`, `POST /api/v1/directions/{id}/reject`, `POST /api/v1/directions/{id}/request-revision` |

---

### 3.2 Управление темами (Topics)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Topics/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateTopic/CreateTopicCommand.cs` | НР создает тему |
| `Commands/CreateTopic/CreateTopicCommandHandler.cs` | |
| `Commands/UpdateTopic/UpdateTopicCommand.cs` | |
| `Commands/ApproveTopic/ApproveTopicCommand.cs` | Кафедра утверждает |
| `Commands/CloseTopic/CloseTopicCommand.cs` | Закрытие темы |
| `Queries/GetTopicsByDirection/GetTopicsByDirectionQuery.cs` | Темы направления |
| `Queries/GetAvailableTopics/GetAvailableTopicsQuery.cs` | Доступные для выбора студентами |
| `Queries/GetTopicById/GetTopicByIdQuery.cs` | |
| `DTOs/TopicDto.cs` | |
| `DTOs/TopicDetailDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `TopicsController.cs` | `GET /api/v1/topics`, `GET /api/v1/topics/available`, `GET /api/v1/directions/{dirId}/topics`, `POST`, `PUT`, `POST /api/v1/topics/{id}/approve`, `POST /api/v1/topics/{id}/close` |

---

## 📁 Фаза 4: Заявки и Выбор тем

### 4.1 Заявки студентов (Topic Applications)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Applications/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateApplication/CreateApplicationCommand.cs` | Студент подает заявку |
| `Commands/CreateApplication/CreateApplicationCommandHandler.cs` | |
| `Commands/AcceptApplication/AcceptApplicationCommand.cs` | НР принимает |
| `Commands/RejectApplication/RejectApplicationCommand.cs` | НР отклоняет |
| `Commands/WithdrawApplication/WithdrawApplicationCommand.cs` | Студент отзывает |
| `Queries/GetApplicationsByTopic/GetApplicationsByTopicQuery.cs` | Заявки на тему (для НР) |
| `Queries/GetApplicationsByStudent/GetApplicationsByStudentQuery.cs` | Заявки студента |
| `DTOs/TopicApplicationDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `TopicApplicationsController.cs` | `GET /api/v1/applications`, `GET /api/v1/topics/{topicId}/applications`, `POST /api/v1/topics/{topicId}/apply`, `POST /api/v1/applications/{id}/accept`, `POST /api/v1/applications/{id}/reject`, `DELETE /api/v1/applications/{id}` |

---

### 4.2 Создание работ и участников

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Works/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateStudentWork/CreateStudentWorkCommand.cs` | Автоматически при принятии заявки |
| `Commands/AddParticipant/AddParticipantCommand.cs` | Добавление участника в команду |
| `Commands/RemoveParticipant/RemoveParticipantCommand.cs` | |
| `Queries/GetStudentWorkById/GetStudentWorkByIdQuery.cs` | |
| `Queries/GetStudentWorksByDepartment/GetStudentWorksByDepartmentQuery.cs` | Для кафедры |
| `Queries/GetStudentWorksBySupervisor/GetStudentWorksBySupervisorQuery.cs` | Для НР |
| `Queries/GetMyWork/GetMyWorkQuery.cs` | Для текущего студента |
| `DTOs/StudentWorkDto.cs` | |
| `DTOs/WorkParticipantDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `StudentWorksController.cs` | `GET /api/v1/works`, `GET /api/v1/works/{id}`, `GET /api/v1/works/my`, `POST /api/v1/works/{id}/participants` |

---

## 📁 Фаза 5: Предзащита и проверки

### 5.1 Комиссии

**📂 Application Layer** (`src/AWM.Service.Application/Features/Defense/Commissions/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateCommission/CreateCommissionCommand.cs` | |
| `Commands/AddCommissionMember/AddCommissionMemberCommand.cs` | |
| `Commands/RemoveCommissionMember/RemoveCommissionMemberCommand.cs` | |
| `Queries/GetCommissionsByDepartment/GetCommissionsByDepartmentQuery.cs` | |
| `DTOs/CommissionDto.cs` | |
| `DTOs/CommissionMemberDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `CommissionsController.cs` | `GET /api/v1/commissions`, `POST`, `PUT`, `POST /api/v1/commissions/{id}/members`, `DELETE /api/v1/commissions/{id}/members/{memberId}` |

---

### 5.2 Проверки качества (Quality Checks)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/QualityChecks/`)

| Файл | Описание |
|------|----------|
| `Commands/SubmitForCheck/SubmitForCheckCommand.cs` | Студент сдает на проверку |
| `Commands/RecordCheckResult/RecordCheckResultCommand.cs` | Эксперт фиксирует результат |
| `Queries/GetChecksByWork/GetChecksByWorkQuery.cs` | История проверок работы |
| `Queries/GetPendingChecks/GetPendingChecksQuery.cs` | Ожидающие проверки (для эксперта) |
| `DTOs/QualityCheckDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `QualityChecksController.cs` | `GET /api/v1/works/{workId}/checks`, `GET /api/v1/quality-checks/pending`, `POST /api/v1/works/{workId}/submit-check`, `POST /api/v1/quality-checks/{id}/result` |

---

### 5.3 Предзащиты

**📂 Application Layer** (`src/AWM.Service.Application/Features/Defense/PreDefense/`)

| Файл | Описание |
|------|----------|
| `Commands/SchedulePreDefense/SchedulePreDefenseCommand.cs` | Назначение даты |
| `Commands/RecordAttendance/RecordAttendanceCommand.cs` | Отметка явки |
| `Commands/SubmitPreDefenseGrade/SubmitPreDefenseGradeCommand.cs` | Член комиссии ставит оценку |
| `Commands/FinalizePreDefense/FinalizePreDefenseCommand.cs` | Секретарь финализирует |
| `Queries/GetPreDefenseSchedule/GetPreDefenseScheduleQuery.cs` | |
| `Queries/GetPreDefenseAttempts/GetPreDefenseAttemptsQuery.cs` | |
| `DTOs/PreDefenseAttemptDto.cs` | |
| `DTOs/PreDefenseScheduleDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `PreDefenseController.cs` | `GET /api/v1/pre-defense/schedule`, `POST /api/v1/pre-defense/schedule`, `POST /api/v1/pre-defense/{id}/attendance`, `POST /api/v1/pre-defense/{id}/grade`, `POST /api/v1/pre-defense/{id}/finalize` |

---

## 📁 Фаза 6: Финальная защита

### 6.1 Расписание защит

**📂 Application Layer** (`src/AWM.Service.Application/Features/Defense/Schedule/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateDefenseSchedule/CreateDefenseScheduleCommand.cs` | |
| `Commands/AssignWorkToSlot/AssignWorkToSlotCommand.cs` | |
| `Queries/GetDefenseSchedule/GetDefenseScheduleQuery.cs` | |
| `DTOs/ScheduleDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `DefenseScheduleController.cs` | `GET /api/v1/defense/schedule`, `POST`, `POST /api/v1/defense/schedule/{slotId}/assign` |

---

### 6.2 Оценивание и Протоколы

**📂 Application Layer** (`src/AWM.Service.Application/Features/Defense/Evaluation/`)

| Файл | Описание |
|------|----------|
| `Commands/SubmitGrade/SubmitGradeCommand.cs` | Член ГАК ставит оценку |
| `Commands/FinalizeDefense/FinalizeDefenseCommand.cs` | Секретарь финализирует |
| `Commands/GenerateProtocol/GenerateProtocolCommand.cs` | Генерация протокола |
| `Queries/GetEvaluationCriteria/GetEvaluationCriteriaQuery.cs` | Критерии оценки |
| `Queries/GetGradesByWork/GetGradesByWorkQuery.cs` | |
| `Queries/GetProtocol/GetProtocolQuery.cs` | |
| `DTOs/GradeDto.cs` | |
| `DTOs/ProtocolDto.cs` | |
| `DTOs/EvaluationCriteriaDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `EvaluationController.cs` | `GET /api/v1/evaluation/criteria`, `POST /api/v1/works/{workId}/grades`, `POST /api/v1/works/{workId}/finalize-defense` |
| `ProtocolsController.cs` | `GET /api/v1/protocols/{id}`, `POST /api/v1/works/{workId}/protocol` |

---

## 📁 Фаза 7: Сквозные сервисы

### 7.1 Уведомления

**📂 Application Layer** (`src/AWM.Service.Application/Features/Common/Notifications/`)

| Файл | Описание |
|------|----------|
| `Services/INotificationService.cs` | Интерфейс |
| `Services/NotificationService.cs` | Реализация |
| `Commands/MarkAsRead/MarkAsReadCommand.cs` | |
| `Queries/GetMyNotifications/GetMyNotificationsQuery.cs` | |
| `DTOs/NotificationDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `NotificationsController.cs` | `GET /api/v1/notifications`, `POST /api/v1/notifications/{id}/read` |

---

### 7.2 Файловое хранилище (Attachments)

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Attachments/`)

| Файл | Описание |
|------|----------|
| `Services/IAttachmentService.cs` | Интерфейс |
| `Commands/UploadAttachment/UploadAttachmentCommand.cs` | |
| `Commands/DeleteAttachment/DeleteAttachmentCommand.cs` | |
| `Queries/GetAttachmentsByWork/GetAttachmentsByWorkQuery.cs` | |
| `DTOs/AttachmentDto.cs` | |

**📂 Infrastructure Layer** (`src/AWM.Service.Infrastructure/FileStorage/`)

| Файл | Описание |
|------|----------|
| `LocalFileStorageService.cs` | Локальное хранилище |
| `S3FileStorageService.cs` | AWS S3 (опционально) |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `AttachmentsController.cs` | `GET /api/v1/works/{workId}/attachments`, `POST /api/v1/works/{workId}/attachments`, `DELETE /api/v1/attachments/{id}`, `GET /api/v1/attachments/{id}/download` |

---

### 7.3 Отзывы и Рецензии

**📂 Application Layer** (`src/AWM.Service.Application/Features/Thesis/Reviews/`)

| Файл | Описание |
|------|----------|
| `Commands/CreateSupervisorReview/CreateSupervisorReviewCommand.cs` | Отзыв НР |
| `Commands/UploadReview/UploadReviewCommand.cs` | Загрузка рецензии |
| `Queries/GetReviewsByWork/GetReviewsByWorkQuery.cs` | |
| `DTOs/SupervisorReviewDto.cs` | |
| `DTOs/ReviewDto.cs` | |

**📂 WebAPI Layer**

| Файл | Эндпоинты |
|------|-----------|
| `ReviewsController.cs` | `GET /api/v1/works/{workId}/reviews`, `POST /api/v1/works/{workId}/supervisor-review`, `POST /api/v1/works/{workId}/external-review` |

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
