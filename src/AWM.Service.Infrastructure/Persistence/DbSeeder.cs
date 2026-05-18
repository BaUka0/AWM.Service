namespace AWM.Service.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Defense.Entities;
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
        // 1. ORG: Institutes
        // =======================================================
        if (!await db.Institutes.AnyAsync())
        {
            db.Institutes.AddRange(
                new[]
                {
                    Institute.Create("Институт информационных технологий", 0),
                    Institute.Create("Институт инженерии", 0)
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
        // 4. AUTH: Legacy Roles (for Workflow compatibility)
        // =======================================================
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(
                Role.Create("Admin", "Администратор"),
                Role.Create("ViceRector", "Проректор по УР"),
                Role.Create("HeadOfDepartment", "Заведующий кафедрой"),
                Role.Create("Supervisor", "Научный руководитель"),
                Role.Create("Secretary", "Секретарь"),
                Role.Create("Expert", "Эксперт"),
                Role.Create("Student", "Студент"),
                Role.Create("CommissionMember", "Член комиссии")
            );
            await db.SaveChangesAsync();
        }

        var roleAdmin = await db.Roles.FirstAsync(r => r.SystemName == "Admin");
        var roleViceRector = await db.Roles.FirstAsync(r => r.SystemName == "ViceRector");
        var roleHeadDept = await db.Roles.FirstAsync(r => r.SystemName == "HeadOfDepartment");
        var roleSupervisor = await db.Roles.FirstAsync(r => r.SystemName == "Supervisor");
        var roleSecretary = await db.Roles.FirstAsync(r => r.SystemName == "Secretary");
        var roleExpert = await db.Roles.FirstAsync(r => r.SystemName == "Expert");
        var roleStudent = await db.Roles.FirstAsync(r => r.SystemName == "Student");
        var roleCommission = await db.Roles.FirstAsync(r => r.SystemName == "CommissionMember");

        // =======================================================
        // 4b. RBAC+ Seed: RoleAccess, RoleOperation, RoleActionType, RoleOperationAction
        // =======================================================
        if (!await db.RoleAccesses.AnyAsync())
        {
            db.RoleAccesses.AddRange(
                new RoleAccess("ADMIN", "Администратор", "Администратор", "Administrator", 0),
                new RoleAccess("VICERECTOR", "Проректор по УР", "Проректор по УР", "Vice-Rector", 0),
                new RoleAccess("HEADDEPARTMENT", "Заведующий кафедрой", "Кафедра меңгерушісі", "Head of Department", 0),
                new RoleAccess("SUPERVISOR", "Научный руководитель", "Ғылыми жетекші", "Supervisor", 0),
                new RoleAccess("SECRETARY", "Секретарь", "Хатшы", "Secretary", 0),
                new RoleAccess("EXPERT", "Эксперт", "Сарапшы", "Expert", 0),
                new RoleAccess("STUDENT", "Студент", "Студент", "Student", 0),
                new RoleAccess("COMMISSIONMEMBER", "Член комиссии", "Комиссия мүшесі", "Commission Member", 0),
                new RoleAccess("REVIEWER", "Рецензент", "Рецензент", "Reviewer", 0),
                new RoleAccess("CHAIRMAN", "Председатель комиссии", "Комиссия төрағасы", "Chairman", 0)
            );
            await db.SaveChangesAsync();
        }

        var raAdmin = await db.RoleAccesses.FirstAsync(r => r.Code == "ADMIN");
        var raViceRector = await db.RoleAccesses.FirstAsync(r => r.Code == "VICERECTOR");
        var raHeadDept = await db.RoleAccesses.FirstAsync(r => r.Code == "HEADDEPARTMENT");
        var raSupervisor = await db.RoleAccesses.FirstAsync(r => r.Code == "SUPERVISOR");
        var raSecretary = await db.RoleAccesses.FirstAsync(r => r.Code == "SECRETARY");
        var raExpert = await db.RoleAccesses.FirstAsync(r => r.Code == "EXPERT");
        var raStudent = await db.RoleAccesses.FirstAsync(r => r.Code == "STUDENT");
        var raCommission = await db.RoleAccesses.FirstAsync(r => r.Code == "COMMISSIONMEMBER");
        var raReviewer = await db.RoleAccesses.FirstAsync(r => r.Code == "REVIEWER");
        var raChairman = await db.RoleAccesses.FirstAsync(r => r.Code == "CHAIRMAN");

        if (!await db.RoleActionTypes.AnyAsync())
        {
            db.RoleActionTypes.AddRange(
                new RoleActionType("READ", "Просмотр", "Қарау", "Read"),
                new RoleActionType("CREATE", "Создание", "Жасау", "Create"),
                new RoleActionType("UPDATE", "Обновление", "Жаңарту", "Update"),
                new RoleActionType("DELETE", "Удаление", "Жою", "Delete")
            );
            await db.SaveChangesAsync();
        }

        var actRead = await db.RoleActionTypes.FirstAsync(a => a.Code == "READ");
        var actCreate = await db.RoleActionTypes.FirstAsync(a => a.Code == "CREATE");
        var actUpdate = await db.RoleActionTypes.FirstAsync(a => a.Code == "UPDATE");
        var actDelete = await db.RoleActionTypes.FirstAsync(a => a.Code == "DELETE");

        if (!await db.RoleOperations.AnyAsync())
        {
            db.RoleOperations.AddRange(
                new RoleOperation("Directions", "Направления", "Бағыттар", "Directions", 0, orderBy: 10),
                new RoleOperation("Directions_Approval", "Утверждение направлений", "Бағыттарды бекіту", "Direction Approval", 0, parentId: null, orderBy: 11),
                new RoleOperation("Topics", "Темы", "Тақырыптар", "Topics", 0, orderBy: 20),
                new RoleOperation("Topics_Approval", "Утверждение тем", "Тақырыптарды бекіту", "Topic Approval", 0, parentId: null, orderBy: 21),
                new RoleOperation("TopicApplications", "Заявки", "Өтініштер", "Applications", 0, orderBy: 30),
                new RoleOperation("StudentWorks", "Работы", "Жұмыстар", "Student Works", 0, orderBy: 40),
                new RoleOperation("Works_StateChange", "Смена статуса работы", "Жұмыс күйін өзгерту", "Work State Change", 0, orderBy: 41),
                new RoleOperation("QualityChecks", "Проверки качества", "Сапа тексеру", "Quality Checks", 0, orderBy: 50),
                new RoleOperation("Reviews", "Рецензии", "Пікірлер", "Reviews", 0, orderBy: 60),
                new RoleOperation("PreDefense", "Предзащита", "Алдын ала қорғау", "Pre-Defense", 0, orderBy: 70),
                new RoleOperation("PreDefense_Grading", "Оценка предзащиты", "Алдын ала қорғау бағалауы", "Pre-Defense Grading", 0, orderBy: 71),
                new RoleOperation("FinalDefense", "Защита", "Қорғау", "Final Defense", 0, orderBy: 80),
                new RoleOperation("Defense_Grading", "Оценка защиты", "Қорғау бағалауы", "Defense Grading", 0, orderBy: 81),
                new RoleOperation("Defense_Protocol", "Протоколы защиты", "Қорғау хаттамалары", "Defense Protocols", 0, orderBy: 82),
                new RoleOperation("Commissions", "Комиссии", "Комиссиялар", "Commissions", 0, orderBy: 90),
                new RoleOperation("Users", "Пользователи", "Пайдаланушылар", "Users", 0, orderBy: 100),
                new RoleOperation("Users_Roles", "Управление ролями", "Рөлдерді басқару", "Role Management", 0, orderBy: 101),
                new RoleOperation("Organization", "Организация", "Ұйым", "Organization", 0, orderBy: 110),
                new RoleOperation("Org_Departments", "Кафедры", "Кафедралар", "Departments", 0, orderBy: 111),
                new RoleOperation("Org_Institutes", "Институты", "Институттар", "Institutes", 0, orderBy: 112),
                new RoleOperation("Reports", "Отчеты", "Есептер", "Reports", 0, orderBy: 120)
            );
            await db.SaveChangesAsync();
        }

        var opDirections = await db.RoleOperations.FirstAsync(o => o.Name == "Directions");
        var opDirectionsApproval = await db.RoleOperations.FirstAsync(o => o.Name == "Directions_Approval");
        var opTopics = await db.RoleOperations.FirstAsync(o => o.Name == "Topics");
        var opTopicsApproval = await db.RoleOperations.FirstAsync(o => o.Name == "Topics_Approval");
        var opApplications = await db.RoleOperations.FirstAsync(o => o.Name == "TopicApplications");
        var opWorks = await db.RoleOperations.FirstAsync(o => o.Name == "StudentWorks");
        var opStateChange = await db.RoleOperations.FirstAsync(o => o.Name == "Works_StateChange");
        var opQuality = await db.RoleOperations.FirstAsync(o => o.Name == "QualityChecks");
        var opReviews = await db.RoleOperations.FirstAsync(o => o.Name == "Reviews");
        var opPreDefense = await db.RoleOperations.FirstAsync(o => o.Name == "PreDefense");
        var opPreDefenseGrade = await db.RoleOperations.FirstAsync(o => o.Name == "PreDefense_Grading");
        var opFinalDefense = await db.RoleOperations.FirstAsync(o => o.Name == "FinalDefense");
        var opDefenseGrade = await db.RoleOperations.FirstAsync(o => o.Name == "Defense_Grading");
        var opDefenseProtocol = await db.RoleOperations.FirstAsync(o => o.Name == "Defense_Protocol");
        var opCommissions = await db.RoleOperations.FirstAsync(o => o.Name == "Commissions");
        var opUsers = await db.RoleOperations.FirstAsync(o => o.Name == "Users");
        var opUsersRoles = await db.RoleOperations.FirstAsync(o => o.Name == "Users_Roles");
        var opOrg = await db.RoleOperations.FirstAsync(o => o.Name == "Organization");
        var opOrgDepts = await db.RoleOperations.FirstAsync(o => o.Name == "Org_Departments");
        var opOrgInsts = await db.RoleOperations.FirstAsync(o => o.Name == "Org_Institutes");
        var opReports = await db.RoleOperations.FirstAsync(o => o.Name == "Reports");

        if (!await db.RoleOperationActions.AnyAsync())
        {
            // Helper to add multiple actions
            void Grant(RoleAccess ra, RoleOperation op, params RoleActionType[] actions)
            {
                foreach (var a in actions)
                    db.RoleOperationActions.Add(new RoleOperationAction(ra.Id, op.Id, a.Id));
            }

            // ADMIN — full access
            Grant(raAdmin, opDirections, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opDirectionsApproval, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opTopics, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opTopicsApproval, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opApplications, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opWorks, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opStateChange, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opQuality, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opReviews, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opPreDefense, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opPreDefenseGrade, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opFinalDefense, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opDefenseGrade, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opDefenseProtocol, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opCommissions, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opUsers, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opUsersRoles, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opOrg, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opOrgDepts, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opOrgInsts, actRead, actCreate, actUpdate, actDelete);
            Grant(raAdmin, opReports, actRead, actCreate, actUpdate, actDelete);

            // STUDENT
            Grant(raStudent, opTopics, actRead);
            Grant(raStudent, opApplications, actRead, actCreate);
            Grant(raStudent, opWorks, actRead, actUpdate);
            Grant(raStudent, opReviews, actRead);

            // SUPERVISOR
            Grant(raSupervisor, opTopics, actRead, actCreate);
            Grant(raSupervisor, opWorks, actRead, actUpdate);
            Grant(raSupervisor, opReviews, actRead, actCreate);
            Grant(raSupervisor, opDirections, actRead);

            // HEAD OF DEPARTMENT
            Grant(raHeadDept, opDirections, actRead, actCreate, actUpdate);
            Grant(raHeadDept, opDirectionsApproval, actUpdate);
            Grant(raHeadDept, opTopics, actRead, actCreate, actUpdate);
            Grant(raHeadDept, opTopicsApproval, actUpdate);
            Grant(raHeadDept, opApplications, actRead, actUpdate, actDelete);
            Grant(raHeadDept, opWorks, actRead, actUpdate);
            Grant(raHeadDept, opStateChange, actUpdate);
            Grant(raHeadDept, opPreDefense, actRead);
            Grant(raHeadDept, opPreDefenseGrade, actUpdate);
            Grant(raHeadDept, opCommissions, actRead, actCreate, actUpdate, actDelete);
            Grant(raHeadDept, opFinalDefense, actRead);
            Grant(raHeadDept, opDefenseGrade, actUpdate);
            Grant(raHeadDept, opDefenseProtocol, actCreate);
            Grant(raHeadDept, opReports, actRead);

            // SECRETARY
            Grant(raSecretary, opDirections, actRead);
            Grant(raSecretary, opTopics, actRead);
            Grant(raSecretary, opWorks, actRead, actUpdate);
            Grant(raSecretary, opCommissions, actRead, actCreate, actUpdate, actDelete);
            Grant(raSecretary, opPreDefense, actRead);
            Grant(raSecretary, opFinalDefense, actRead);
            Grant(raSecretary, opDefenseProtocol, actCreate);
            Grant(raSecretary, opReports, actRead);

            // EXPERT
            Grant(raExpert, opQuality, actCreate);
            Grant(raExpert, opWorks, actRead);

            // COMMISSION MEMBER
            Grant(raCommission, opFinalDefense, actRead);
            Grant(raCommission, opDefenseGrade, actUpdate);
            Grant(raCommission, opCommissions, actRead);

            // REVIEWER
            Grant(raReviewer, opReviews, actRead, actCreate);

            // CHAIRMAN
            Grant(raChairman, opFinalDefense, actRead);
            Grant(raChairman, opDefenseGrade, actUpdate);
            Grant(raChairman, opCommissions, actRead, actCreate, actUpdate, actDelete);
            Grant(raChairman, opDefenseProtocol, actCreate);

            // VICE RECTOR
            Grant(raViceRector, opReports, actRead, actCreate);
            Grant(raViceRector, opOrg, actRead);

            await db.SaveChangesAsync();
        }

        // =======================================================
        // 5. AUTH: Users (8 test users, one per role)
        // =======================================================
        var hashedPassword = passwordHasher.HashPassword(TestPassword);

        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new User("admin", "admin@test.edu", hashedPassword),
                new User("vicerector", "vicerector@test.edu", hashedPassword),
                new User("head_cs", "head_cs@test.edu", hashedPassword),
                new User("supervisor1", "supervisor1@test.edu", hashedPassword),
                new User("supervisor2", "supervisor2@test.edu", hashedPassword),
                new User("secretary1", "secretary1@test.edu", hashedPassword),
                new User("expert1", "expert1@test.edu", hashedPassword),
                new User("commission1", "commission1@test.edu", hashedPassword),
                new User("student1", "student1@test.edu", hashedPassword),
                new User("student2", "student2@test.edu", hashedPassword),
                new User("student3", "student3@test.edu", hashedPassword)
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
        // 7. AUTH: User Access Assignments (RBAC+)
        // =======================================================
        if (!await db.UserAccesses.AnyAsync())
        {
            // Admin
            userAdmin.AssignRoleAccess(roleAdmin.Id, assignedBy: userAdmin.Id);

            // ViceRector
            userViceRector.AssignRoleAccess(roleViceRector.Id, assignedBy: userAdmin.Id);

            // HeadOfDepartment
            userHeadCS.AssignRoleAccess(roleHeadDept.Id, assignedBy: userAdmin.Id);

            // Supervisor1
            userSupervisor1.AssignRoleAccess(roleSupervisor.Id, assignedBy: userAdmin.Id);

            // Supervisor2
            userSupervisor2.AssignRoleAccess(roleSupervisor.Id, assignedBy: userAdmin.Id);

            // Secretary
            userSecretary1.AssignRoleAccess(roleSecretary.Id, assignedBy: userAdmin.Id);

            // Expert
            userExpert1.AssignRoleAccess(roleExpert.Id, assignedBy: userAdmin.Id);

            // CommissionMember
            userCommission1.AssignRoleAccess(roleCommission.Id, assignedBy: userAdmin.Id);

            // Students
            userStudent1.AssignRoleAccess(roleStudent.Id, assignedBy: userAdmin.Id);
            userStudent2.AssignRoleAccess(roleStudent.Id, assignedBy: userAdmin.Id);
            userStudent3.AssignRoleAccess(roleStudent.Id, assignedBy: userAdmin.Id);

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
        // 10b. THESIS + DEFENSE + EDU: Reference tables (replacing enums)
        // =======================================================
        if (!await db.ApplicationStatuses.AnyAsync())
        {
            db.ApplicationStatuses.AddRange(
                new ApplicationStatus(1, "Submitted"),
                new ApplicationStatus(2, "Accepted"),
                new ApplicationStatus(3, "Rejected")
            );
            await db.SaveChangesAsync();
        }

        if (!await db.ParticipantRoles.AnyAsync())
        {
            db.ParticipantRoles.AddRange(
                new ParticipantRole(1, "Leader"),
                new ParticipantRole(2, "Member")
            );
            await db.SaveChangesAsync();
        }

        if (!await db.AttachmentTypes.AnyAsync())
        {
            db.AttachmentTypes.AddRange(
                new AttachmentType(1, "Draft"),
                new AttachmentType(2, "Final"),
                new AttachmentType(3, "Presentation"),
                new AttachmentType(4, "Software"),
                new AttachmentType(5, "Demo"),
                new AttachmentType(6, "Handout")
            );
            await db.SaveChangesAsync();
        }

        if (!await db.CheckTypes.AnyAsync())
        {
            db.CheckTypes.AddRange(
                new CheckType(1, "NormControl"),
                new CheckType(2, "SoftwareCheck"),
                new CheckType(3, "AntiPlagiarism")
            );
            await db.SaveChangesAsync();
        }

        if (!await db.CommissionTypes.AnyAsync())
        {
            db.CommissionTypes.AddRange(
                new CommissionType(1, "PreDefense"),
                new CommissionType(2, "GAK")
            );
            await db.SaveChangesAsync();
        }

        if (!await db.CommissionRoles.AnyAsync())
        {
            db.CommissionRoles.AddRange(
                new CommissionRole(1, "Chairman"),
                new CommissionRole(2, "Secretary"),
                new CommissionRole(3, "Member")
            );
            await db.SaveChangesAsync();
        }

        if (!await db.AttendanceStatuses.AnyAsync())
        {
            db.AttendanceStatuses.AddRange(
                new AttendanceStatus(1, "Attended"),
                new AttendanceStatus(2, "Absent"),
                new AttendanceStatus(3, "Excused")
            );
            await db.SaveChangesAsync();
        }

        if (!await db.StudentStatuses.AnyAsync())
        {
            db.StudentStatuses.AddRange(
                new StudentStatus(1, "Active"),
                new StudentStatus(2, "Graduated"),
                new StudentStatus(3, "OnLeave"),
                new StudentStatus(4, "Expelled"),
                new StudentStatus(5, "Transferred")
            );
            await db.SaveChangesAsync();
        }

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
        // 6. COMMON: Semester Types
        // =======================================================
        if (!await db.SemesterTypes.AnyAsync())
        {
            db.SemesterTypes.AddRange(
                new SemesterType("Весна", 0),
                new SemesterType("Лето", 0),
                new SemesterType("Осень", 0),
                new SemesterType("Зима", 0)
            );
            await db.SaveChangesAsync();
        }

        // =======================================================
        // 7. COMMON: Workflow Stages
        // =======================================================
        if (!await db.WorkflowStages.AnyAsync())
        {
            db.WorkflowStages.AddRange(
                new WorkflowStage("DirectionSubmission", 1),
                new WorkflowStage("TopicCreation", 2),
                new WorkflowStage("TopicSelection", 3),
                new WorkflowStage("PreDefense1", 4),
                new WorkflowStage("PreDefense2", 5),
                new WorkflowStage("PreDefense3", 6),
                new WorkflowStage("FinalDefense", 7)
            );
            await db.SaveChangesAsync();
        }

        // =======================================================
        // 8. COMMON: Semesters
        // =======================================================
        if (!await db.Semesters.AnyAsync())
        {
            db.Semesters.AddRange(
                new Semester(3, "Осень 2025", new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc), 2025, userAdmin.Id),
                new Semester(1, "Весна 2026", new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 30, 23, 59, 59, DateTimeKind.Utc), 2025, userAdmin.Id)
            );
            await db.SaveChangesAsync();
        }

        var semester = await db.Semesters.FirstAsync();

        // =======================================================
        // 9. COMMON: Stages (all WorkflowStages for CS dept)
        // =======================================================
        if (!await db.Stages.AnyAsync())
        {
            var now = DateTime.UtcNow;
            db.Stages.AddRange(
                new Stage(departmentCS.Id, semester.Id, 1,
                    now.AddMonths(-2), now.AddMonths(1), userAdmin.Id),
                new Stage(departmentCS.Id, semester.Id, 2,
                    now.AddMonths(-1), now.AddMonths(2), userAdmin.Id),
                new Stage(departmentCS.Id, semester.Id, 3,
                    now.AddDays(-15), now.AddMonths(3), userAdmin.Id),
                new Stage(departmentCS.Id, semester.Id, 4,
                    now.AddMonths(3), now.AddMonths(4), userAdmin.Id),
                new Stage(departmentCS.Id, semester.Id, 5,
                    now.AddMonths(4), now.AddMonths(5), userAdmin.Id),
                new Stage(departmentCS.Id, semester.Id, 6,
                    now.AddMonths(5), now.AddMonths(6), userAdmin.Id),
                new Stage(departmentCS.Id, semester.Id, 7,
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

        var workTypeCourse = await db.WorkTypes.FirstAsync(w => w.Name == "CourseWork");
        var workTypeDiploma = await db.WorkTypes.FirstAsync(w => w.Name == "DiplomaWork");
        var workTypeMaster = await db.WorkTypes.FirstAsync(w => w.Name == "MasterThesis");

        // Helper function to create states and transitions for a specific work type
        async Task SeedWorkflowForWorkType(ApplicationDbContext context, int workTypeId)
        {
            // States
            var states = new[]
            {
                new State(workTypeId, DirectionStates.Draft, userAdmin.Id, "Черновик направления"),
                new State(workTypeId, DirectionStates.Submitted, userAdmin.Id, "На рассмотрении"),
                new State(workTypeId, DirectionStates.Approved, userAdmin.Id, "Одобрено"),
                new State(workTypeId, DirectionStates.Rejected, userAdmin.Id, "Отклонено"),
                new State(workTypeId, DirectionStates.RequiresRevision, userAdmin.Id, "Требует доработки"),

                new State(workTypeId, WorkStates.Draft, userAdmin.Id, "Черновик работы"),
                new State(workTypeId, WorkStates.OnReview, userAdmin.Id, "На проверке у руководителя"),
                new State(workTypeId, WorkStates.NormControl, userAdmin.Id, "Нормоконтроль"),
                new State(workTypeId, WorkStates.SoftwareCheck, userAdmin.Id, "Проверка ПО"),
                new State(workTypeId, WorkStates.AntiPlagiarism, userAdmin.Id, "Антиплагиат"),
                new State(workTypeId, WorkStates.PreDefense1, userAdmin.Id, "Предзащита 1"),
                new State(workTypeId, WorkStates.PreDefense2, userAdmin.Id, "Предзащита 2"),
                new State(workTypeId, WorkStates.PreDefense3, userAdmin.Id, "Предзащита 3"),
                new State(workTypeId, WorkStates.ReadyForDefense, userAdmin.Id, "Готов к защите"),
                new State(workTypeId, WorkStates.Defended, userAdmin.Id, "Защищён", isFinal: true),
                new State(workTypeId, WorkStates.Cancelled, userAdmin.Id, "Отменён", isFinal: true)
            };

            context.States.AddRange(states);
            await context.SaveChangesAsync();

            var stateDirDraft = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == DirectionStates.Draft);
            var stateDirSubmitted = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == DirectionStates.Submitted);
            var stateDirApproved = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == DirectionStates.Approved);
            var stateDirRejected = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == DirectionStates.Rejected);
            var stateDirRevision = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == DirectionStates.RequiresRevision);

            var stateWorkDraft = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == WorkStates.Draft);
            var stateWorkOnReview = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == WorkStates.OnReview);
            var stateWorkNormControl = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == WorkStates.NormControl);
            var stateWorkSoftwareCheck = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == WorkStates.SoftwareCheck);
            var stateWorkAntiPlagiarism = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == WorkStates.AntiPlagiarism);
            var stateWorkPreDefense1 = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == WorkStates.PreDefense1);
            var stateWorkReadyForDefense = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == WorkStates.ReadyForDefense);
            var stateWorkDefended = await context.States.FirstAsync(s => s.WorkTypeId == workTypeId && s.SystemName == WorkStates.Defended);

            context.Transitions.AddRange(
                Transition.Manual(stateDirDraft.Id, stateDirSubmitted.Id, roleSupervisor.Id, userAdmin.Id),
                Transition.Manual(stateDirSubmitted.Id, stateDirApproved.Id, roleHeadDept.Id, userAdmin.Id),
                Transition.Manual(stateDirSubmitted.Id, stateDirRejected.Id, roleHeadDept.Id, userAdmin.Id),
                Transition.Manual(stateDirSubmitted.Id, stateDirRevision.Id, roleHeadDept.Id, userAdmin.Id),
                Transition.Manual(stateDirRevision.Id, stateDirSubmitted.Id, roleSupervisor.Id, userAdmin.Id),

                Transition.Manual(stateWorkDraft.Id, stateWorkOnReview.Id, roleStudent.Id, userAdmin.Id),
                Transition.Manual(stateWorkOnReview.Id, stateWorkNormControl.Id, roleSupervisor.Id, userAdmin.Id),
                Transition.Manual(stateWorkNormControl.Id, stateWorkSoftwareCheck.Id, roleExpert.Id, userAdmin.Id),
                Transition.Manual(stateWorkSoftwareCheck.Id, stateWorkAntiPlagiarism.Id, roleExpert.Id, userAdmin.Id),
                Transition.Automatic(stateWorkAntiPlagiarism.Id, stateWorkPreDefense1.Id, userAdmin.Id),
                Transition.Manual(stateWorkPreDefense1.Id, stateWorkReadyForDefense.Id, roleHeadDept.Id, userAdmin.Id),
                Transition.Manual(stateWorkReadyForDefense.Id, stateWorkDefended.Id, roleCommission.Id, userAdmin.Id)
            );
            await context.SaveChangesAsync();
        }

        // =======================================================
        // 14. WF: States (Direction + StudentWork workflows)
        // =======================================================
        if (!await db.States.AnyAsync())
        {
            await SeedWorkflowForWorkType(db, workTypeCourse.Id);
            await SeedWorkflowForWorkType(db, workTypeDiploma.Id);
            await SeedWorkflowForWorkType(db, workTypeMaster.Id);
        }

        var stateDirDraft = await db.States.FirstAsync(s => s.WorkTypeId == workTypeDiploma.Id && s.SystemName == DirectionStates.Draft);
        var stateDirApproved = await db.States.FirstAsync(s => s.WorkTypeId == workTypeDiploma.Id && s.SystemName == DirectionStates.Approved);
        var stateWorkDraft = await db.States.FirstAsync(s => s.WorkTypeId == workTypeDiploma.Id && s.SystemName == WorkStates.Draft);

        // =======================================================
        // 16. THESIS: Directions (1 Draft, 1 Approved)
        // =======================================================
        if (!await db.Directions.AnyAsync())
        {
            // Direction in Draft state
            var dirDraft = new Direction(
                departmentCS.Id, staffSupervisor1.Id, semester.Id, workTypeDiploma.Id,
                "Искусственный интеллект в обработке данных",
                stateDirDraft.Id,
                titleKz: "Мәліметтерді өңдеуде жасанды интеллект",
                titleEn: "Artificial Intelligence in Data Processing",
                descriptionRu: "Исследование применения методов ИИ для анализа и обработки больших данных");

            // Direction that went through workflow → Approved
            var dirApproved = new Direction(
                departmentCS.Id, staffSupervisor1.Id, semester.Id, workTypeDiploma.Id,
                "Разработка web-приложений с использованием микросервисов",
                stateDirApproved.Id,
                titleKz: "Микросервистерді қолдана отырып web-қосымшаларды әзірлеу",
                titleEn: "Web Application Development Using Microservices",
                descriptionRu: "Проектирование и разработка масштабируемых web-систем на основе микросервисной архитектуры");

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
                departmentCS.Id, staffSupervisor1.Id, semester.Id, workTypeDiploma.Id,
                "Разработка системы рекомендаций на основе collaborative filtering",
                directionApproved.Id,
                titleKz: "Collaborative filtering негізінде ұсыныс жүйесін әзірлеу",
                titleEn: "Recommendation System Development Based on Collaborative Filtering",
                descriptionRu: "Реализация и исследование алгоритмов коллаборативной фильтрации",
                maxParticipants: 1);
            topicIndividual.Approve(); // Approve for student selection

            var topicTeam = new Topic(
                departmentCS.Id, staffSupervisor1.Id, semester.Id, workTypeDiploma.Id,
                "Разработка платформы для управления дипломными работами",
                directionApproved.Id,
                titleKz: "Дипломдық жұмыстарды басқару платформасын әзірлеу",
                titleEn: "Academic Work Management Platform Development",
                descriptionRu: "Командный проект по разработке системы AWM",
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
                semester.Id, departmentCS.Id, stateWorkDraft.Id, userStudent1.Id,
                topicForApplication.Id);

            work.AddParticipant(student1.Id, 1); // Leader = 1

            db.StudentWorks.Add(work);
            await db.SaveChangesAsync();
        }
    }
}
