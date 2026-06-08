namespace AWM.Service.Infrastructure.Persistence.Seeders;

using AWM.Service.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Seeds RBAC+ reference data: action types, roles, operations tree, and permission matrix.
/// </summary>
internal sealed class AuthSeeder
{
    private readonly ApplicationDbContext _context;

    public AuthSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await SeedRoleActionTypesAsync(ct);
        await SeedRoleAccessesAsync(ct);
        await SeedRoleOperationsAsync(ct);
        await SeedRoleOperationActionsAsync(ct);
        await SeedLocalAccountsAsync(ct);
        await SeedUserAccessesAsync(ct);
    }

    private async Task SeedRoleActionTypesAsync(CancellationToken ct)
    {
        if (await _context.RoleActionTypes.AnyAsync(ct)) return;

        var types = new[]
        {
            new RoleActionType("READ",   "Просмотр",        "Қарау",     "Read"),
            new RoleActionType("CREATE", "Создание",        "Жасау",     "Create"),
            new RoleActionType("UPDATE", "Редактирование",  "Өзгерту",   "Update"),
            new RoleActionType("DELETE", "Удаление",        "Жою",       "Delete"),
        };

        _context.RoleActionTypes.AddRange(types);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedRoleAccessesAsync(CancellationToken ct)
    {
        if (await _context.RoleAccesses.AnyAsync(ct)) return;

        var roles = new[]
        {
            new RoleAccess("ADMIN",                "Администратор",        "Әкімші",               "Administrator",         0),
            new RoleAccess("DEPARTMENT_HEAD",      "Заведующий кафедрой",  "Кафедра меңгерушісі",  "Department Head",        0),
            new RoleAccess("SUPERVISOR",           "Научный руководитель", "Ғылыми жетекші",       "Supervisor",             0),
            new RoleAccess("STUDENT",              "Студент",              "Студент",              "Student",                0),
            new RoleAccess("COMMISSION_CHAIRMAN",  "Председатель ГАК",     "МАК төрағасы",         "Commission Chairman",    0),
            new RoleAccess("COMMISSION_SECRETARY", "Секретарь ГАК",        "МАК хатшысы",          "Commission Secretary",   0),
            new RoleAccess("COMMISSION_MEMBER",    "Член ГАК",             "МАК мүшесі",           "Commission Member",      0),
            new RoleAccess("QUALITY_EXPERT",       "Нормоконтролер",       "Нормабақылаушы",       "Quality Expert",         0),
            new RoleAccess("REVIEWER",             "Рецензент",            "Рецензент",            "Reviewer",               0),
            new RoleAccess("DEAN_OFFICE",          "Деканат",              "Деканат",              "Dean's Office",          0),
        };

        _context.RoleAccesses.AddRange(roles);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedRoleOperationsAsync(CancellationToken ct)
    {
        if (await _context.RoleOperations.AnyAsync(ct)) return;

        // Root nodes
        var thesis = new RoleOperation("THESIS", "Управление ВКР", "ДЖ басқару", "Thesis Management", 0, null, 10);
        var defense = new RoleOperation("DEFENSE", "Защита ВКР", "ДЖ қорғау", "Defense", 0, null, 20);
        var system = new RoleOperation("SYSTEM", "Системное управление", "Жүйелік басқару", "System Management", 0, null, 30);

        _context.RoleOperations.AddRange(thesis, defense, system);
        await _context.SaveChangesAsync(ct);

        // THESIS children
        _context.RoleOperations.AddRange(
            new RoleOperation("THESIS.DIRECTION", "Направления", "Бағыттар", "Directions", 0, thesis.Id, 10),
            new RoleOperation("THESIS.TOPIC", "Темы", "Тақырыптар", "Topics", 0, thesis.Id, 20),
            new RoleOperation("THESIS.WORK", "Студенческие работы", "Студенттік жұмыстар", "Student Works", 0, thesis.Id, 30),
            new RoleOperation("THESIS.ATTACHMENT", "Документы", "Құжаттар", "Documents", 0, thesis.Id, 40),
            new RoleOperation("THESIS.CHECK", "Проверки качества", "Сапа тексерулері", "Quality Checks", 0, thesis.Id, 50),
            new RoleOperation("THESIS.REVIEW", "Рецензии", "Рецензиялар", "Reviews", 0, thesis.Id, 60),
            new RoleOperation("THESIS.APPLICATION", "Заявки на темы", "Тақырыпқа өтінімдер", "Topic Applications", 0, thesis.Id, 70)
        );

        // DEFENSE children
        _context.RoleOperations.AddRange(
            new RoleOperation("DEFENSE.PREDEFENSE", "Предзащита", "Алдын ала қорғау", "Pre-Defense", 0, defense.Id, 10),
            new RoleOperation("DEFENSE.COMMISSION", "Комиссия", "Комиссия", "Commission", 0, defense.Id, 20),
            new RoleOperation("DEFENSE.SCHEDULE", "Расписание", "Кесте", "Schedule", 0, defense.Id, 30),
            new RoleOperation("DEFENSE.GRADE", "Оценки", "Бағалар", "Grades", 0, defense.Id, 40),
            new RoleOperation("DEFENSE.PROTOCOL", "Протоколы", "Хаттамалар", "Protocols", 0, defense.Id, 50)
        );

        // SYSTEM children
        _context.RoleOperations.AddRange(
            new RoleOperation("SYSTEM.ROLE", "Роли и доступ", "Рөлдер мен қол жеткізу", "Roles & Access", 0, system.Id, 10),
            new RoleOperation("SYSTEM.USER", "Пользователи", "Пайдаланушылар", "Users", 0, system.Id, 20),
            new RoleOperation("SYSTEM.STAGE", "Этапы", "Кезеңдер", "Stages", 0, system.Id, 30),
            new RoleOperation("SYSTEM.WORKTYPE", "Типы работ", "Жұмыс түрлері", "Work Types", 0, system.Id, 40),
            new RoleOperation("SYSTEM.CRITERIA", "Критерии оценки", "Бағалау өлшемдері", "Evaluation Criteria", 0, system.Id, 50)
        );

        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedRoleOperationActionsAsync(CancellationToken ct)
    {
        if (await _context.RoleOperationActions.AnyAsync(ct)) return;

        var roles = await _context.RoleAccesses.ToDictionaryAsync(r => r.Code, ct);
        var ops = await _context.RoleOperations.ToDictionaryAsync(o => o.Name, ct);
        var actions = await _context.RoleActionTypes.ToDictionaryAsync(a => a.Code, ct);

        int R = actions["READ"].Id;
        int C = actions["CREATE"].Id;
        int U = actions["UPDATE"].Id;
        int D = actions["DELETE"].Id;
        int[] RCUD = { R, C, U, D };
        int[] RCU = { R, C, U };
        int[] RC = { R, C };
        int[] RU = { R, U };
        int[] Ro = { R };

        var matrix = new List<(string Role, string Op, int[] Actions)>
        {
            // ADMIN — full access to everything
            ("ADMIN", "THESIS",            RCUD),
            ("ADMIN", "THESIS.DIRECTION",  RCUD),
            ("ADMIN", "THESIS.TOPIC",      RCUD),
            ("ADMIN", "THESIS.WORK",       RCUD),
            ("ADMIN", "THESIS.ATTACHMENT", RCUD),
            ("ADMIN", "THESIS.CHECK",      RCUD),
            ("ADMIN", "THESIS.REVIEW",     RCUD),
            ("ADMIN", "THESIS.APPLICATION",RCUD),
            ("ADMIN", "DEFENSE",           RCUD),
            ("ADMIN", "DEFENSE.PREDEFENSE",RCUD),
            ("ADMIN", "DEFENSE.COMMISSION",RCUD),
            ("ADMIN", "DEFENSE.SCHEDULE",  RCUD),
            ("ADMIN", "DEFENSE.GRADE",     RCUD),
            ("ADMIN", "DEFENSE.PROTOCOL",  RCUD),
            ("ADMIN", "SYSTEM",            RCUD),
            ("ADMIN", "SYSTEM.ROLE",       RCUD),
            ("ADMIN", "SYSTEM.USER",       RCUD),
            ("ADMIN", "SYSTEM.STAGE",      RCUD),
            ("ADMIN", "SYSTEM.WORKTYPE",   RCUD),
            ("ADMIN", "SYSTEM.CRITERIA",   RCUD),

            // DEPARTMENT_HEAD
            ("DEPARTMENT_HEAD", "THESIS.DIRECTION",  RCUD),
            ("DEPARTMENT_HEAD", "THESIS.TOPIC",      RCUD),
            ("DEPARTMENT_HEAD", "THESIS.WORK",       Ro),
            ("DEPARTMENT_HEAD", "THESIS.ATTACHMENT", Ro),
            ("DEPARTMENT_HEAD", "THESIS.CHECK",      Ro),
            ("DEPARTMENT_HEAD", "THESIS.REVIEW",     Ro),
            ("DEPARTMENT_HEAD", "THESIS.APPLICATION",RCUD),
            ("DEPARTMENT_HEAD", "DEFENSE.PREDEFENSE",Ro),
            ("DEPARTMENT_HEAD", "DEFENSE.COMMISSION",RCUD),
            ("DEPARTMENT_HEAD", "DEFENSE.SCHEDULE",  RCUD),
            ("DEPARTMENT_HEAD", "DEFENSE.GRADE",     Ro),
            ("DEPARTMENT_HEAD", "DEFENSE.PROTOCOL",  Ro),
            ("DEPARTMENT_HEAD", "SYSTEM.STAGE",      RCUD),
            ("DEPARTMENT_HEAD", "SYSTEM.CRITERIA",   RCUD),

            // SUPERVISOR
            ("SUPERVISOR", "THESIS.DIRECTION",  RCU),
            ("SUPERVISOR", "THESIS.TOPIC",      RCU),
            ("SUPERVISOR", "THESIS.WORK",       Ro),
            ("SUPERVISOR", "THESIS.ATTACHMENT", Ro),
            ("SUPERVISOR", "THESIS.REVIEW",     RC),
            ("SUPERVISOR", "THESIS.APPLICATION",RCUD),
            ("SUPERVISOR", "DEFENSE.PREDEFENSE",Ro),

            // STUDENT
            ("STUDENT", "THESIS.TOPIC",      Ro),
            ("STUDENT", "THESIS.WORK",       RC),
            ("STUDENT", "THESIS.ATTACHMENT", RC),
            ("STUDENT", "THESIS.APPLICATION",RCUD),
            ("STUDENT", "THESIS.CHECK",      Ro),
            ("STUDENT", "DEFENSE.PREDEFENSE",Ro),
            ("STUDENT", "DEFENSE.SCHEDULE",  Ro),

            // COMMISSION_CHAIRMAN
            ("COMMISSION_CHAIRMAN", "THESIS.WORK",        Ro),
            ("COMMISSION_CHAIRMAN", "THESIS.ATTACHMENT",  Ro),
            ("COMMISSION_CHAIRMAN", "DEFENSE.PREDEFENSE", Ro),
            ("COMMISSION_CHAIRMAN", "DEFENSE.COMMISSION", Ro),
            ("COMMISSION_CHAIRMAN", "DEFENSE.SCHEDULE",   Ro),
            ("COMMISSION_CHAIRMAN", "DEFENSE.GRADE",      RCU),
            ("COMMISSION_CHAIRMAN", "DEFENSE.PROTOCOL",   RCU),

            // COMMISSION_SECRETARY
            ("COMMISSION_SECRETARY", "THESIS.WORK",        Ro),
            ("COMMISSION_SECRETARY", "DEFENSE.PREDEFENSE", Ro),
            ("COMMISSION_SECRETARY", "DEFENSE.COMMISSION", Ro),
            ("COMMISSION_SECRETARY", "DEFENSE.SCHEDULE",   RCU),
            ("COMMISSION_SECRETARY", "DEFENSE.PROTOCOL",   RCU),

            // COMMISSION_MEMBER
            ("COMMISSION_MEMBER", "THESIS.WORK",        Ro),
            ("COMMISSION_MEMBER", "THESIS.ATTACHMENT",  Ro),
            ("COMMISSION_MEMBER", "DEFENSE.PREDEFENSE", Ro),
            ("COMMISSION_MEMBER", "DEFENSE.COMMISSION", Ro),
            ("COMMISSION_MEMBER", "DEFENSE.SCHEDULE",   Ro),
            ("COMMISSION_MEMBER", "DEFENSE.GRADE",      RC),
            ("COMMISSION_MEMBER", "DEFENSE.PROTOCOL",   Ro),

            // QUALITY_EXPERT
            ("QUALITY_EXPERT", "THESIS.WORK",       Ro),
            ("QUALITY_EXPERT", "THESIS.ATTACHMENT", Ro),
            ("QUALITY_EXPERT", "THESIS.CHECK",      RCU),

            // REVIEWER
            ("REVIEWER", "THESIS.WORK",       Ro),
            ("REVIEWER", "THESIS.ATTACHMENT", Ro),
            ("REVIEWER", "THESIS.REVIEW",     RC),

            // DEAN_OFFICE
            ("DEAN_OFFICE", "THESIS.DIRECTION",  Ro),
            ("DEAN_OFFICE", "THESIS.TOPIC",      Ro),
            ("DEAN_OFFICE", "THESIS.WORK",       Ro),
            ("DEAN_OFFICE", "THESIS.ATTACHMENT", Ro),
            ("DEAN_OFFICE", "THESIS.CHECK",      Ro),
            ("DEAN_OFFICE", "THESIS.REVIEW",     Ro),
            ("DEAN_OFFICE", "THESIS.APPLICATION",Ro),
            ("DEAN_OFFICE", "DEFENSE.PREDEFENSE",Ro),
            ("DEAN_OFFICE", "DEFENSE.COMMISSION",Ro),
            ("DEAN_OFFICE", "DEFENSE.SCHEDULE",  Ro),
            ("DEAN_OFFICE", "DEFENSE.GRADE",     Ro),
            ("DEAN_OFFICE", "DEFENSE.PROTOCOL",  Ro),
            ("DEAN_OFFICE", "SYSTEM.STAGE",      RCU),
        };

        var permissions = matrix
            .SelectMany(m => m.Actions.Select(actionId =>
                new RoleOperationAction(roles[m.Role].Id, ops[m.Op].Id, actionId)))
            .ToList();

        _context.RoleOperationActions.AddRange(permissions);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedLocalAccountsAsync(CancellationToken ct)
    {
        if (await _context.LocalAccounts.AnyAsync(ct)) return;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");

        var accounts = new[]
        {
            new LocalAccount(1, hashedPassword, 0),
            new LocalAccount(2, hashedPassword, 0),
            new LocalAccount(3, hashedPassword, 0),
            new LocalAccount(10, hashedPassword, 0),
            new LocalAccount(11, hashedPassword, 0),
        };

        _context.LocalAccounts.AddRange(accounts);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedUserAccessesAsync(CancellationToken ct)
    {
        if (await _context.UserAccesses.AnyAsync(ct)) return;

        var roles = await _context.RoleAccesses.ToDictionaryAsync(r => r.Code, ct);

        var accesses = new List<UserAccess>
        {
            new UserAccess(1, roles["ADMIN"].Id, 0),

            new UserAccess(2, roles["SUPERVISOR"].Id, 0),
            new UserAccess(2, roles["DEPARTMENT_HEAD"].Id, 0),

            new UserAccess(3, roles["SUPERVISOR"].Id, 0),

            new UserAccess(10, roles["STUDENT"].Id, 0),

            new UserAccess(11, roles["STUDENT"].Id, 0),
        };

        _context.UserAccesses.AddRange(accesses);
        await _context.SaveChangesAsync(ct);
    }
}
