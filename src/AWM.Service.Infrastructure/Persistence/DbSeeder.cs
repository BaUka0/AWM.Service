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
    private static class Constants
    {
        public const string TestPassword = "Test123!";

        public static class Org
        {
            public const string UniversityName = "Тестовый Университет";
            public const string UniversityCode = "TESTUNI";

            public const string InstituteITName = "Институт информационных технологий";
            public const string InstituteITCode = "IIT";
            public const string InstituteEngName = "Институт инженерии";
            public const string InstituteEngCode = "IE";

            public const string DeptCSName = "Компьютерные науки";
            public const string DeptCSCode = "CS";
            public const string DeptSEName = "Программная инженерия";
            public const string DeptSECode = "SE";
            public const string DeptMEName = "Машиностроение";
            public const string DeptMECode = "ME";
        }

        public static class Auth
        {
            public const string AdminLogin = "admin";
            public const string ViceRectorLogin = "vicerector";
            public const string HeadCSLogin = "head_cs";
            public const string Supervisor1Login = "supervisor1";
            public const string Supervisor2Login = "supervisor2";
            public const string Secretary1Login = "secretary1";
            public const string Expert1Login = "expert1";
            public const string Commission1Login = "commission1";
            public const string Student1Login = "student1";
            public const string Student2Login = "student2";
            public const string Student3Login = "student3";

            public const string AdminEmail = "admin@test.edu";
            public const string ViceRectorEmail = "vicerector@test.edu";
            public const string HeadCSEmail = "head_cs@test.edu";
            public const string Supervisor1Email = "supervisor1@test.edu";
            public const string Supervisor2Email = "supervisor2@test.edu";
            public const string Secretary1Email = "secretary1@test.edu";
            public const string Expert1Email = "expert1@test.edu";
            public const string Commission1Email = "commission1@test.edu";
            public const string Student1Email = "student1@test.edu";
            public const string Student2Email = "student2@test.edu";
            public const string Student3Email = "student3@test.edu";
        }

        public static class Common
        {
            public const string AcademicYearName = "2025-2026";
        }

        public static class Edu
        {
            public const string DegreeBachelor = "Bachelor";
            public const string DegreeMaster = "Master";
            public const string DegreePhD = "PhD";

            public const string ProgramCSBachelorName = "Информатика (бакалавр)";
            public const string ProgramCSBachelorCode = "6B06101";
            public const string ProgramCSMasterName = "Информатика (магистр)";
            public const string ProgramCSMasterCode = "7M06101";
            public const string ProgramSEBachelorName = "Программная инженерия (бакалавр)";
            public const string ProgramSEBachelorCode = "6B06102";

            public const string StaffPositionDocent = "Доцент";
            public const string StaffPositionProfessor = "Профессор";
            public const string StaffPositionSeniorLecturer = "Старший преподаватель";

            public const string StaffDegreePhD = "PhD";
            public const string StaffDegreeDrSci = "DrSci";

            public const string GroupCS221 = "CS-22-1";
            public const string GroupCSM24 = "CS-M-24";
            public const string GroupSE221 = "SE-22-1";
        }

        public static class Wf
        {
            public const string WorkTypeCourseWork = "CourseWork";
            public const string WorkTypeDiplomaWork = "DiplomaWork";
            public const string WorkTypeMasterThesis = "MasterThesis";

            public static class DirectionStates
            {
                public const string Draft = "Черновик направления";
                public const string Submitted = "На рассмотрении";
                public const string Approved = "Одобрено";
                public const string Rejected = "Отклонено";
                public const string RequiresRevision = "Требует доработки";
            }

            public static class WorkStates
            {
                public const string Draft = "Черновик работы";
                public const string OnReview = "На проверке у руководителя";
                public const string NormControl = "Нормоконтроль";
                public const string SoftwareCheck = "Проверка ПО";
                public const string AntiPlagiarism = "Антиплагиат";
                public const string PreDefense1 = "Предзащита 1";
                public const string PreDefense2 = "Предзащита 2";
                public const string PreDefense3 = "Предзащита 3";
                public const string ReadyForDefense = "Готов к защите";
                public const string Defended = "Защищён";
                public const string Cancelled = "Отменён";
            }
        }

        public static class Thesis
        {
            public const string Dir1TitleRu = "Искусственный интеллект в обработке данных";
            public const string Dir1TitleKz = "Мәліметтерді өңдеуде жасанды интеллект";
            public const string Dir1TitleEn = "Artificial Intelligence in Data Processing";
            public const string Dir1Desc = "Исследование применения методов ИИ для анализа и обработки больших данных";

            public const string Dir2TitleRu = "Разработка web-приложений с использованием микросервисов";
            public const string Dir2TitleKz = "Микросервистерді қолдана отырып web-қосымшаларды әзірлеу";
            public const string Dir2TitleEn = "Web Application Development Using Microservices";
            public const string Dir2Desc = "Проектирование и разработка масштабируемых web-систем на основе микросервисной архитектуры";

            public const string Topic1TitleRu = "Разработка системы рекомендаций на основе collaborative filtering";
            public const string Topic1TitleKz = "Collaborative filtering негізінде ұсыныс жүйесін әзірлеу";
            public const string Topic1TitleEn = "Recommendation System Development Based on Collaborative Filtering";
            public const string Topic1Desc = "Реализация и исследование алгоритмов коллаборативной фильтрации";

            public const string Topic2TitleRu = "Разработка платформы для управления дипломными работами";
            public const string Topic2TitleKz = "Дипломдық жұмыстарды басқару платформасын әзірлеу";
            public const string Topic2TitleEn = "Academic Work Management Platform Development";
            public const string Topic2Desc = "Командный проект по разработке системы AWM";

            public const string App1Message = "Хочу заниматься исследованием алгоритмов рекомендательных систем";
            public const string App2Message = "Готов работать в команде над платформой AWM";
            public const string App3Message = "Интересует backend-разработка для платформы";
        }
    }

    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher passwordHasher)
    {
        // =======================================================
        // 1. ORG: University
        // =======================================================
        if (!await db.Universities.AnyAsync())
        {
            db.Universities.Add(new University(Constants.Org.UniversityName, Constants.Org.UniversityCode));
            await db.SaveChangesAsync();
        }

        var university = await db.Universities.FirstAsync(u => u.Code == Constants.Org.UniversityCode);

        // =======================================================
        // 2. ORG: Institutes
        // =======================================================
        if (!await db.Institutes.AnyAsync())
        {
            var itInstitute = university.AddInstitute(Constants.Org.InstituteITName, 0);
            itInstitute.UpdateCode(Constants.Org.InstituteITCode, 0);

            var engInstitute = university.AddInstitute(Constants.Org.InstituteEngName, 0);
            engInstitute.UpdateCode(Constants.Org.InstituteEngCode, 0);

            db.Institutes.AddRange(itInstitute, engInstitute);
            await db.SaveChangesAsync();
        }

        var instituteIT = await db.Institutes.FirstAsync(i => i.Code == Constants.Org.InstituteITCode);
        var instituteEng = await db.Institutes.FirstAsync(i => i.Code == Constants.Org.InstituteEngCode);

        // =======================================================
        // 3. ORG: Departments
        // =======================================================
        if (!await db.Departments.AnyAsync())
        {
            var deptCS = instituteIT.AddDepartment(Constants.Org.DeptCSName, 0, Constants.Org.DeptCSCode);
            var deptSE = instituteIT.AddDepartment(Constants.Org.DeptSEName, 0, Constants.Org.DeptSECode);
            var deptME = instituteEng.AddDepartment(Constants.Org.DeptMEName, 0, Constants.Org.DeptMECode);

            // Explicitly add to context to ensure they are tracked if not already
            db.Departments.AddRange(deptCS, deptSE, deptME);
            await db.SaveChangesAsync();
        }

        var departmentCS = await db.Departments.FirstAsync(d => d.Code == Constants.Org.DeptCSCode);
        var departmentSE = await db.Departments.FirstAsync(d => d.Code == Constants.Org.DeptSECode);
        var departmentME = await db.Departments.FirstAsync(d => d.Code == Constants.Org.DeptMECode);

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
        var hashedPassword = passwordHasher.HashPassword(Constants.TestPassword);

        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new User(university.Id, Constants.Auth.AdminLogin, Constants.Auth.AdminEmail, hashedPassword),
                new User(university.Id, Constants.Auth.ViceRectorLogin, Constants.Auth.ViceRectorEmail, hashedPassword),
                new User(university.Id, Constants.Auth.HeadCSLogin, Constants.Auth.HeadCSEmail, hashedPassword),
                new User(university.Id, Constants.Auth.Supervisor1Login, Constants.Auth.Supervisor1Email, hashedPassword),
                new User(university.Id, Constants.Auth.Supervisor2Login, Constants.Auth.Supervisor2Email, hashedPassword),
                new User(university.Id, Constants.Auth.Secretary1Login, Constants.Auth.Secretary1Email, hashedPassword),
                new User(university.Id, Constants.Auth.Expert1Login, Constants.Auth.Expert1Email, hashedPassword),
                new User(university.Id, Constants.Auth.Commission1Login, Constants.Auth.Commission1Email, hashedPassword),
                new User(university.Id, Constants.Auth.Student1Login, Constants.Auth.Student1Email, hashedPassword),
                new User(university.Id, Constants.Auth.Student2Login, Constants.Auth.Student2Email, hashedPassword),
                new User(university.Id, Constants.Auth.Student3Login, Constants.Auth.Student3Email, hashedPassword)
            );
            await db.SaveChangesAsync();
        }

        var userAdmin = await db.Users.FirstAsync(u => u.Login == Constants.Auth.AdminLogin);
        var userViceRector = await db.Users.FirstAsync(u => u.Login == Constants.Auth.ViceRectorLogin);
        var userHeadCS = await db.Users.FirstAsync(u => u.Login == Constants.Auth.HeadCSLogin);
        var userSupervisor1 = await db.Users.FirstAsync(u => u.Login == Constants.Auth.Supervisor1Login);
        var userSupervisor2 = await db.Users.FirstAsync(u => u.Login == Constants.Auth.Supervisor2Login);
        var userSecretary1 = await db.Users.FirstAsync(u => u.Login == Constants.Auth.Secretary1Login);
        var userExpert1 = await db.Users.FirstAsync(u => u.Login == Constants.Auth.Expert1Login);
        var userCommission1 = await db.Users.FirstAsync(u => u.Login == Constants.Auth.Commission1Login);
        var userStudent1 = await db.Users.FirstAsync(u => u.Login == Constants.Auth.Student1Login);
        var userStudent2 = await db.Users.FirstAsync(u => u.Login == Constants.Auth.Student2Login);
        var userStudent3 = await db.Users.FirstAsync(u => u.Login == Constants.Auth.Student3Login);

        // =======================================================
        // 6. COMMON: Academic Year (current, 2025-2026)
        // =======================================================
        if (!await db.AcademicYears.AnyAsync())
        {
            var year = new AcademicYear(
                university.Id,
                Constants.Common.AcademicYearName,
                new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                userAdmin.Id);
            year.SetAsCurrent(userAdmin.Id);
            db.AcademicYears.Add(year);
            await db.SaveChangesAsync();
        }

        var academicYear = await db.AcademicYears.FirstAsync(y => y.Name == Constants.Common.AcademicYearName);

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

        var bachelor = await db.DegreeLevels.FirstAsync(d => d.Name == Constants.Edu.DegreeBachelor);
        var master = await db.DegreeLevels.FirstAsync(d => d.Name == Constants.Edu.DegreeMaster);
        var phd = await db.DegreeLevels.FirstAsync(d => d.Name == Constants.Edu.DegreePhD);

        // =======================================================
        // 9. EDU: Academic Programs
        // =======================================================
        if (!await db.AcademicPrograms.AnyAsync())
        {
            db.AcademicPrograms.AddRange(
                new AcademicProgram(departmentCS.Id, bachelor.Id, Constants.Edu.ProgramCSBachelorName, userAdmin.Id, Constants.Edu.ProgramCSBachelorCode),
                new AcademicProgram(departmentCS.Id, master.Id, Constants.Edu.ProgramCSMasterName, userAdmin.Id, Constants.Edu.ProgramCSMasterCode),
                new AcademicProgram(departmentSE.Id, bachelor.Id, Constants.Edu.ProgramSEBachelorName, userAdmin.Id, Constants.Edu.ProgramSEBachelorCode)
            );
            await db.SaveChangesAsync();
        }

        var programCSBachelor = await db.AcademicPrograms.FirstAsync(p => p.Code == Constants.Edu.ProgramCSBachelorCode);
        var programCSMaster = await db.AcademicPrograms.FirstAsync(p => p.Code == Constants.Edu.ProgramCSMasterCode);
        var programSEBachelor = await db.AcademicPrograms.FirstAsync(p => p.Code == Constants.Edu.ProgramSEBachelorCode);

        // =======================================================
        // 10. EDU: Staff
        // =======================================================
        if (!await db.Staff.AnyAsync())
        {
            db.Staff.AddRange(
                new Staff(userSupervisor1.Id, departmentCS.Id, userAdmin.Id, isSupervisor: true,
                    position: Constants.Edu.StaffPositionDocent, academicDegree: Constants.Edu.StaffDegreePhD, maxStudentsLoad: 8),
                new Staff(userSupervisor2.Id, departmentSE.Id, userAdmin.Id, isSupervisor: true,
                    position: Constants.Edu.StaffPositionProfessor, academicDegree: Constants.Edu.StaffDegreeDrSci, maxStudentsLoad: 5),
                new Staff(userExpert1.Id, departmentCS.Id, userAdmin.Id, isSupervisor: false,
                    position: Constants.Edu.StaffPositionSeniorLecturer, academicDegree: Constants.Edu.StaffDegreePhD, maxStudentsLoad: 1)
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
                    userAdmin.Id, groupCode: Constants.Edu.GroupCS221),
                new Student(userStudent2.Id, programCSMaster.Id, admissionYear: 2024, currentCourse: 2,
                    userAdmin.Id, groupCode: Constants.Edu.GroupCSM24),
                new Student(userStudent3.Id, programSEBachelor.Id, admissionYear: 2022, currentCourse: 4,
                    userAdmin.Id, groupCode: Constants.Edu.GroupSE221)
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

        var workTypeDiploma = await db.WorkTypes.FirstAsync(w => w.Name == Constants.Wf.WorkTypeDiplomaWork);
        var workTypeMaster = await db.WorkTypes.FirstAsync(w => w.Name == Constants.Wf.WorkTypeMasterThesis);

        // =======================================================
        // 14. WF: States (Direction + StudentWork workflows)
        // =======================================================
        if (!await db.States.AnyAsync())
        {
            // Direction workflow states
            db.States.AddRange(
                new State(workTypeDiploma.Id, DirectionStates.Draft, userAdmin.Id, Constants.Wf.DirectionStates.Draft),
                new State(workTypeDiploma.Id, DirectionStates.Submitted, userAdmin.Id, Constants.Wf.DirectionStates.Submitted),
                new State(workTypeDiploma.Id, DirectionStates.Approved, userAdmin.Id, Constants.Wf.DirectionStates.Approved),
                new State(workTypeDiploma.Id, DirectionStates.Rejected, userAdmin.Id, Constants.Wf.DirectionStates.Rejected),
                new State(workTypeDiploma.Id, DirectionStates.RequiresRevision, userAdmin.Id, Constants.Wf.DirectionStates.RequiresRevision),

                // StudentWork workflow states
                new State(workTypeDiploma.Id, WorkStates.Draft, userAdmin.Id, Constants.Wf.WorkStates.Draft),
                new State(workTypeDiploma.Id, WorkStates.OnReview, userAdmin.Id, Constants.Wf.WorkStates.OnReview),
                new State(workTypeDiploma.Id, WorkStates.NormControl, userAdmin.Id, Constants.Wf.WorkStates.NormControl),
                new State(workTypeDiploma.Id, WorkStates.SoftwareCheck, userAdmin.Id, Constants.Wf.WorkStates.SoftwareCheck),
                new State(workTypeDiploma.Id, WorkStates.AntiPlagiarism, userAdmin.Id, Constants.Wf.WorkStates.AntiPlagiarism),
                new State(workTypeDiploma.Id, WorkStates.PreDefense1, userAdmin.Id, Constants.Wf.WorkStates.PreDefense1),
                new State(workTypeDiploma.Id, WorkStates.PreDefense2, userAdmin.Id, Constants.Wf.WorkStates.PreDefense2),
                new State(workTypeDiploma.Id, WorkStates.PreDefense3, userAdmin.Id, Constants.Wf.WorkStates.PreDefense3),
                new State(workTypeDiploma.Id, WorkStates.ReadyForDefense, userAdmin.Id, Constants.Wf.WorkStates.ReadyForDefense),
                new State(workTypeDiploma.Id, WorkStates.Defended, userAdmin.Id, Constants.Wf.WorkStates.Defended, isFinal: true),
                new State(workTypeDiploma.Id, WorkStates.Cancelled, userAdmin.Id, Constants.Wf.WorkStates.Cancelled, isFinal: true)
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
                Constants.Thesis.Dir1TitleRu,
                stateDirDraft.Id,
                titleKz: Constants.Thesis.Dir1TitleKz,
                titleEn: Constants.Thesis.Dir1TitleEn,
                description: Constants.Thesis.Dir1Desc);

            // Direction that went through workflow → Approved
            var dirApproved = new Direction(
                departmentCS.Id, staffSupervisor1.Id, academicYear.Id, workTypeDiploma.Id,
                Constants.Thesis.Dir2TitleRu,
                stateDirApproved.Id,
                titleKz: Constants.Thesis.Dir2TitleKz,
                titleEn: Constants.Thesis.Dir2TitleEn,
                description: Constants.Thesis.Dir2Desc);

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
                Constants.Thesis.Topic1TitleRu,
                directionApproved.Id,
                titleKz: Constants.Thesis.Topic1TitleKz,
                titleEn: Constants.Thesis.Topic1TitleEn,
                description: Constants.Thesis.Topic1Desc,
                maxParticipants: 1);
            topicIndividual.Approve(); // Approve for student selection

            var topicTeam = new Topic(
                departmentCS.Id, staffSupervisor1.Id, academicYear.Id, workTypeDiploma.Id,
                Constants.Thesis.Topic2TitleRu,
                directionApproved.Id,
                titleKz: Constants.Thesis.Topic2TitleKz,
                titleEn: Constants.Thesis.Topic2TitleEn,
                description: Constants.Thesis.Topic2Desc,
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
                    Constants.Thesis.App1Message),
                new TopicApplication(topicForTeam.Id, student1.Id,
                    Constants.Thesis.App2Message),
                new TopicApplication(topicForTeam.Id, student2.Id,
                    Constants.Thesis.App3Message)
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
