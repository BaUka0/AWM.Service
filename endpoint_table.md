| Controller | HTTP | Route template | Action | Route params | Query params | Body params | Payload type (by signature) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| AcademicProgramsController | GET | api/v{version:apiVersion}/academic-programs | GetAcademicPrograms | - | departmentId: int?; degreeLevelId: int?; code: string?; name: string?; includeDeleted: bool | - | IActionResult (нет T в сигнатуре) |
| AcademicProgramsController | POST | api/v{version:apiVersion}/academic-programs | CreateAcademicProgram | - | - | request: CreateAcademicProgramRequest | IActionResult (нет T в сигнатуре) |
| AcademicProgramsController | PUT | api/v{version:apiVersion}/academic-programs/{id} | UpdateAcademicProgram | id: int | - | request: UpdateAcademicProgramRequest | IActionResult (нет T в сигнатуре) |
| AttachmentsController | GET | api/v{version:apiVersion}/works/{workId:long}/attachments | GetAll | workId: long | - | - | IActionResult (нет T в сигнатуре) |
| AttachmentsController | POST | api/v{version:apiVersion}/works/{workId:long}/attachments | Upload | workId: long | - | request: UploadAttachmentRequest (form) | IActionResult (нет T в сигнатуре) |
| AttachmentsController | DELETE | api/v{version:apiVersion}/works/{workId:long}/attachments/{attachmentId:long} | Delete | workId: long; attachmentId: long | - | - | IActionResult (нет T в сигнатуре) |
| AttachmentsController | GET | api/v{version:apiVersion}/works/{workId:long}/attachments/{attachmentId:long} | GetById | workId: long; attachmentId: long | - | - | IActionResult (нет T в сигнатуре) |
| AttachmentsController | GET | api/v{version:apiVersion}/works/{workId:long}/attachments/{attachmentId:long}/download | Download | workId: long; attachmentId: long | - | - | IActionResult (нет T в сигнатуре) |
| AuthController | POST | api/v{version:apiVersion}/auth/login | Login | - | - | request: LoginRequest | IActionResult (нет T в сигнатуре) |
| AuthController | POST | api/v{version:apiVersion}/auth/register | Register | - | - | request: RegisterRequest | IActionResult (нет T в сигнатуре) |
| CommissionsController | GET | api/v{version:apiVersion}/commissions | GetByDepartment | - | departmentId: int; academicYearId: int | - | IActionResult (нет T в сигнатуре) |
| CommissionsController | POST | api/v{version:apiVersion}/commissions | Create | - | - | request: CreateCommissionRequest | IActionResult (нет T в сигнатуре) |
| CommissionsController | GET | api/v{version:apiVersion}/commissions/{id:int} | GetById | id: int | - | - | IActionResult (нет T в сигнатуре) |
| CommissionsController | PUT | api/v{version:apiVersion}/commissions/{id:int} | Update | id: int | - | request: UpdateCommissionRequest | IActionResult (нет T в сигнатуре) |
| CommissionsController | POST | api/v{version:apiVersion}/commissions/{id:int}/members | AddMember | id: int | - | request: AddCommissionMemberRequest | IActionResult (нет T в сигнатуре) |
| CommissionsController | DELETE | api/v{version:apiVersion}/commissions/{id:int}/members/{memberId:int} | RemoveMember | id: int; memberId: int | - | - | IActionResult (нет T в сигнатуре) |
| DefenseScheduleController | GET | api/v{version:apiVersion}/defense-schedule | GetSchedule | - | commissionId: int | - | IActionResult (нет T в сигнатуре) |
| DefenseScheduleController | POST | api/v{version:apiVersion}/defense-schedule | Create | - | - | request: CreateDefenseScheduleRequest | IActionResult (нет T в сигнатуре) |
| DefenseScheduleController | PUT | api/v{version:apiVersion}/defense-schedule/{scheduleId:long} | Update | scheduleId: long | - | request: UpdateDefenseScheduleRequest | IActionResult (нет T в сигнатуре) |
| DefenseScheduleController | POST | api/v{version:apiVersion}/defense-schedule/{scheduleId:long}/assign | AssignWork | scheduleId: long | - | request: AssignWorkToSlotRequest | IActionResult (нет T в сигнатуре) |
| DefenseScheduleController | GET | api/v{version:apiVersion}/defense-schedule/{slotId:long} | GetSlotById | slotId: long | - | - | IActionResult (нет T в сигнатуре) |
| DegreeLevelsController | GET | api/v{version:apiVersion}/degree-levels | GetDegreeLevels | - | name: string?; minDurationYears: int?; maxDurationYears: int? | - | IActionResult (нет T в сигнатуре) |
| DegreeLevelsController | POST | api/v{version:apiVersion}/degree-levels | CreateDegreeLevel | - | - | request: CreateDegreeLevelRequest | IActionResult (нет T в сигнатуре) |
| DepartmentsController | DELETE | api/v{version:apiVersion}/departments/{departmentId} | Delete | departmentId: int | - | - | IActionResult (нет T в сигнатуре) |
| DepartmentsController | PUT | api/v{version:apiVersion}/departments/{departmentId} | Update | departmentId: int | - | request: UpdateDepartmentRequest | IActionResult (нет T в сигнатуре) |
| DepartmentsController | GET | api/v{version:apiVersion}/institutes/{instituteId}/departments | GetByInstituteId | instituteId: int | - | - | IActionResult (нет T в сигнатуре) |
| DepartmentsController | POST | api/v{version:apiVersion}/institutes/{instituteId}/departments | Create | instituteId: int | - | request: CreateDepartmentRequest | IActionResult (нет T в сигнатуре) |
| DirectionsController | POST | api/v{version:apiVersion}/directions | CreateDirection | - | - | request: CreateDirectionRequest | IActionResult (нет T в сигнатуре) |
| DirectionsController | GET | api/v{version:apiVersion}/directions/{id} | GetDirectionById | id: long | includeDeleted: bool | - | IActionResult (нет T в сигнатуре) |
| DirectionsController | PUT | api/v{version:apiVersion}/directions/{id} | UpdateDirection | id: long | - | request: UpdateDirectionRequest | IActionResult (нет T в сигнатуре) |
| DirectionsController | POST | api/v{version:apiVersion}/directions/{id}/approve | ApproveDirection | id: long | - | - | IActionResult (нет T в сигнатуре) |
| DirectionsController | POST | api/v{version:apiVersion}/directions/{id}/reject | RejectDirection | id: long | - | request: RejectDirectionRequest | IActionResult (нет T в сигнатуре) |
| DirectionsController | POST | api/v{version:apiVersion}/directions/{id}/request-revision | RequestRevision | id: long | - | request: RequestRevisionRequest | IActionResult (нет T в сигнатуре) |
| DirectionsController | POST | api/v{version:apiVersion}/directions/{id}/submit | SubmitDirection | id: long | - | - | IActionResult (нет T в сигнатуре) |
| DirectionsController | GET | api/v{version:apiVersion}/directions/by-department | GetDirectionsByDepartment | - | request: GetDirectionsByDepartmentRequest | - | IActionResult (нет T в сигнатуре) |
| DirectionsController | GET | api/v{version:apiVersion}/directions/by-supervisor | GetDirectionsBySupervisor | - | supervisorId: int; academicYearId: int; workTypeId: int?; stateId: int?; includeDeleted: bool | - | IActionResult (нет T в сигнатуре) |
| EvaluationController | GET | api/v{version:apiVersion}/evaluation/criteria | GetCriteria | - | workTypeId: int; departmentId: int? | - | IActionResult (нет T в сигнатуре) |
| EvaluationController | PUT | api/v{version:apiVersion}/evaluation/schedule/{scheduleId:long}/finalize | FinalizeDefense | scheduleId: long | - | - | IActionResult (нет T в сигнатуре) |
| EvaluationController | GET | api/v{version:apiVersion}/evaluation/schedule/{scheduleId:long}/grades | GetGrades | scheduleId: long | - | - | IActionResult (нет T в сигнатуре) |
| EvaluationController | POST | api/v{version:apiVersion}/evaluation/schedule/{scheduleId:long}/grades | SubmitGrade | scheduleId: long | - | request: SubmitGradeRequest | IActionResult (нет T в сигнатуре) |
| InstitutesController | GET | api/v{version:apiVersion}/institutes | GetAll | - | universityId: int; includeDepartments: bool | - | IActionResult (нет T в сигнатуре) |
| InstitutesController | POST | api/v{version:apiVersion}/institutes | Create | - | - | request: CreateInstituteRequest | IActionResult (нет T в сигнатуре) |
| InstitutesController | DELETE | api/v{version:apiVersion}/institutes/{instituteId} | Delete | instituteId: int | - | - | IActionResult (нет T в сигнатуре) |
| InstitutesController | GET | api/v{version:apiVersion}/institutes/{instituteId} | GetById | instituteId: int | includeDepartments: bool | - | IActionResult (нет T в сигнатуре) |
| InstitutesController | PUT | api/v{version:apiVersion}/institutes/{instituteId} | Update | instituteId: int | - | request: UpdateInstituteRequest | IActionResult (нет T в сигнатуре) |
| NotificationsController | GET | api/v{version:apiVersion}/notifications | GetMyNotifications | - | skip: int; take: int; onlyUnread: bool? | - | IActionResult (нет T в сигнатуре) |
| NotificationsController | PATCH | api/v{version:apiVersion}/notifications/{notificationId:long}/read | MarkAsRead | notificationId: long | - | - | IActionResult (нет T в сигнатуре) |
| NotificationsController | PATCH | api/v{version:apiVersion}/notifications/read-all | MarkAllAsRead | - | - | - | IActionResult (нет T в сигнатуре) |
| NotificationsController | GET | api/v{version:apiVersion}/notifications/unread-count | GetUnreadCount | - | - | - | IActionResult (нет T в сигнатуре) |
| PeriodsController | GET | api/v{version:apiVersion}/departments/{departmentId}/periods | GetAll | departmentId: int | academicYearId: int | - | IActionResult (нет T в сигнатуре) |
| PeriodsController | POST | api/v{version:apiVersion}/departments/{departmentId}/periods | Create | departmentId: int | - | request: CreatePeriodRequest | IActionResult (нет T в сигнатуре) |
| PeriodsController | PUT | api/v{version:apiVersion}/departments/{departmentId}/periods/{periodId} | Update | departmentId: int; periodId: int | - | request: UpdatePeriodRequest | IActionResult (нет T в сигнатуре) |
| PeriodsController | GET | api/v{version:apiVersion}/departments/{departmentId}/periods/active | GetActive | departmentId: int | academicYearId: int; stage: WorkflowStage | - | IActionResult (нет T в сигнатуре) |
| PreDefenseController | PUT | api/v{version:apiVersion}/pre-defense/attempts/{attemptId:long}/attendance | RecordAttendance | attemptId: long | - | request: RecordAttendanceRequest | IActionResult (нет T в сигнатуре) |
| PreDefenseController | PUT | api/v{version:apiVersion}/pre-defense/attempts/{attemptId:long}/finalize | Finalize | attemptId: long | - | request: FinalizePreDefenseRequest | IActionResult (нет T в сигнатуре) |
| PreDefenseController | GET | api/v{version:apiVersion}/pre-defense/schedule | GetSchedule | - | commissionId: int | - | IActionResult (нет T в сигнатуре) |
| PreDefenseController | POST | api/v{version:apiVersion}/pre-defense/schedule/{scheduleId:long}/grades | SubmitGrade | scheduleId: long | - | request: SubmitPreDefenseGradeRequest | IActionResult (нет T в сигнатуре) |
| PreDefenseController | GET | api/v{version:apiVersion}/pre-defense/works/{workId:long}/attempts | GetAttempts | workId: long | - | - | IActionResult (нет T в сигнатуре) |
| PreDefenseController | POST | api/v{version:apiVersion}/pre-defense/works/{workId:long}/schedule | Schedule | workId: long | - | request: SchedulePreDefenseRequest | IActionResult (нет T в сигнатуре) |
| ProtocolsController | POST | api/v{version:apiVersion}/protocols | Generate | - | - | request: GenerateProtocolRequest | IActionResult (нет T в сигнатуре) |
| ProtocolsController | GET | api/v{version:apiVersion}/protocols/{protocolId:long} | GetById | protocolId: long | - | - | IActionResult (нет T в сигнатуре) |
| QualityChecksController | GET | api/v{version:apiVersion}/quality-checks/by-work/{workId:long} | GetByWork | workId: long | - | - | IActionResult (нет T в сигнатуре) |
| QualityChecksController | GET | api/v{version:apiVersion}/quality-checks/pending | GetPending | - | departmentId: int; academicYearId: int; checkType: CheckType? | - | IActionResult (нет T в сигнатуре) |
| QualityChecksController | PUT | api/v{version:apiVersion}/quality-checks/works/{workId:long}/checks/{checkId:long}/record | RecordResult | workId: long; checkId: long | - | request: RecordCheckResultRequest | IActionResult (нет T в сигнатуре) |
| QualityChecksController | POST | api/v{version:apiVersion}/quality-checks/works/{workId:long}/submit | Submit | workId: long | - | request: SubmitForCheckRequest | IActionResult (нет T в сигнатуре) |
| ReviewsController | GET | api/v{version:apiVersion}/works/{workId:long}/reviews | GetReviewsByWork | workId: long | - | - | IActionResult (нет T в сигнатуре) |
| ReviewsController | POST | api/v{version:apiVersion}/works/{workId:long}/reviews/external/{reviewId:long} | UploadExternalReview | workId: long | - | request: UploadReviewRequest (form) | IActionResult (нет T в сигнатуре) |
| ReviewsController | POST | api/v{version:apiVersion}/works/{workId:long}/reviews/supervisor | CreateOrUpdateSupervisorReview | workId: long | - | request: CreateSupervisorReviewRequest (form) | IActionResult (нет T в сигнатуре) |
| StaffController | GET | api/v{version:apiVersion}/staff | GetByDepartment | - | departmentId: int | - | IActionResult (нет T в сигнатуре) |
| StaffController | POST | api/v{version:apiVersion}/staff | Create | - | - | request: CreateStaffRequest | IActionResult (нет T в сигнатуре) |
| StaffController | PUT | api/v{version:apiVersion}/staff/{staffId} | Update | staffId: int | - | request: UpdateStaffRequest | IActionResult (нет T в сигнатуре) |
| StudentsController | GET | api/v{version:apiVersion}/students | GetByProgram | - | programId: int | - | IActionResult (нет T в сигнатуре) |
| StudentsController | POST | api/v{version:apiVersion}/students | Create | - | - | request: CreateStudentRequest | IActionResult (нет T в сигнатуре) |
| StudentsController | GET | api/v{version:apiVersion}/students/{studentId} | GetById | studentId: int | - | - | IActionResult (нет T в сигнатуре) |
| StudentsController | PUT | api/v{version:apiVersion}/students/{studentId} | Update | studentId: int | - | request: UpdateStudentRequest | IActionResult (нет T в сигнатуре) |
| StudentWorksController | POST | api/v{version:apiVersion}/works | Create | - | - | request: CreateStudentWorkRequest | IActionResult (нет T в сигнатуре) |
| StudentWorksController | GET | api/v{version:apiVersion}/works/{id:long} | GetById | id: long | - | - | IActionResult (нет T в сигнатуре) |
| StudentWorksController | POST | api/v{version:apiVersion}/works/{workId:long}/participants | AddParticipant | workId: long | - | request: AddParticipantRequest | IActionResult (нет T в сигнатуре) |
| StudentWorksController | DELETE | api/v{version:apiVersion}/works/{workId:long}/participants/{studentId:int} | RemoveParticipant | workId: long; studentId: int | - | - | IActionResult (нет T в сигнатуре) |
| StudentWorksController | GET | api/v{version:apiVersion}/works/by-department | GetByDepartment | - | departmentId: int; academicYearId: int | - | IActionResult (нет T в сигнатуре) |
| StudentWorksController | GET | api/v{version:apiVersion}/works/my | GetMyWorks | - | - | - | IActionResult (нет T в сигнатуре) |
| StudentWorksController | GET | api/v{version:apiVersion}/works/supervisor/{supervisorId:int} | GetBySupervisor | supervisorId: int | academicYearId: int? | - | IActionResult (нет T в сигнатуре) |
| TopicApplicationsController | POST | api/v{version:apiVersion}/applications | Create | - | - | request: CreateApplicationRequest | IActionResult (нет T в сигнатуре) |
| TopicApplicationsController | DELETE | api/v{version:apiVersion}/applications/{applicationId:long} | Withdraw | applicationId: long | - | - | IActionResult (нет T в сигнатуре) |
| TopicApplicationsController | POST | api/v{version:apiVersion}/applications/{applicationId:long}/accept | Accept | applicationId: long | - | - | IActionResult (нет T в сигнатуре) |
| TopicApplicationsController | POST | api/v{version:apiVersion}/applications/{applicationId:long}/reject | Reject | applicationId: long | - | request: RejectApplicationRequest | IActionResult (нет T в сигнатуре) |
| TopicApplicationsController | GET | api/v{version:apiVersion}/applications/by-topic/{topicId:long} | GetByTopic | topicId: long | status: ApplicationStatus? | - | IActionResult (нет T в сигнатуре) |
| TopicApplicationsController | GET | api/v{version:apiVersion}/applications/my | GetMyApplications | - | academicYearId: int? | - | IActionResult (нет T в сигнатуре) |
| TopicsController | POST | api/v{version:apiVersion}/topics | Create | - | - | request: CreateTopicRequest | IActionResult (нет T в сигнатуре) |
| TopicsController | GET | api/v{version:apiVersion}/topics/{id} | GetById | id: long | - | - | IActionResult (нет T в сигнатуре) |
| TopicsController | PUT | api/v{version:apiVersion}/topics/{id} | Update | id: long | - | request: UpdateTopicRequest | IActionResult (нет T в сигнатуре) |
| TopicsController | POST | api/v{version:apiVersion}/topics/{id}/approve | Approve | id: long | - | - | IActionResult (нет T в сигнатуре) |
| TopicsController | POST | api/v{version:apiVersion}/topics/{id}/close | Close | id: long | - | - | IActionResult (нет T в сигнатуре) |
| TopicsController | GET | api/v{version:apiVersion}/topics/available | GetAvailable | - | departmentId: int; academicYearId: int | - | IActionResult (нет T в сигнатуре) |
| TopicsController | GET | api/v{version:apiVersion}/topics/by-direction/{directionId} | GetByDirection | directionId: long | - | - | IActionResult (нет T в сигнатуре) |
COUNT=95
