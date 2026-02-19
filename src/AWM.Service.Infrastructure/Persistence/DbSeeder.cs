namespace AWM.Service.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Wf.Entities;

/// <summary>
/// Seeds test data for all CQRS handlers and context-aware RBAC testing.
/// All FK references resolved via Select queries (sequences continue after data deletion).
/// Only seeds when tables are empty.
/// </summary>
public static class DbSeeder
{
    private const string TestPassword = "Test123!";

    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher passwordHasher)
    {
        // =======================================================
        // 1. ORG: University
        // =======================================================
        if (!await db.Universities.AnyAsync())
        {
            db.Universities.Add(new University("Тестовый Университет", "TESTUNI"));
            await db.SaveChangesAsync();
        }

        var university = await db.Universities.FirstAsync(u => u.Code == "TESTUNI");

        // =======================================================
        // 2. ORG: Institutes
        // =======================================================
        if (!await db.Institutes.AnyAsync())
        {
            db.Institutes.AddRange(
                new[]
                {
                    university.AddInstitute("Институт информационных технологий", 0),
                    university.AddInstitute("Институт инженерии", 0)
                });
            await db.SaveChangesAsync();

            // Set codes via raw update (Code setter is through UpdateCode)
            var itInstitute = await db.Institutes.FirstAsync(i => i.Name.Contains("информационных"));
            itInstitute.UpdateCode("IIT", 0);

            var engInstitute = await db.Institutes.FirstAsync(i => i.Name.Contains("инженерии"));
            engInstitute.UpdateCode("IE", 0);

            await db.SaveChangesAsync();
        }

        var instituteIT = await db.Institutes.FirstAsync(i => i.Code == "IIT");
        var instituteEng = await db.Institutes.FirstAsync(i => i.Code == "IE");

        // =======================================================
        // 3. ORG: Departments
        // =======================================================
        if (!await db.Departments.AnyAsync())
        {
            var deptCS = instituteIT.AddDepartment("Компьютерные науки", 0, "CS");
            var deptSE = instituteIT.AddDepartment("Программная инженерия", 0, "SE");
            var deptME = instituteEng.AddDepartment("Машиностроение", 0, "ME");
            await db.SaveChangesAsync();
        }

        var departmentCS = await db.Departments.FirstAsync(d => d.Code == "CS");
        var departmentSE = await db.Departments.FirstAsync(d => d.Code == "SE");
        var departmentME = await db.Departments.FirstAsync(d => d.Code == "ME");

        // =======================================================
        // 4. AUTH: Roles (all 8 RoleTypes)
        // =======================================================
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(
                Role.FromRoleType(RoleType.Admin, ScopeLevel.Global),
                Role.FromRoleType(RoleType.ViceRector, ScopeLevel.University),
                Role.FromRoleType(RoleType.HeadOfDepartment, ScopeLevel.Department),
                Role.FromRoleType(RoleType.Supervisor, ScopeLevel.Department),
                Role.FromRoleType(RoleType.Secretary, ScopeLevel.Department),
                Role.FromRoleType(RoleType.Expert, ScopeLevel.Department),
                Role.FromRoleType(RoleType.Student, ScopeLevel.Department),
                Role.FromRoleType(RoleType.CommissionMember, ScopeLevel.Commission)
            );
            await db.SaveChangesAsync();
        }

        var roleAdmin = await db.Roles.FirstAsync(r => r.SystemName == nameof(RoleType.Admin));
        var roleViceRector = await db.Roles.FirstAsync(r => r.SystemName == nameof(RoleType.ViceRector));
        var roleHeadDept = await db.Roles.FirstAsync(r => r.SystemName == nameof(RoleType.HeadOfDepartment));
        var roleSupervisor = await db.Roles.FirstAsync(r => r.SystemName == nameof(RoleType.Supervisor));
        var roleSecretary = await db.Roles.FirstAsync(r => r.SystemName == nameof(RoleType.Secretary));
        var roleExpert = await db.Roles.FirstAsync(r => r.SystemName == nameof(RoleType.Expert));
        var roleStudent = await db.Roles.FirstAsync(r => r.SystemName == nameof(RoleType.Student));
        var roleCommission = await db.Roles.FirstAsync(r => r.SystemName == nameof(RoleType.CommissionMember));

        // =======================================================
        // 5. AUTH: Users (8 test users, one per role)
        // =======================================================
        var hashedPassword = passwordHasher.HashPassword(TestPassword);

        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new User(university.Id, "admin", "admin@test.edu", hashedPassword),
                new User(university.Id, "vicerector", "vicerector@test.edu", hashedPassword),
                new User(university.Id, "head_cs", "head_cs@test.edu", hashedPassword),
                new User(university.Id, "supervisor1", "supervisor1@test.edu", hashedPassword),
                new User(university.Id, "supervisor2", "supervisor2@test.edu", hashedPassword),
                new User(university.Id, "secretary1", "secretary1@test.edu", hashedPassword),
                new User(university.Id, "expert1", "expert1@test.edu", hashedPassword),
                new User(university.Id, "commission1", "commission1@test.edu", hashedPassword),
                new User(university.Id, "student1", "student1@test.edu", hashedPassword),
                new User(university.Id, "student2", "student2@test.edu", hashedPassword),
                new User(university.Id, "student3", "student3@test.edu", hashedPassword)
            );
            await db.SaveChangesAsync();
        }

        var userAdmin = await db.Users.FirstAsync(u => u.Login == "admin");
        var userViceRector = await db.Users.FirstAsync(u => u.Login == "vicerector");
        var userHeadCS = await db.Users.FirstAsync(u => u.Login == "head_cs");
        var userSupervisor1 = await db.Users.FirstAsync(u => u.Login == "supervisor1");
        var userSupervisor2 = await db.Users.FirstAsync(u => u.Login == "supervisor2");
        var userSecretary1 = await db.Users.FirstAsync(u => u.Login == "secretary1");
        var userExpert1 = await db.Users.FirstAsync(u => u.Login == "expert1");
        var userCommission1 = await db.Users.FirstAsync(u => u.Login == "commission1");
        var userStudent1 = await db.Users.FirstAsync(u => u.Login == "student1");
        var userStudent2 = await db.Users.FirstAsync(u => u.Login == "student2");
        var userStudent3 = await db.Users.FirstAsync(u => u.Login == "student3");

        // =======================================================
        // 6. COMMON: Academic Year (current, 2025-2026)
        // =======================================================
        if (!await db.AcademicYears.AnyAsync())
        {
            var year = new AcademicYear(
                university.Id,
                "2025-2026",
                new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                userAdmin.Id);
            year.SetAsCurrent(userAdmin.Id);
            db.AcademicYears.Add(year);
            await db.SaveChangesAsync();
        }

        var academicYear = await db.AcademicYears.FirstAsync(y => y.Name == "2025-2026");

        // =======================================================
        // 7. AUTH: User Role Assignments (context-aware RBAC)
        // =======================================================
        if (!await db.UserRoleAssignments.AnyAsync())
        {
            // Admin — Global scope (no dept/inst/year context)
            userAdmin.AssignRole(roleAdmin.Id, assignedBy: userAdmin.Id);

            // ViceRector — University scope (no dept context, but with year)
            userViceRector.AssignRole(roleViceRector.Id, academicYearId: academicYear.Id, assignedBy: userAdmin.Id);

            // HeadOfDepartment — Department scope (CS dept, with year)
            userHeadCS.AssignRole(roleHeadDept.Id,
                departmentId: departmentCS.Id,
                instituteId: instituteIT.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            // Supervisor1 — Department scope (CS dept)
            userSupervisor1.AssignRole(roleSupervisor.Id,
                departmentId: departmentCS.Id,
                instituteId: instituteIT.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            // Supervisor2 — Department scope (SE dept)
            userSupervisor2.AssignRole(roleSupervisor.Id,
                departmentId: departmentSE.Id,
                instituteId: instituteIT.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            // Secretary — Department scope (CS dept)
            userSecretary1.AssignRole(roleSecretary.Id,
                departmentId: departmentCS.Id,
                instituteId: instituteIT.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            // Expert — Department scope (CS dept)
            userExpert1.AssignRole(roleExpert.Id,
                departmentId: departmentCS.Id,
                instituteId: instituteIT.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            // CommissionMember — Commission scope
            userCommission1.AssignRole(roleCommission.Id,
                departmentId: departmentCS.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            // Students — Department scope (CS dept)
            userStudent1.AssignRole(roleStudent.Id,
                departmentId: departmentCS.Id,
                instituteId: instituteIT.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            userStudent2.AssignRole(roleStudent.Id,
                departmentId: departmentCS.Id,
                instituteId: instituteIT.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            // Student3 — Different dept (SE) for cross-dept testing
            userStudent3.AssignRole(roleStudent.Id,
                departmentId: departmentSE.Id,
                instituteId: instituteIT.Id,
                academicYearId: academicYear.Id,
                assignedBy: userAdmin.Id);

            await db.SaveChangesAsync();
        }

        // =======================================================
        // 8. EDU: Degree Levels
        // =======================================================
        if (!await db.DegreeLevels.AnyAsync())
        {
            db.DegreeLevels.AddRange(
                DegreeLevel.Bachelor(userAdmin.Id),
                DegreeLevel.Master(userAdmin.Id),
                DegreeLevel.PhD(userAdmin.Id)
            );
            await db.SaveChangesAsync();
        }

        var bachelor = await db.DegreeLevels.FirstAsync(d => d.Name == "Bachelor");
        var master = await db.DegreeLevels.FirstAsync(d => d.Name == "Master");
        var phd = await db.DegreeLevels.FirstAsync(d => d.Name == "PhD");

        // =======================================================
        // 9. EDU: Academic Programs
        // =======================================================
        if (!await db.AcademicPrograms.AnyAsync())
        {
            db.AcademicPrograms.AddRange(
                new AcademicProgram(departmentCS.Id, bachelor.Id, "Информатика (бакалавр)", userAdmin.Id, "6B06101"),
                new AcademicProgram(departmentCS.Id, master.Id, "Информатика (магистр)", userAdmin.Id, "7M06101"),
                new AcademicProgram(departmentSE.Id, bachelor.Id, "Программная инженерия (бакалавр)", userAdmin.Id, "6B06102")
            );
            await db.SaveChangesAsync();
        }

        var programCSBachelor = await db.AcademicPrograms.FirstAsync(p => p.Code == "6B06101");
        var programCSMaster = await db.AcademicPrograms.FirstAsync(p => p.Code == "7M06101");
        var programSEBachelor = await db.AcademicPrograms.FirstAsync(p => p.Code == "6B06102");

        // =======================================================
        // 10. EDU: Staff
        // =======================================================
        if (!await db.Staff.AnyAsync())
        {
            db.Staff.AddRange(
                new Staff(userSupervisor1.Id, departmentCS.Id, userAdmin.Id, isSupervisor: true,
                    position: "Доцент", academicDegree: "PhD", maxStudentsLoad: 8),
                new Staff(userSupervisor2.Id, departmentSE.Id, userAdmin.Id, isSupervisor: true,
                    position: "Профессор", academicDegree: "DrSci", maxStudentsLoad: 5),
                new Staff(userExpert1.Id, departmentCS.Id, userAdmin.Id, isSupervisor: false,
                    position: "Старший преподаватель", academicDegree: "PhD", maxStudentsLoad: 1)
            );
            await db.SaveChangesAsync();
        }

        var staffSupervisor1 = await db.Staff.FirstAsync(s => s.UserId == userSupervisor1.Id);
        var staffSupervisor2 = await db.Staff.FirstAsync(s => s.UserId == userSupervisor2.Id);

        // =======================================================
        // 11. EDU: Students
        // =======================================================
        if (!await db.Students.AnyAsync())
        {
            db.Students.AddRange(
                new Student(userStudent1.Id, programCSBachelor.Id, admissionYear: 2022, currentCourse: 4,
                    userAdmin.Id, groupCode: "CS-22-1"),
                new Student(userStudent2.Id, programCSMaster.Id, admissionYear: 2024, currentCourse: 2,
                    userAdmin.Id, groupCode: "CS-M-24"),
                new Student(userStudent3.Id, programSEBachelor.Id, admissionYear: 2022, currentCourse: 4,
                    userAdmin.Id, groupCode: "SE-22-1")
            );
            await db.SaveChangesAsync();
        }

        var student1 = await db.Students.FirstAsync(s => s.UserId == userStudent1.Id);
        var student2 = await db.Students.FirstAsync(s => s.UserId == userStudent2.Id);
        var student3 = await db.Students.FirstAsync(s => s.UserId == userStudent3.Id);

        // =======================================================
        // 12. COMMON: Periods (all WorkflowStages for CS dept)
        // =======================================================
        if (!await db.Periods.AnyAsync())
        {
            var now = DateTime.UtcNow;
            db.Periods.AddRange(
                new Period(departmentCS.Id, academicYear.Id, WorkflowStage.DirectionSubmission,
                    now.AddMonths(-2), now.AddMonths(1), userAdmin.Id),
                new Period(departmentCS.Id, academicYear.Id, WorkflowStage.TopicCreation,
                    now.AddMonths(-1), now.AddMonths(2), userAdmin.Id),
                new Period(departmentCS.Id, academicYear.Id, WorkflowStage.TopicSelection,
                    now.AddDays(-15), now.AddMonths(3), userAdmin.Id),
                new Period(departmentCS.Id, academicYear.Id, WorkflowStage.PreDefense1,
                    now.AddMonths(3), now.AddMonths(4), userAdmin.Id),
                new Period(departmentCS.Id, academicYear.Id, WorkflowStage.PreDefense2,
                    now.AddMonths(4), now.AddMonths(5), userAdmin.Id),
                new Period(departmentCS.Id, academicYear.Id, WorkflowStage.PreDefense3,
                    now.AddMonths(5), now.AddMonths(6), userAdmin.Id),
                new Period(departmentCS.Id, academicYear.Id, WorkflowStage.FinalDefense,
                    now.AddMonths(6), now.AddMonths(7), userAdmin.Id)
            );
            await db.SaveChangesAsync();
        }

        // =======================================================
        // 13. WF: Work Types
        // =======================================================
        if (!await db.WorkTypes.AnyAsync())
        {
            db.WorkTypes.AddRange(
                WorkType.CourseWork(userAdmin.Id),
                WorkType.DiplomaWork(bachelor.Id, userAdmin.Id),
                WorkType.MasterThesis(master.Id, userAdmin.Id)
            );
            await db.SaveChangesAsync();
        }

        var workTypeDiploma = await db.WorkTypes.FirstAsync(w => w.Name == "DiplomaWork");
        var workTypeMaster = await db.WorkTypes.FirstAsync(w => w.Name == "MasterThesis");

        // =======================================================
        // 14. WF: States (Direction + StudentWork workflows)
        // =======================================================
        if (!await db.States.AnyAsync())
        {
            // Direction workflow states
            db.States.AddRange(
                new State(workTypeDiploma.Id, DirectionStates.Draft, userAdmin.Id, "Черновик направления"),
                new State(workTypeDiploma.Id, DirectionStates.Submitted, userAdmin.Id, "На рассмотрении"),
                new State(workTypeDiploma.Id, DirectionStates.Approved, userAdmin.Id, "Одобрено"),
                new State(workTypeDiploma.Id, DirectionStates.Rejected, userAdmin.Id, "Отклонено"),
                new State(workTypeDiploma.Id, DirectionStates.RequiresRevision, userAdmin.Id, "Требует доработки"),

                // StudentWork workflow states
                new State(workTypeDiploma.Id, WorkStates.Draft, userAdmin.Id, "Черновик работы"),
                new State(workTypeDiploma.Id, WorkStates.OnReview, userAdmin.Id, "На проверке у руководителя"),
                new State(workTypeDiploma.Id, WorkStates.NormControl, userAdmin.Id, "Нормоконтроль"),
                new State(workTypeDiploma.Id, WorkStates.SoftwareCheck, userAdmin.Id, "Проверка ПО"),
                new State(workTypeDiploma.Id, WorkStates.AntiPlagiarism, userAdmin.Id, "Антиплагиат"),
                new State(workTypeDiploma.Id, WorkStates.PreDefense1, userAdmin.Id, "Предзащита 1"),
                new State(workTypeDiploma.Id, WorkStates.PreDefense2, userAdmin.Id, "Предзащита 2"),
                new State(workTypeDiploma.Id, WorkStates.PreDefense3, userAdmin.Id, "Предзащита 3"),
                new State(workTypeDiploma.Id, WorkStates.ReadyForDefense, userAdmin.Id, "Готов к защите"),
                new State(workTypeDiploma.Id, WorkStates.Defended, userAdmin.Id, "Защищён", isFinal: true),
                new State(workTypeDiploma.Id, WorkStates.Cancelled, userAdmin.Id, "Отменён", isFinal: true)
            );
            await db.SaveChangesAsync();
        }

        var stateDirDraft = await db.States.FirstAsync(s => s.SystemName == DirectionStates.Draft);
        var stateDirSubmitted = await db.States.FirstAsync(s => s.SystemName == DirectionStates.Submitted);
        var stateDirApproved = await db.States.FirstAsync(s => s.SystemName == DirectionStates.Approved);
        var stateDirRejected = await db.States.FirstAsync(s => s.SystemName == DirectionStates.Rejected);
        var stateDirRevision = await db.States.FirstAsync(s => s.SystemName == DirectionStates.RequiresRevision);

        var stateWorkDraft = await db.States.FirstAsync(s => s.SystemName == WorkStates.Draft);
        var stateWorkOnReview = await db.States.FirstAsync(s => s.SystemName == WorkStates.OnReview);
        var stateWorkNormControl = await db.States.FirstAsync(s => s.SystemName == WorkStates.NormControl);
        var stateWorkSoftwareCheck = await db.States.FirstAsync(s => s.SystemName == WorkStates.SoftwareCheck);
        var stateWorkAntiPlagiarism = await db.States.FirstAsync(s => s.SystemName == WorkStates.AntiPlagiarism);
        var stateWorkPreDefense1 = await db.States.FirstAsync(s => s.SystemName == WorkStates.PreDefense1);
        var stateWorkReadyForDefense = await db.States.FirstAsync(s => s.SystemName == WorkStates.ReadyForDefense);
        var stateWorkDefended = await db.States.FirstAsync(s => s.SystemName == WorkStates.Defended);

        // =======================================================
        // 15. WF: Transitions (Direction workflow)
        // =======================================================
        if (!await db.Transitions.AnyAsync())
        {
            db.Transitions.AddRange(
                // Direction workflow: Draft → Submitted (Supervisor)
                Transition.Manual(stateDirDraft.Id, stateDirSubmitted.Id, roleSupervisor.Id, userAdmin.Id),
                // Submitted → Approved (HeadOfDepartment)
                Transition.Manual(stateDirSubmitted.Id, stateDirApproved.Id, roleHeadDept.Id, userAdmin.Id),
                // Submitted → Rejected (HeadOfDepartment)
                Transition.Manual(stateDirSubmitted.Id, stateDirRejected.Id, roleHeadDept.Id, userAdmin.Id),
                // Submitted → RequiresRevision (HeadOfDepartment)
                Transition.Manual(stateDirSubmitted.Id, stateDirRevision.Id, roleHeadDept.Id, userAdmin.Id),
                // RequiresRevision → Submitted (Supervisor, resubmit)
                Transition.Manual(stateDirRevision.Id, stateDirSubmitted.Id, roleSupervisor.Id, userAdmin.Id),

                // StudentWork workflow: Draft → OnReview (Student)
                Transition.Manual(stateWorkDraft.Id, stateWorkOnReview.Id, roleStudent.Id, userAdmin.Id),
                // OnReview → NormControl (Supervisor)
                Transition.Manual(stateWorkOnReview.Id, stateWorkNormControl.Id, roleSupervisor.Id, userAdmin.Id),
                // NormControl → SoftwareCheck (Expert)
                Transition.Manual(stateWorkNormControl.Id, stateWorkSoftwareCheck.Id, roleExpert.Id, userAdmin.Id),
                // SoftwareCheck → AntiPlagiarism (Expert)
                Transition.Manual(stateWorkSoftwareCheck.Id, stateWorkAntiPlagiarism.Id, roleExpert.Id, userAdmin.Id),
                // AntiPlagiarism → PreDefense1 (automatic)
                Transition.Automatic(stateWorkAntiPlagiarism.Id, stateWorkPreDefense1.Id, userAdmin.Id),
                // PreDefense1 → ReadyForDefense (HeadOfDepartment)
                Transition.Manual(stateWorkPreDefense1.Id, stateWorkReadyForDefense.Id, roleHeadDept.Id, userAdmin.Id),
                // ReadyForDefense → Defended (Commission)
                Transition.Manual(stateWorkReadyForDefense.Id, stateWorkDefended.Id, roleCommission.Id, userAdmin.Id)
            );
            await db.SaveChangesAsync();
        }

        // =======================================================
        // 16. THESIS: Directions (1 Draft, 1 Approved)
        // =======================================================
        if (!await db.Directions.AnyAsync())
        {
            // Direction in Draft state
            var dirDraft = new Direction(
                departmentCS.Id, staffSupervisor1.Id, academicYear.Id, workTypeDiploma.Id,
                "Искусственный интеллект в обработке данных",
                stateDirDraft.Id,
                titleKz: "Мәліметтерді өңдеуде жасанды интеллект",
                titleEn: "Artificial Intelligence in Data Processing",
                description: "Исследование применения методов ИИ для анализа и обработки больших данных");

            // Direction that went through workflow → Approved
            var dirApproved = new Direction(
                departmentCS.Id, staffSupervisor1.Id, academicYear.Id, workTypeDiploma.Id,
                "Разработка web-приложений с использованием микросервисов",
                stateDirApproved.Id,
                titleKz: "Микросервистерді қолдана отырып web-қосымшаларды әзірлеу",
                titleEn: "Web Application Development Using Microservices",
                description: "Проектирование и разработка масштабируемых web-систем на основе микросервисной архитектуры");

            db.Directions.AddRange(dirDraft, dirApproved);
            await db.SaveChangesAsync();
        }

        var directionApproved = await db.Directions.FirstAsync(d => d.CurrentStateId == stateDirApproved.Id);

        // =======================================================
        // 17. THESIS: Topics (1 individual, 1 team)
        // =======================================================
        if (!await db.Topics.AnyAsync())
        {
            var topicIndividual = new Topic(
                departmentCS.Id, staffSupervisor1.Id, academicYear.Id, workTypeDiploma.Id,
                "Разработка системы рекомендаций на основе collaborative filtering",
                directionApproved.Id,
                titleKz: "Collaborative filtering негізінде ұсыныс жүйесін әзірлеу",
                titleEn: "Recommendation System Development Based on Collaborative Filtering",
                description: "Реализация и исследование алгоритмов коллаборативной фильтрации",
                maxParticipants: 1);
            topicIndividual.Approve(); // Approve for student selection

            var topicTeam = new Topic(
                departmentCS.Id, staffSupervisor1.Id, academicYear.Id, workTypeDiploma.Id,
                "Разработка платформы для управления дипломными работами",
                directionApproved.Id,
                titleKz: "Дипломдық жұмыстарды басқару платформасын әзірлеу",
                titleEn: "Academic Work Management Platform Development",
                description: "Командный проект по разработке системы AWM",
                maxParticipants: 3);
            topicTeam.Approve();

            db.Topics.AddRange(topicIndividual, topicTeam);
            await db.SaveChangesAsync();
        }

        var topicForApplication = await db.Topics.FirstAsync(t => t.MaxParticipants == 1);
        var topicForTeam = await db.Topics.FirstAsync(t => t.MaxParticipants == 3);

        // =======================================================
        // 18. THESIS: Topic Applications
        // =======================================================
        if (!await db.TopicApplications.AnyAsync())
        {
            db.TopicApplications.AddRange(
                new TopicApplication(topicForApplication.Id, student1.Id,
                    "Хочу заниматься исследованием алгоритмов рекомендательных систем"),
                new TopicApplication(topicForTeam.Id, student1.Id,
                    "Готов работать в команде над платформой AWM"),
                new TopicApplication(topicForTeam.Id, student2.Id,
                    "Интересует backend-разработка для платформы")
            );
            await db.SaveChangesAsync();
        }

        // =======================================================
        // 19. THESIS: Student Works (1 work in Draft with participant)
        // =======================================================
        if (!await db.StudentWorks.AnyAsync())
        {
            var work = new StudentWork(
                academicYear.Id, departmentCS.Id, stateWorkDraft.Id, userStudent1.Id,
                topicForApplication.Id);

            work.AddParticipant(student1.Id, ParticipantRole.Leader);

            db.StudentWorks.Add(work);
            await db.SaveChangesAsync();
        }
    }
}
