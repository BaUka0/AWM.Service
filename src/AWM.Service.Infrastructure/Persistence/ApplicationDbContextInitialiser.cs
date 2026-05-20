using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AWM.Service.Infrastructure.Persistence;

public sealed class ApplicationDbContextInitialiser
{
    private readonly ApplicationDbContext _context;
    private readonly UniversityDbContext _universityContext;
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;

    public ApplicationDbContextInitialiser(
        ApplicationDbContext context,
        UniversityDbContext universityContext,
        ILogger<ApplicationDbContextInitialiser> logger)
    {
        _context = context;
        _universityContext = universityContext;
        _logger = logger;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            _logger.LogInformation("Applying migrations to ApplicationDbContext...");
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
            _logger.LogInformation("Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        _logger.LogInformation("Seeding reference data...");

        // 0. University Master Data (Edu_* tables)
        if (!await _universityContext.Users.AnyAsync())
        {
            _logger.LogInformation("Seeding University Master Data via Raw SQL...");
            
            // Edu_SpecialityLevels
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT 1 FROM Edu_SpecialityLevels WHERE ID = 1) INSERT INTO Edu_SpecialityLevels (ID, Title) VALUES (1, N'Бакалавриат'); " +
                "IF NOT EXISTS (SELECT 1 FROM Edu_SpecialityLevels WHERE ID = 2) INSERT INTO Edu_SpecialityLevels (ID, Title) VALUES (2, N'Магистратура'); " +
                "IF NOT EXISTS (SELECT 1 FROM Edu_SpecialityLevels WHERE ID = 3) INSERT INTO Edu_SpecialityLevels (ID, Title) VALUES (3, N'Докторантура');");
            
            // Edu_Specialities
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT 1 FROM Edu_Specialities WHERE ID = 1) INSERT INTO Edu_Specialities (ID, Code, Title, YearsOfStudy, Deleted, LevelID) VALUES (1, '5B070300', N'Информационные системы', 4, 0, 1);");
            
            // Edu_SemesterTypes
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT 1 FROM Edu_SemesterTypes WHERE ID = 1) INSERT INTO Edu_SemesterTypes (ID, Title, OrderBy) VALUES (1, N'Осенний', 1); " +
                "IF NOT EXISTS (SELECT 1 FROM Edu_SemesterTypes WHERE ID = 2) INSERT INTO Edu_SemesterTypes (ID, Title, OrderBy) VALUES (2, N'Весенний', 2);");
            
            // Edu_Semesters
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT 1 FROM Edu_Semesters WHERE ID = 1) INSERT INTO Edu_Semesters (ID, Title, StartsOn, EndsOn, StudyYear, SemesterTypeID) VALUES (1, N'Осенний семестр 2025-2026', '2025-09-01', '2026-01-31', 2025, 1);");
            
            // Edu_OrgUnitTypes
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT 1 FROM Edu_OrgUnitTypes WHERE ID = 1) INSERT INTO Edu_OrgUnitTypes (ID, Title) VALUES (1, N'Кафедра'); " +
                "IF NOT EXISTS (SELECT 1 FROM Edu_OrgUnitTypes WHERE ID = 2) INSERT INTO Edu_OrgUnitTypes (ID, Title) VALUES (2, N'Факультет');");
            
            // Edu_OrgUnits
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT 1 FROM Edu_OrgUnits WHERE ID = 1) INSERT INTO Edu_OrgUnits (ID, Title, Deleted, TypeID) VALUES (1, N'Департамент компьютерных наук', 0, 1);");
            
            // Edu_Positions
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT 1 FROM Edu_Positions WHERE ID = 1) INSERT INTO Edu_Positions (ID, Title, Deleted) VALUES (1, N'Профессор', 0);");
            
            // Edu_StudentStatuses
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT 1 FROM Edu_StudentStatuses WHERE ID = 1) INSERT INTO Edu_StudentStatuses (ID, Title) VALUES (1, N'Активный');");
            
            // Edu_Users
            await _context.Database.ExecuteSqlRawAsync("INSERT INTO Edu_Users (ID, LastName, FirstName, Email, DOB, Male, MobilePhone, IIN) VALUES " +
                "(100, N'Студентов', N'Студент', 'student@univ.edu', '2004-01-01', 1, '87771112233', '040101123456'), " +
                "(200, N'Преподавателев', N'Преподаватель', 'teacher@univ.edu', '1980-01-01', 1, '87774445566', '800101123456'), " +
                "(300, N'Заведующий', N'Завкафедрой', 'head@univ.edu', '1975-01-01', 1, '87779998877', '750101123456'), " +
                "(400, N'Администраторов', N'Администратор', 'admin@univ.edu', '1990-01-01', 1, '87777777777', '900101123456')");
            
            // Edu_Employees
            await _context.Database.ExecuteSqlRawAsync("INSERT INTO Edu_Employees (ID, IsAdvisor) VALUES (200, 1), (300, 1)");
            
            // Edu_EmployeePositions
            await _context.Database.ExecuteSqlRawAsync("INSERT INTO Edu_EmployeePositions (ID, StartedOn, Rate, IsMainPosition, OrgUnitID, PositionID, EmployeeID) VALUES " +
                "(1, '2020-09-01', 1.0, 1, 1, 1, 200), " +
                "(2, '2018-09-01', 1.0, 1, 1, 1, 300)");
            
            // Edu_Students
            await _context.Database.ExecuteSqlRawAsync("INSERT INTO Edu_Students (StudentID, SpecialityID, StatusID, Year, NeedsDorm, AdvisorID) VALUES (100, 1, 1, 4, 0, 200)");
        }

        // 1. Role Action Types
        if (!await _context.RoleActionTypes.AnyAsync())
        {
            _logger.LogInformation("Seeding RoleActionTypes...");
            var read = new RoleActionType("READ", "Просмотр", "Просмотр", "Read");
            var create = new RoleActionType("CREATE", "Создание", "Создание", "Create");
            var update = new RoleActionType("UPDATE", "Редактирование", "Редактирование", "Update");
            var delete = new RoleActionType("DELETE", "Удаление", "Удаление", "Delete");
            
            _context.RoleActionTypes.AddRange(read, create, update, delete);
            await _context.SaveChangesAsync();
        }

        // 2. Role Access
        if (!await _context.RoleAccesses.AnyAsync())
        {
            _logger.LogInformation("Seeding RoleAccesses...");
            var roles = new List<RoleAccess>
            {
                new RoleAccess("ADMIN", "Администратор", "Әкімші", "Administrator", 1),
                new RoleAccess("STUDENT", "Студент", "Студент", "Student", 1),
                new RoleAccess("SUPERVISOR", "Научный руководитель", "Ғылыми жетекші", "Supervisor", 1),
                new RoleAccess("HEAD_OF_DEPARTMENT", "Заведующий кафедрой", "Кафедра меңгерушісі", "Head of Department", 1),
                new RoleAccess("DIRECTOR", "Директор института", "Институт директоры", "Director of Institute", 1),
                new RoleAccess("SECRETARY", "Секретарь комиссии", "Комиссия хатшысы", "Commission Secretary", 1),
                new RoleAccess("EXPERT", "Эксперт", "Сарапшы", "Expert", 1),
                new RoleAccess("REVIEWER", "Рецензент", "Рецензент", "Reviewer", 1),
                new RoleAccess("COMMISSION_MEMBER", "Член комиссии", "Комиссия мүшесі", "Commission Member", 1),
                new RoleAccess("COMMISSION_CHAIR", "Председатель комиссии", "Комиссия төрағасы", "Commission Chair", 1)
            };
            _context.RoleAccesses.AddRange(roles);
            await _context.SaveChangesAsync();
        }

        // 2.5. Local Accounts & User Accesses
        if (!await _context.LocalAccounts.AnyAsync())
        {
            _logger.LogInformation("Seeding LocalAccounts...");
            var hash = "$2a$11$ergkPUv8BMR4PqbrmCJw7uL99CPl7945xl.f5w3sQvNdWnYvkosaW"; // password123
            
            var studentAccount = new LocalAccount(100, hash, createdBy: 0);
            var teacherAccount = new LocalAccount(200, hash, createdBy: 0);
            var headAccount = new LocalAccount(300, hash, createdBy: 0);
            var adminAccount = new LocalAccount(400, hash, createdBy: 0);

            _context.LocalAccounts.AddRange(studentAccount, teacherAccount, headAccount, adminAccount);
            await _context.SaveChangesAsync();
        }

        if (!await _context.UserAccesses.AnyAsync())
        {
            _logger.LogInformation("Seeding UserAccesses...");
            
            var studentAccess = new UserAccess(100, 2, assignedBy: 0); // STUDENT role ID = 2
            var supervisorAccess = new UserAccess(200, 3, assignedBy: 0); // SUPERVISOR role ID = 3
            var headAccess = new UserAccess(300, 4, assignedBy: 0); // HEAD_OF_DEPARTMENT role ID = 4
            var adminAccess = new UserAccess(400, 1, assignedBy: 0); // ADMIN role ID = 1

            _context.UserAccesses.AddRange(studentAccess, supervisorAccess, headAccess, adminAccess);
            await _context.SaveChangesAsync();
        }

        // 3. Role Operations
        if (!await _context.RoleOperations.AnyAsync())
        {
            _logger.LogInformation("Seeding RoleOperations...");
            
            var systemAdmin = new RoleOperation("AdminMenu", "Администрирование", "Әкімшілік", "Administration", 1, null, 1);
            _context.RoleOperations.Add(systemAdmin);
            await _context.SaveChangesAsync();
            
            var users = systemAdmin.AddChild("Users", "Пользователи", "Пайдаланушылар", "Users", 1, 1);
            var rolesOp = systemAdmin.AddChild("Roles", "Роли и права", "Рөлдер мен құқықтар", "Roles & Permissions", 1, 2);
            var settings = systemAdmin.AddChild("Settings", "Настройки", "Баптаулар", "Settings", 1, 3);
            _context.RoleOperations.AddRange(users, rolesOp, settings);
            
            var masterData = new RoleOperation("MasterData", "Справочники", "Анықтамалықтар", "Master Data", 1, null, 2);
            _context.RoleOperations.Add(masterData);
            await _context.SaveChangesAsync();
            
            var specialities = masterData.AddChild("Specialities", "Специальности", "Мамандықтар", "Specialities", 1, 1);
            var orgUnits = masterData.AddChild("OrgUnits", "Кафедры и институты", "Кафедралар мен институттар", "Organizational Units", 1, 2);
            var semesters = masterData.AddChild("Semesters", "Семестры", "Семестрлер", "Semesters", 1, 3);
            var stages = masterData.AddChild("Stages", "Этапы дипломных работ", "Дипломдық жұмыс кезеңдері", "Academic Stages", 1, 4);
            _context.RoleOperations.AddRange(specialities, orgUnits, semesters, stages);
            
            var topicSelection = new RoleOperation("TopicSelection", "Выбор тем", "Тақырып таңдау", "Topic Selection", 1, null, 3);
            _context.RoleOperations.Add(topicSelection);
            await _context.SaveChangesAsync();
            
            var directions = topicSelection.AddChild("Directions", "Направления тем", "Тақырып бағыттары", "Topic Directions", 1, 1);
            var topics = topicSelection.AddChild("Topics", "Темы дипломных", "Дипломдық тақырыптар", "Topics", 1, 2);
            var topicApps = topicSelection.AddChild("TopicApplications", "Заявки на темы", "Тақырыптарға өтінімдер", "Topic Applications", 1, 3);
            _context.RoleOperations.AddRange(directions, topics, topicApps);
            
            var studentWork = new RoleOperation("StudentWork", "Дипломные работы", "Дипломдық жұмыстар", "Student Works", 1, null, 4);
            _context.RoleOperations.Add(studentWork);
            await _context.SaveChangesAsync();
            
            var works = studentWork.AddChild("StudentWorks", "Работы студентов", "Студенттердің жұмыстары", "Student Works", 1, 1);
            var attachments = studentWork.AddChild("Attachments", "Файлы и вложения", "Файлдар мен тіркемелер", "Attachments", 1, 2);
            var qualityChecks = studentWork.AddChild("QualityChecks", "Проверки качества", "Сапаны тексеру", "Quality Checks", 1, 3);
            var reviews = studentWork.AddChild("Reviews", "Рецензии", "Рецензиялар", "Reviews", 1, 4);
            var supervisorReviews = studentWork.AddChild("SupervisorReviews", "Отзывы руководителей", "Жетекшілердің пікірлері", "Supervisor Reviews", 1, 5);
            _context.RoleOperations.AddRange(works, attachments, qualityChecks, reviews, supervisorReviews);
            
            var defense = new RoleOperation("Defense", "Защита", "Қорғау", "Defense", 1, null, 5);
            _context.RoleOperations.Add(defense);
            await _context.SaveChangesAsync();
            
            var commissions = defense.AddChild("Commissions", "Комиссии", "Комиссиялар", "Commissions", 1, 1);
            var schedules = defense.AddChild("Schedules", "Расписание защит", "Қорғау кестесі", "Schedules", 1, 2);
            var preDefense = defense.AddChild("PreDefenseAttempts", "Попытки предзащиты", "Алдын ала қорғау әрекеттері", "Pre-Defense Attempts", 1, 3);
            var grades = defense.AddChild("Grades", "Оценки членов ГАК", "Мемлекеттік комиссия бағалары", "Grades", 1, 4);
            var protocols = defense.AddChild("Protocols", "Протоколы защит", "Қорғау хаттамалары", "Protocols", 1, 5);
            _context.RoleOperations.AddRange(commissions, schedules, preDefense, grades, protocols);
            
            var communications = new RoleOperation("Communications", "Коммуникации", "Байланыс", "Communications", 1, null, 6);
            _context.RoleOperations.Add(communications);
            await _context.SaveChangesAsync();
            
            var notifTemplates = communications.AddChild("NotificationTemplates", "Шаблоны уведомлений", "Хабарлама үлгілері", "Notification Templates", 1, 1);
            var notifications = communications.AddChild("Notifications", "Уведомления", "Хабарламалар", "Notifications", 1, 2);
            _context.RoleOperations.AddRange(notifTemplates, notifications);
            
            await _context.SaveChangesAsync();
        }

        // 4. Role Operation Actions (Permission Matrix)
        if (!await _context.RoleOperationActions.AnyAsync())
        {
            _logger.LogInformation("Seeding RoleOperationActions Matrix...");
            var roles = await _context.RoleAccesses.ToListAsync();
            var operations = await _context.RoleOperations.ToListAsync();
            var actions = await _context.RoleActionTypes.ToListAsync();
            
            var matrix = new List<RoleOperationAction>();
            
            var readAct = actions.First(a => a.Code == "READ");
            var createAct = actions.First(a => a.Code == "CREATE");
            var updateAct = actions.First(a => a.Code == "UPDATE");
            var deleteAct = actions.First(a => a.Code == "DELETE");
            var allActions = new[] { readAct, createAct, updateAct, deleteAct };
            
            int GetRoleId(string code) => roles.First(r => r.Code == code).Id;
            int GetOpId(string name) => operations.First(o => o.Name == name).Id;
            
            // ADMIN gets ALL actions on ALL operations
            var adminId = GetRoleId("ADMIN");
            foreach (var op in operations)
            {
                foreach (var act in allActions)
                {
                    matrix.Add(new RoleOperationAction(adminId, op.Id, act.Id));
                }
            }
            
            // STUDENT permissions
            var studentId = GetRoleId("STUDENT");
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Specialities"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("OrgUnits"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Semesters"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Stages"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Directions"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Topics"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("TopicApplications"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("TopicApplications"), createAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("TopicApplications"), updateAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("TopicApplications"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("StudentWorks"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Attachments"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Attachments"), createAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("QualityChecks"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Reviews"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("SupervisorReviews"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Schedules"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("PreDefenseAttempts"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Grades"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Protocols"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Notifications"), readAct.Id));
            matrix.Add(new RoleOperationAction(studentId, GetOpId("Notifications"), updateAct.Id));

            // SUPERVISOR permissions
            var supervisorId = GetRoleId("SUPERVISOR");
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Specialities"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("OrgUnits"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Semesters"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Stages"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Directions"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Directions"), createAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Directions"), updateAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Directions"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Topics"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Topics"), createAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Topics"), updateAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Topics"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("TopicApplications"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("TopicApplications"), updateAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("StudentWorks"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("StudentWorks"), updateAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Attachments"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("QualityChecks"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Reviews"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("SupervisorReviews"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("SupervisorReviews"), createAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("SupervisorReviews"), updateAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("SupervisorReviews"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Schedules"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("PreDefenseAttempts"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Grades"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Notifications"), readAct.Id));
            matrix.Add(new RoleOperationAction(supervisorId, GetOpId("Notifications"), updateAct.Id));

            // HEAD_OF_DEPARTMENT permissions
            var hodId = GetRoleId("HEAD_OF_DEPARTMENT");
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Specialities"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("OrgUnits"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Semesters"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Stages"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Stages"), createAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Stages"), updateAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Stages"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Directions"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Directions"), updateAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Topics"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Topics"), updateAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("StudentWorks"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Attachments"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("QualityChecks"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Reviews"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("SupervisorReviews"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Commissions"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Commissions"), createAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Commissions"), updateAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Commissions"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Schedules"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Schedules"), createAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Schedules"), updateAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Schedules"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("PreDefenseAttempts"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Grades"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Protocols"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Notifications"), readAct.Id));
            matrix.Add(new RoleOperationAction(hodId, GetOpId("Notifications"), updateAct.Id));

            // EXPERT permissions
            var expertId = GetRoleId("EXPERT");
            matrix.Add(new RoleOperationAction(expertId, GetOpId("StudentWorks"), readAct.Id));
            matrix.Add(new RoleOperationAction(expertId, GetOpId("Attachments"), readAct.Id));
            matrix.Add(new RoleOperationAction(expertId, GetOpId("QualityChecks"), readAct.Id));
            matrix.Add(new RoleOperationAction(expertId, GetOpId("QualityChecks"), createAct.Id));
            matrix.Add(new RoleOperationAction(expertId, GetOpId("QualityChecks"), updateAct.Id));
            matrix.Add(new RoleOperationAction(expertId, GetOpId("QualityChecks"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(expertId, GetOpId("Notifications"), readAct.Id));
            matrix.Add(new RoleOperationAction(expertId, GetOpId("Notifications"), updateAct.Id));

            // REVIEWER permissions
            var reviewerRoleId = GetRoleId("REVIEWER");
            matrix.Add(new RoleOperationAction(reviewerRoleId, GetOpId("StudentWorks"), readAct.Id));
            matrix.Add(new RoleOperationAction(reviewerRoleId, GetOpId("Attachments"), readAct.Id));
            matrix.Add(new RoleOperationAction(reviewerRoleId, GetOpId("Reviews"), readAct.Id));
            matrix.Add(new RoleOperationAction(reviewerRoleId, GetOpId("Reviews"), createAct.Id));
            matrix.Add(new RoleOperationAction(reviewerRoleId, GetOpId("Reviews"), updateAct.Id));
            matrix.Add(new RoleOperationAction(reviewerRoleId, GetOpId("Reviews"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(reviewerRoleId, GetOpId("Notifications"), readAct.Id));
            matrix.Add(new RoleOperationAction(reviewerRoleId, GetOpId("Notifications"), updateAct.Id));

            // SECRETARY permissions
            var secretaryId = GetRoleId("SECRETARY");
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("StudentWorks"), readAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Attachments"), readAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Commissions"), readAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Commissions"), createAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Commissions"), updateAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Commissions"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Schedules"), readAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Schedules"), createAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Schedules"), updateAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Schedules"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("PreDefenseAttempts"), readAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("PreDefenseAttempts"), createAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("PreDefenseAttempts"), updateAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("PreDefenseAttempts"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Grades"), readAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Grades"), createAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Grades"), updateAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Grades"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Protocols"), readAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Protocols"), createAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Protocols"), updateAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Protocols"), deleteAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Notifications"), readAct.Id));
            matrix.Add(new RoleOperationAction(secretaryId, GetOpId("Notifications"), updateAct.Id));

            // COMMISSION_MEMBER permissions
            var memberId = GetRoleId("COMMISSION_MEMBER");
            matrix.Add(new RoleOperationAction(memberId, GetOpId("StudentWorks"), readAct.Id));
            matrix.Add(new RoleOperationAction(memberId, GetOpId("Schedules"), readAct.Id));
            matrix.Add(new RoleOperationAction(memberId, GetOpId("PreDefenseAttempts"), readAct.Id));
            matrix.Add(new RoleOperationAction(memberId, GetOpId("Grades"), readAct.Id));
            matrix.Add(new RoleOperationAction(memberId, GetOpId("Grades"), createAct.Id));
            matrix.Add(new RoleOperationAction(memberId, GetOpId("Grades"), updateAct.Id));
            matrix.Add(new RoleOperationAction(memberId, GetOpId("Protocols"), readAct.Id));
            matrix.Add(new RoleOperationAction(memberId, GetOpId("Notifications"), readAct.Id));

            // COMMISSION_CHAIR permissions
            var chairId = GetRoleId("COMMISSION_CHAIR");
            matrix.Add(new RoleOperationAction(chairId, GetOpId("StudentWorks"), readAct.Id));
            matrix.Add(new RoleOperationAction(chairId, GetOpId("Schedules"), readAct.Id));
            matrix.Add(new RoleOperationAction(chairId, GetOpId("PreDefenseAttempts"), readAct.Id));
            matrix.Add(new RoleOperationAction(chairId, GetOpId("Grades"), readAct.Id));
            matrix.Add(new RoleOperationAction(chairId, GetOpId("Grades"), createAct.Id));
            matrix.Add(new RoleOperationAction(chairId, GetOpId("Grades"), updateAct.Id));
            matrix.Add(new RoleOperationAction(chairId, GetOpId("Protocols"), readAct.Id));
            matrix.Add(new RoleOperationAction(chairId, GetOpId("Protocols"), updateAct.Id));
            matrix.Add(new RoleOperationAction(chairId, GetOpId("Notifications"), readAct.Id));

            _context.RoleOperationActions.AddRange(matrix);
            await _context.SaveChangesAsync();
        }

        // 5. Application Statuses (Thesis)
        if (!await _context.ApplicationStatuses.AnyAsync())
        {
            _logger.LogInformation("Seeding ApplicationStatuses...");
            await SeedWithIdentityInsertAsync(
                _context.ApplicationStatuses,
                "Thesis",
                "ApplicationStatuses",
                new ApplicationStatus(1, "Pending"),
                new ApplicationStatus(2, "Approved"),
                new ApplicationStatus(3, "Rejected")
            );
        }

        // 7. Attachment Types (Thesis)
        if (!await _context.AttachmentTypes.AnyAsync())
        {
            _logger.LogInformation("Seeding AttachmentTypes...");
            await SeedWithIdentityInsertAsync(
                _context.AttachmentTypes,
                "Thesis",
                "AttachmentTypes",
                new AttachmentType(1, "TaskDescription"),
                new AttachmentType(2, "WorkDraft"),
                new AttachmentType(3, "FinalWork"),
                new AttachmentType(4, "Presentation"),
                new AttachmentType(5, "AntiplagiarismReport")
            );
        }

        // 8. Check Types (Thesis)
        if (!await _context.CheckTypes.AnyAsync())
        {
            _logger.LogInformation("Seeding CheckTypes...");
            await SeedWithIdentityInsertAsync(
                _context.CheckTypes,
                "Thesis",
                "CheckTypes",
                new CheckType(1, "NormControl"),
                new CheckType(2, "AntiPlagiarism")
            );
        }

        // 9. Commission Types (Defense)
        if (!await _context.CommissionTypes.AnyAsync())
        {
            _logger.LogInformation("Seeding CommissionTypes...");
            await SeedWithIdentityInsertAsync(
                _context.CommissionTypes,
                "Defense",
                "CommissionTypes",
                new CommissionType(1, "PreDefense"),
                new CommissionType(2, "Defense")
            );
        }

        // 10. Commission Roles (Defense)
        if (!await _context.CommissionRoles.AnyAsync())
        {
            _logger.LogInformation("Seeding CommissionRoles...");
            await SeedWithIdentityInsertAsync(
                _context.CommissionRoles,
                "Defense",
                "CommissionRoles",
                new CommissionRole(1, "Chair"),
                new CommissionRole(2, "Member"),
                new CommissionRole(3, "Secretary")
            );
        }

        // 11. Attendance Statuses (Defense)
        if (!await _context.AttendanceStatuses.AnyAsync())
        {
            _logger.LogInformation("Seeding AttendanceStatuses...");
            await SeedWithIdentityInsertAsync(
                _context.AttendanceStatuses,
                "Defense",
                "AttendanceStatuses",
                new AttendanceStatus(1, "Present"),
                new AttendanceStatus(2, "Absent"),
                new AttendanceStatus(3, "Excused")
            );
        }

        // 12. Workflow Stages (Common)
        if (!await _context.WorkflowStages.AnyAsync())
        {
            _logger.LogInformation("Seeding WorkflowStages...");
            _context.WorkflowStages.AddRange(
                new WorkflowStage("TopicProposal", 1),
                new WorkflowStage("Preparation", 2),
                new WorkflowStage("PreDefense", 3),
                new WorkflowStage("Review", 4),
                new WorkflowStage("Defense", 5)
            );
            await _context.SaveChangesAsync();
        }

        // 13. Notification Templates (Common)
        if (!await _context.NotificationTemplates.AnyAsync())
        {
            _logger.LogInformation("Seeding NotificationTemplates...");
            _context.NotificationTemplates.AddRange(
                new NotificationTemplate(
                    "TopicApproved",
                    "Тема утверждена",
                    "Ваша тема '{TopicTitle}' была успешно утверждена.",
                    1,
                    "Тақырып мақұлданды",
                    "Сіздің '{TopicTitle}' тақырыбыңыз сәтті мақұлданды.",
                    "Topic Approved",
                    "Your topic '{TopicTitle}' has been successfully approved."
                ),
                new NotificationTemplate(
                    "TopicRejected",
                    "Тема отклонена",
                    "Ваша тема '{TopicTitle}' была отклонена по причине: {Comment}.",
                    1,
                    "Тақырып қабылданбады",
                    "Сіздің '{TopicTitle}' тақырыбыңыз қабылданбады, себебі: {Comment}.",
                    "Topic Rejected",
                    "Your topic '{TopicTitle}' was rejected for the following reason: {Comment}."
                ),
                new NotificationTemplate(
                    "NewApplication",
                    "Новая заявка на тему",
                    "Студент {StudentName} подал заявку на вашу тему '{TopicTitle}'.",
                    1,
                    "Тақырыпқа жаңа өтінім",
                    "Студент {StudentName} сіздің '{TopicTitle}' тақырыбыңызға өтінім берді.",
                    "New Topic Application",
                    "Student {StudentName} has applied for your topic '{TopicTitle}'."
                ),
                new NotificationTemplate(
                    "QualityCheckFailed",
                    "Проверка качества не пройдена",
                    "Ваша работа не прошла проверку: {CheckType}. Комментарий: {Comment}.",
                    1,
                    "Сапаны тексеру сәтсіз аяқталды",
                    "Сіздің жұмысыңыз тексеруден өтпеді: {CheckType}. Түсініктеме: {Comment}.",
                    "Quality Check Failed",
                    "Your work did not pass the check: {CheckType}. Comment: {Comment}."
                ),
                new NotificationTemplate(
                    "PreDefenseAssigned",
                    "Назначена предзащита",
                    "Вам назначена предзащита на {Date} в аудитории {Location}.",
                    1,
                    "Алдын ала қорғау тағайындалды",
                    "Сізге {Date} күні {Location} аудиториясында алдын ала қорғау тағайындалды.",
                    "Pre-Defense Assigned",
                    "A pre-defense has been assigned to you on {Date} at room {Location}."
                )
            );
            await _context.SaveChangesAsync();
        }

        // 14. Work Types (Wf)
        if (!await _context.WorkTypes.AnyAsync())
        {
            _logger.LogInformation("Seeding WorkTypes...");
            var allLevels = await _universityContext.SpecialityLevels.ToListAsync();

            var bachelorLevel = allLevels.FirstOrDefault(l => l.Title.Contains("Бакалавриат", StringComparison.OrdinalIgnoreCase) || l.Title.Contains("Bachelor", StringComparison.OrdinalIgnoreCase));
            var masterLevel = allLevels.FirstOrDefault(l => l.Title.Contains("Магистратура", StringComparison.OrdinalIgnoreCase) || l.Title.Contains("Master", StringComparison.OrdinalIgnoreCase));
            var phdLevel = allLevels.FirstOrDefault(l => l.Title.Contains("Докторантура", StringComparison.OrdinalIgnoreCase) || l.Title.Contains("PhD", StringComparison.OrdinalIgnoreCase));

            var courseWork = WorkType.CourseWork(1);
            var diplomaWork = WorkType.DiplomaWork(bachelorLevel?.Id ?? 1, 1);
            var masterThesis = WorkType.MasterThesis(masterLevel?.Id ?? 2, 1);
            var phdThesis = WorkType.PhD(phdLevel?.Id ?? 3, 1);
            
            _context.WorkTypes.AddRange(courseWork, diplomaWork, masterThesis, phdThesis);
            await _context.SaveChangesAsync();
        }

        // 15. States and Transitions for Work Types (Wf)
        var diplomaWorkType = await _context.WorkTypes.FirstOrDefaultAsync(wt => wt.Name == "DiplomaWork");
        if (diplomaWorkType != null)
        {
            if (!await _context.States.AnyAsync(s => s.WorkTypeId == diplomaWorkType.Id))
            {
                _logger.LogInformation("Seeding States for DiplomaWork workflow...");
                var draft = new State(diplomaWorkType.Id, "Draft", 1, "Черновик");
                var onReview = new State(diplomaWorkType.Id, "OnReview", 1, "На рецензировании");
                var normControl = new State(diplomaWorkType.Id, "NormControl", 1, "Нормоконтроль");
                var softwareCheck = new State(diplomaWorkType.Id, "SoftwareCheck", 1, "Тексеру (ПО)");
                var antiPlagiarism = new State(diplomaWorkType.Id, "AntiPlagiarism", 1, "Антиплагиат");
                var preDefense1 = new State(diplomaWorkType.Id, "PreDefense1", 1, "Предзащита 1");
                var preDefense2 = new State(diplomaWorkType.Id, "PreDefense2", 1, "Предзащита 2");
                var readyForDefense = new State(diplomaWorkType.Id, "ReadyForDefense", 1, "Готов к защите");
                var defended = new State(diplomaWorkType.Id, "Defended", 1, "Защищено", isFinal: true);
                var cancelled = new State(diplomaWorkType.Id, "Cancelled", 1, "Отменено", isFinal: true);
                
                _context.States.AddRange(draft, onReview, normControl, softwareCheck, antiPlagiarism, preDefense1, preDefense2, readyForDefense, defended, cancelled);
                await _context.SaveChangesAsync();
            }

            var states = await _context.States.Where(s => s.WorkTypeId == diplomaWorkType.Id).ToListAsync();
            var draftState = states.First(s => s.SystemName == "Draft");
            var onReviewState = states.First(s => s.SystemName == "OnReview");
            var normControlState = states.First(s => s.SystemName == "NormControl");
            var softwareCheckState = states.First(s => s.SystemName == "SoftwareCheck");
            var antiPlagiarismState = states.First(s => s.SystemName == "AntiPlagiarism");
            var preDefense1State = states.First(s => s.SystemName == "PreDefense1");
            var preDefense2State = states.First(s => s.SystemName == "PreDefense2");
            var readyForDefenseState = states.First(s => s.SystemName == "ReadyForDefense");
            var defendedState = states.First(s => s.SystemName == "Defended");
            var cancelledState = states.First(s => s.SystemName == "Cancelled");

            if (!await _context.Transitions.AnyAsync(t => t.FromStateId == draftState.Id))
            {
                _logger.LogInformation("Seeding Transitions for DiplomaWork workflow...");
                var studentRole = await _context.RoleAccesses.FirstOrDefaultAsync(r => r.Code == "STUDENT");
                var supervisorRole = await _context.RoleAccesses.FirstOrDefaultAsync(r => r.Code == "SUPERVISOR");
                var expertRole = await _context.RoleAccesses.FirstOrDefaultAsync(r => r.Code == "EXPERT");
                var secretaryRole = await _context.RoleAccesses.FirstOrDefaultAsync(r => r.Code == "SECRETARY");
                var chairRole = await _context.RoleAccesses.FirstOrDefaultAsync(r => r.Code == "COMMISSION_CHAIR");

                var transitions = new List<Transition>
                {
                    new Transition(draftState.Id, onReviewState.Id, 1, studentRole?.Id),
                    new Transition(onReviewState.Id, normControlState.Id, 1, supervisorRole?.Id),
                    new Transition(onReviewState.Id, draftState.Id, 1, supervisorRole?.Id),
                    new Transition(normControlState.Id, softwareCheckState.Id, 1, expertRole?.Id),
                    new Transition(normControlState.Id, draftState.Id, 1, expertRole?.Id),
                    new Transition(softwareCheckState.Id, antiPlagiarismState.Id, 1, expertRole?.Id),
                    new Transition(softwareCheckState.Id, draftState.Id, 1, expertRole?.Id),
                    new Transition(antiPlagiarismState.Id, preDefense1State.Id, 1, expertRole?.Id),
                    new Transition(antiPlagiarismState.Id, draftState.Id, 1, expertRole?.Id),
                    new Transition(preDefense1State.Id, preDefense2State.Id, 1, secretaryRole?.Id),
                    new Transition(preDefense1State.Id, readyForDefenseState.Id, 1, secretaryRole?.Id),
                    new Transition(preDefense2State.Id, readyForDefenseState.Id, 1, secretaryRole?.Id),
                    new Transition(preDefense2State.Id, cancelledState.Id, 1, secretaryRole?.Id),
                    new Transition(readyForDefenseState.Id, defendedState.Id, 1, chairRole?.Id),
                    new Transition(draftState.Id, cancelledState.Id, 1, supervisorRole?.Id)
                };
                
                _context.Transitions.AddRange(transitions);
                await _context.SaveChangesAsync();
            }
        }

        _logger.LogInformation("Reference data seeded successfully.");
    }

    private async Task SeedWithIdentityInsertAsync<TEntity>(
        DbSet<TEntity> dbSet,
        string schema,
        string table,
        params TEntity[] entities) where TEntity : class
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{schema}].[{table}] ON");
                dbSet.AddRange(entities);
                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{schema}].[{table}] OFF");
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
