-- ============================================================================
-- CLEANED UNIVERSITY SCHEMA
-- Only tables and columns that have corresponding C# entities in AWM.Service.
-- Read-only: mapped via UniversityDbContext with ExcludeFromMigrations().
-- ============================================================================

create table Edu_OrgUnitTypes
(
    ID    int not null primary key,
    Title nvarchar(max)
)
go

create table Edu_OrgUnits
(
    ID         int           not null primary key,
    ParentID   int null references Edu_OrgUnits,
    Title      nvarchar(max) not null,
    Deleted    bit           not null,
    ShortTitle nvarchar(max),
    TypeID     int           not null references Edu_OrgUnitTypes on delete cascade
)
go

create table Edu_SpecialityLevels
(
    ID     int           not null primary key,
    Title  nvarchar(max) not null,
    NoBDID nvarchar(max)
)
go

create table Edu_Specialities
(
    ID           int           not null primary key,
    Code         nvarchar(max) not null,
    Title        nvarchar(max) not null,
    YearsOfStudy int,
    Deleted      bit           not null,
    ShortTitle   nvarchar(max),
    LevelID      int           not null references Edu_SpecialityLevels
)
go

create table Edu_Specializations
(
    Id      int not null primary key,
    TitleRu nvarchar(max),
    TitleKz nvarchar(max),
    TitleEn nvarchar(max),
    Code    nvarchar(max)
)
go

create table Edu_SpecialitySpecializations
(
    ID               int not null primary key,
    SpecialityId     int references Edu_Specialities(ID),
    SpecializationId int references Edu_Specializations(Id)
)
go

create table Edu_Specializations_OrgUnits
(
    ID               int not null primary key,
    SpecializationID int references Edu_Specializations(Id),
    OrgUnitID        int references Edu_OrgUnits(ID)
)
go

create table Edu_StudentStatuses
(
    ID     int not null primary key,
    Title  nvarchar(max),
    NOBDID nvarchar(max)
)
go

create table Edu_Users
(
    ID            int           not null primary key,
    LastName      nvarchar(max) not null,
    FirstName     nvarchar(max),
    MiddleName    nvarchar(max),
    Email         nvarchar(max),
    DOB           date,
    Male          bit,
    MobilePhone   nvarchar(max),
    IIN           nvarchar(max),
    PhotoFileName nvarchar(255),
    PhotoFileData varbinary(max)
)
go

create table Edu_Employees
(
    ID          int not null primary key references Edu_Users on delete cascade,
    IsAdvisor   bit not null,
    RoleGroupId int
)
go

create table Edu_Positions
(
    ID          int not null primary key,
    Title       nvarchar(max),
    Deleted     bit not null,
    Description nvarchar(max),
    Lectures    int not null,
    Practices   int not null,
    Labs        int not null,
    CategoryID  int
)
go

create table Edu_EmployeePositions
(
    ID             int           not null primary key,
    StartedOn      date          not null,
    EndedOn        date,
    LastUpdatedBy  nvarchar(max) not null,
    LastUpdatedOn  datetime2(6)  not null,
    Rate           float,
    IsMainPosition bit,
    HrOrderId      int,
    OrgUnitID      int           not null references Edu_OrgUnits on delete cascade,
    PositionID     int           not null references Edu_Positions on delete cascade,
    EmployeeID     int           not null references Edu_Employees on delete cascade
)
go

create table Edu_SemesterTypes
(
    ID      int           not null primary key,
    Title   nvarchar(max) not null,
    OrderBy int           not null
)
go

create table Edu_Semesters
(
    ID             int           not null primary key,
    Title          nvarchar(max) not null,
    StartsOn       datetime2(6)  not null,
    EndsOn         datetime2(6)  not null,
    StudyYear      int           not null,
    SemesterTypeID int           not null references Edu_SemesterTypes on delete cascade
)
go

create table Edu_Students
(
    StudentID              int           not null primary key references Edu_Users on delete cascade,
    SpecialityID           int references Edu_Specialities on delete set null,
    StatusID               int references Edu_StudentStatuses on delete set null,
    CategoryID             int,
    NeedsDorm              bit           not null,
    AltynBelgi             bit           not null,
    Year                   int           not null,
    RupID                  int,
    EntryDate              date,
    GPA                    float,
    LastUpdatedBy          nvarchar(max) not null,
    LastUpdatedOn          datetime2(6)  not null,
    GraduatedOn            datetime2(6),
    AcademicStatusEndsOn   date,
    AcademicStatusStartsOn date,
    GPA_Y                  float,
    IsPersonalDataComplete bit,
    HosterPrivelegeID      int,
    MinorSpecialityID      int,
    EnrollmentTypeId       int,
    EctsGPA                float,
    EctsGPA_Y              float,
    IsScholarship          bit,
    ScholarshipTypeID      int,
    ScholarshipOrderNumber nvarchar(max),
    ScholarshipOrderDate   date,
    ScholarshipDateStart   date,
    ScholarshipDateEnd     date,
    FundingID              int,
    IsKNB                  bit,
    EducationTypeID        int,
    EducationPaymentTypeID int,
    GrantTypeID            int,
    EducationDurationID    int,
    StudyLanguageID        int,
    AcademicStatusID       int,
    AdvisorID              int references Edu_Employees
)
go

PRINT 'Cleaned University Schema created successfully.';
