
using System;
using AWM.Service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.LocalAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("UQ_LocalAccount_UserId");

                    b.ToTable("LocalAccounts", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleAccess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameKz")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameRu")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("UQ_RoleAccess_Code");

                    b.ToTable("RoleAccesses", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleActionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NameKz")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NameRu")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("UQ_RoleActionType_Code");

                    b.ToTable("RoleActionTypes", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleOperation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameKz")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameRu")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("OrderBy")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentId", "OrderBy")
                        .HasDatabaseName("IX_RoleOperations_Tree");

                    b.ToTable("RoleOperations", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleOperationAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("RoleAccessId")
                        .HasColumnType("int");

                    b.Property<int>("RoleActionTypeId")
                        .HasColumnType("int");

                    b.Property<int>("RoleOperationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleActionTypeId");

                    b.HasIndex("RoleOperationId");

                    b.HasIndex("RoleAccessId", "RoleOperationId", "RoleActionTypeId")
                        .IsUnique()
                        .HasDatabaseName("UQ_RoleOperationAction");

                    b.ToTable("RoleOperationActions", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.UserAccess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AssignedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("AssignedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("RoleAccessId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleAccessId");

                    b.HasIndex("UserId", "RoleAccessId")
                        .IsUnique()
                        .HasDatabaseName("UQ_UserAccess");

                    b.ToTable("UserAccesses", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.UserAccessHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("AssignedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("AssignedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("RoleAccessId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleAccessId")
                        .HasDatabaseName("IX_UserAccessHistory_Role");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_UserAccessHistory_User");

                    b.ToTable("UserAccessHistories", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.ViewModels.ReducedUserAccessMatrix", b =>
                {
                    b.Property<string>("RoleCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.ToTable((string)null);

                    b.ToView("ReducedUserAccessMatrix", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.ViewModels.RoleAccessMatrix", b =>
                {
                    b.Property<string>("ActionTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OperationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable((string)null);

                    b.ToView("RoleAccessMatrix", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.ViewModels.UserAccessMatrix", b =>
                {
                    b.Property<string>("ActionTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OperationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.ToTable((string)null);

                    b.ToView("UserAccessMatrix", "Auth");
                });

            modelBuilder.Entity("AWM.Service.Domain.CommonDomain.Entities.Notification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsRead")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<long?>("RelatedEntityId")
                        .HasColumnType("bigint");

                    b.Property<string>("RelatedEntityType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("TemplateId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TemplateId");

                    b.HasIndex("RelatedEntityType", "RelatedEntityId")
                        .HasDatabaseName("IX_Notif_Entity");

                    b.HasIndex("UserId", "IsRead", "CreatedAt")
                        .IsDescending(false, false, true)
                        .HasDatabaseName("IX_Notif_User_Unread");

                    b.ToTable("Notifications", "Common");
                });

            modelBuilder.Entity("AWM.Service.Domain.CommonDomain.Entities.NotificationTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BodyTemplateEn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BodyTemplateKz")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BodyTemplateRu")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("TitleEn")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TitleKz")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TitleRu")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("EventType")
                        .IsUnique()
                        .HasDatabaseName("UQ_Template_Event")
                        .HasFilter("[IsDeleted] = 0");

                    b.ToTable("NotificationTemplates", "Common");
                });

            modelBuilder.Entity("AWM.Service.Domain.CommonDomain.Entities.StaffAssignment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int?>("CommissionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("MetadataJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleType")
                        .HasColumnType("int");

                    b.Property<long>("TargetEntityId")
                        .HasColumnType("bigint");

                    b.Property<string>("TargetEntityType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ValidTo")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CommissionId");

                    b.HasIndex("IsActive");

                    b.HasIndex("UserId");

                    b.HasIndex("TargetEntityType", "TargetEntityId");

                    b.ToTable("StaffAssignments", "Common");
                });

            modelBuilder.Entity("AWM.Service.Domain.CommonDomain.Entities.Stage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("OrgUnitId")
                        .HasColumnType("int")
                        .HasColumnName("OrgUnitId");

                    b.Property<int>("SemesterId")
                        .HasColumnType("int");

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int")
                        .HasColumnName("SpecialityId");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("WorkflowStageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SemesterId");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("WorkflowStageId");

                    b.HasIndex("OrgUnitId", "SpecialityId", "SemesterId", "WorkflowStageId")
                        .HasDatabaseName("IX_Stages_Active")
                        .HasFilter("[IsActive] = 1");

                    b.ToTable("Stages", "Common", t =>
                        {
                            t.HasCheckConstraint("Check_Stage_Dates", "[EndDate] > [StartDate]");
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.CommonDomain.Entities.WorkflowStage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("OrderBy")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.ToTable("WorkflowStages", "Common");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Commission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CommissionTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("OrgUnitId")
                        .HasColumnType("int");

                    b.Property<int?>("PreDefenseNumber")
                        .HasColumnType("int");

                    b.Property<int>("SemesterId")
                        .HasColumnType("int");

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrgUnitId");

                    b.HasIndex("SemesterId");

                    b.HasIndex("SpecialityId");

                    b.ToTable("Commissions", "Defense", t =>
                        {
                            t.HasCheckConstraint("Check_Commission_PreDefNum", "[PreDefenseNumber] IS NULL OR [PreDefenseNumber] BETWEEN 1 AND 3");
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.EvaluationCriteria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CriteriaName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("DefenseStageType")
                        .HasColumnType("int")
                        .HasColumnName("DefenseStageType");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("MaxScore")
                        .HasColumnType("int");

                    b.Property<int?>("OrgUnitId")
                        .HasColumnType("int")
                        .HasColumnName("OrgUnitId");

                    b.Property<int>("SortOrder")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int")
                        .HasColumnName("SpecialityId");

                    b.Property<decimal>("Weight")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(5,2)")
                        .HasDefaultValue(1.0m);

                    b.Property<int>("WorkTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrgUnitId");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("WorkTypeId");

                    b.ToTable("EvaluationCriteria", "Defense");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Grade", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AssignmentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<int>("CriteriaId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<long>("ScheduleId")
                        .HasColumnType("bigint");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime");

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.HasIndex("CriteriaId");

                    b.HasIndex("ScheduleId", "AssignmentId", "CriteriaId")
                        .IsUnique()
                        .HasDatabaseName("UQ_Grade_Schedule_Assignment_Criteria");

                    b.ToTable("Grades", "Defense", t =>
                        {
                            t.HasCheckConstraint("Check_Score_Positive", "[Score] >= 0");
                        });

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("GradesHistory", "Defense");
                                ttb
                                    .HasPeriodStart("SysStartTime")
                                    .HasColumnName("SysStartTime");
                                ttb
                                    .HasPeriodEnd("SysEndTime")
                                    .HasColumnName("SysEndTime");
                            }));
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.PreDefenseAttempt", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("AttemptDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("AttendanceStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<decimal?>("AverageScore")
                        .HasColumnType("decimal(5,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsPassed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("PreDefenseNumber")
                        .HasColumnType("int");

                    b.Property<long?>("ScheduleId")
                        .HasColumnType("bigint");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("WorkId", "PreDefenseNumber")
                        .IsUnique()
                        .HasDatabaseName("IX_PreDefAttempts_Work");

                    b.ToTable("PreDefenseAttempts", "Defense", t =>
                        {
                            t.HasCheckConstraint("Check_PreDefNum", "[PreDefenseNumber] BETWEEN 1 AND 3");
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Protocol", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CommissionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("Decision")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DecisionType")
                        .HasColumnType("int")
                        .HasColumnName("DecisionType");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<string>("DocumentPath")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("FinalGradeLetter")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<decimal?>("FinalScoreNumeric")
                        .HasColumnType("decimal(5,2)");

                    b.Property<DateTime?>("FinalizedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FinalizedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsFinalized")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("IsSigned");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ProtocolNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("ReadinessPercent")
                        .HasColumnType("int")
                        .HasColumnName("ReadinessPercent");

                    b.Property<long>("ScheduleId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("SessionDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("ProtocolDate");

                    b.HasKey("Id");

                    b.HasIndex("CommissionId");

                    b.HasIndex("FinalizedBy");

                    b.HasIndex("ScheduleId")
                        .IsUnique()
                        .HasDatabaseName("UQ_Protocol_Schedule")
                        .HasFilter("[IsDeleted] = 0");

                    b.ToTable("Protocols", "Defense");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Schedule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("CommissionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("DefenseDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsReconciliationStarted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("WorkId");

                    b.HasIndex("CommissionId", "WorkId")
                        .IsUnique()
                        .HasDatabaseName("UQ_Schedule_Commission_Work")
                        .HasFilter("[IsDeleted] = 0");

                    b.ToTable("Schedules", "Defense");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Attachment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("AttachmentTypeId")
                        .HasColumnType("int");

                    b.Property<int?>("AttachmentTypeId1")
                        .HasColumnType("int");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("FileHash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("FileSizeBytes")
                        .HasColumnType("bigint");

                    b.Property<string>("FileStoragePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int?>("StateId")
                        .HasColumnType("int");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AttachmentTypeId");

                    b.HasIndex("AttachmentTypeId1");

                    b.HasIndex("FileHash")
                        .HasDatabaseName("IX_Attach_Hash");

                    b.HasIndex("StateId");

                    b.HasIndex("WorkId")
                        .HasDatabaseName("IX_Attach_Work");

                    b.ToTable("Attachments", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.AttachmentType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("AttachmentTypes", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.CheckType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("HasNumericResult")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("CheckTypes", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Direction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<int>("CurrentStateId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<string>("DescriptionEn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescriptionKz")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescriptionRu")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("OrgUnitId")
                        .HasColumnType("int");

                    b.Property<string>("ReviewComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReviewedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ReviewedBy")
                        .HasColumnType("int");

                    b.Property<int>("SemesterId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SubmittedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime");

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime");

                    b.Property<string>("TitleEn")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("TitleKz")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("TitleRu")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("WorkTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CurrentStateId");

                    b.HasIndex("SemesterId");

                    b.HasIndex("WorkTypeId");

                    b.HasIndex("OrgUnitId", "SemesterId", "CurrentStateId")
                        .HasDatabaseName("IX_Directions_Dept_Year");

                    b.ToTable("Directions", "Thesis");

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("DirectionsHistory", "Thesis");
                                ttb
                                    .HasPeriodStart("SysStartTime")
                                    .HasColumnName("SysStartTime");
                                ttb
                                    .HasPeriodEnd("SysEndTime")
                                    .HasColumnName("SysEndTime");
                            }));
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.QualityCheck", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int?>("AssignedExpertId")
                        .HasColumnType("int");

                    b.Property<long?>("AttachmentId")
                        .HasColumnType("bigint");

                    b.Property<int>("AttemptNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int>("CheckTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsPassed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<decimal?>("ResultValue")
                        .HasColumnType("decimal(5,2)");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AttachmentId");

                    b.HasIndex("CheckTypeId");

                    b.HasIndex("WorkId", "CheckTypeId", "AttemptNumber")
                        .HasDatabaseName("IX_QualityChecks_Work");

                    b.ToTable("QualityChecks", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Reviewer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AcademicDegree")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("Organization")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Phone")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Position")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Reviewers", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.SpecialityCheckType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CheckTypeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<decimal?>("MinimumPassValue")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int>("OrgUnitId")
                        .HasColumnType("int");

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CheckTypeId");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("OrgUnitId", "SpecialityId", "CheckTypeId")
                        .IsUnique()
                        .HasDatabaseName("UQ_OrgUnit_Speciality_CheckType")
                        .HasFilter("[SpecialityId] IS NOT NULL");

                    b.ToTable("SpecialityCheckTypes", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.StudentWork", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<int>("CurrentStateId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<string>("FinalGrade")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<bool>("IsDefended")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("MetadataJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrgUnitId")
                        .HasColumnType("int");

                    b.Property<int>("SemesterId")
                        .HasColumnType("int");

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime");

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime");

                    b.Property<long?>("TopicId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CurrentStateId");

                    b.HasIndex("SemesterId");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("TopicId")
                        .IsUnique()
                        .HasDatabaseName("UQ_Works_Topic")
                        .HasFilter("[TopicId] IS NOT NULL");

                    b.HasIndex("OrgUnitId", "SemesterId", "CurrentStateId")
                        .HasDatabaseName("IX_StudentWorks_Filter");

                    b.ToTable("StudentWorks", "Thesis");

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("StudentWorksHistory", "Thesis");
                                ttb
                                    .HasPeriodStart("SysStartTime")
                                    .HasColumnName("SysStartTime");
                                ttb
                                    .HasPeriodEnd("SysEndTime")
                                    .HasColumnName("SysEndTime");
                            }));
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Topic", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<string>("DescriptionEn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescriptionKz")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescriptionRu")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("DirectionId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("MaxParticipants")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int>("OrgUnitId")
                        .HasColumnType("int");

                    b.Property<string>("ReviewComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReviewedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ReviewedBy")
                        .HasColumnType("int");

                    b.Property<int>("SemesterId")
                        .HasColumnType("int");

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("Status");

                    b.Property<DateTime?>("SubmittedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime");

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime");

                    b.Property<string>("TitleEn")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("TitleKz")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("TitleRu")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("WorkTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DirectionId")
                        .HasDatabaseName("IX_Topics_Direction");

                    b.HasIndex("SemesterId");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("WorkTypeId");

                    b.HasIndex("OrgUnitId", "SemesterId", "Status")
                        .HasDatabaseName("IX_Topics_Filter");

                    b.ToTable("Topics", "Thesis", t =>
                        {
                            t.HasCheckConstraint("Check_Participants_Positive", "[MaxParticipants] BETWEEN 1 AND 5");
                        });

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("TopicsHistory", "Thesis");
                                ttb
                                    .HasPeriodStart("SysStartTime")
                                    .HasColumnName("SysStartTime");
                                ttb
                                    .HasPeriodEnd("SysEndTime")
                                    .HasColumnName("SysEndTime");
                            }));
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.TopicApplication", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("AppliedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("MotivationLetter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReviewedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ReviewedBy")
                        .HasColumnType("int");

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int")
                        .HasColumnName("SpecialityId");

                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.Property<long>("TopicId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ReviewedBy");

                    b.HasIndex("TopicId");

                    b.HasIndex("StatusId", "TopicId")
                        .HasDatabaseName("IX_Applications_Status");

                    b.HasIndex("StudentId", "StatusId")
                        .HasDatabaseName("IX_Applications_Student");

                    b.ToTable("TopicApplications", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.WorkParticipant", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("StudentId")
                        .HasDatabaseName("IX_Participants_Student");

                    b.HasIndex("WorkId")
                        .HasDatabaseName("IX_Participants_Work");

                    b.HasIndex("WorkId", "StudentId")
                        .IsUnique()
                        .HasDatabaseName("UQ_Participant_Work_Student");

                    b.ToTable("WorkParticipants", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.WorkReview", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("AuthorUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsFinal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("MetadataJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("WorkId");

                    b.ToTable("WorkReviews", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.WorkflowHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<int?>("FromStateId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int>("ToStateId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("FromStateId");

                    b.HasIndex("ToStateId");

                    b.HasIndex("UserId");

                    b.HasIndex("WorkId", "CreatedAt")
                        .IsDescending(false, true)
                        .HasDatabaseName("IX_WfHist_Work");

                    b.ToTable("WorkflowHistory", "Thesis");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Employee", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<bool>("IsAdvisor")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Edu_Employees", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.EmployeePosition", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int")
                        .HasColumnName("EmployeeID");

                    b.Property<DateTime?>("EndedOn")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsMainPosition")
                        .HasColumnType("bit");

                    b.Property<int>("OrgUnitId")
                        .HasColumnType("int")
                        .HasColumnName("OrgUnitID");

                    b.Property<int>("PositionId")
                        .HasColumnType("int")
                        .HasColumnName("PositionID");

                    b.Property<decimal?>("Rate")
                        .HasColumnType("decimal(5,2)");

                    b.Property<DateTime?>("StartedOn")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId")
                        .HasDatabaseName("IX_Edu_EmployeePositions_EmployeeId");

                    b.HasIndex("OrgUnitId");

                    b.HasIndex("PositionId");

                    b.ToTable("Edu_EmployeePositions", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.OrgUnit", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("ShortTitle")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("TypeId")
                        .HasDatabaseName("IX_Edu_OrgUnits_TypeId");

                    b.ToTable("Edu_OrgUnits", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.OrgUnitType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Edu_OrgUnitTypes", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Position", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Edu_Positions", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Semester", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<DateTime>("EndsOn")
                        .HasColumnType("datetime");

                    b.Property<int>("SemesterTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartsOn")
                        .HasColumnType("datetime");

                    b.Property<int>("StudyYear")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("SemesterTypeId");

                    b.HasIndex("StudyYear")
                        .HasDatabaseName("IX_Edu_Semesters_StudyYear");

                    b.ToTable("Edu_Semesters", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.SemesterType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<int>("OrderBy")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Edu_SemesterTypes", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Speciality", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int>("LevelId")
                        .HasColumnType("int");

                    b.Property<string>("ShortTitle")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("YearsOfStudy")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LevelId")
                        .HasDatabaseName("IX_Edu_Specialities_LevelId");

                    b.ToTable("Edu_Specialities", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.SpecialityLevel", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Edu_SpecialityLevels", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.SpecialitySpecialization", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int");

                    b.Property<int?>("SpecializationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SpecialityId")
                        .HasDatabaseName("IX_Edu_SpecialitySpecializations_SpecialityId");

                    b.HasIndex("SpecializationId")
                        .HasDatabaseName("IX_Edu_SpecialitySpecializations_SpecializationId");

                    b.ToTable("Edu_SpecialitySpecializations", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Specialization", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    b.Property<string>("Code")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TitleEn")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("TitleKz")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("TitleRu")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Edu_Specializations", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.SpecializationsOrgUnit", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<int?>("OrgUnitId")
                        .HasColumnType("int")
                        .HasColumnName("OrgUnitID");

                    b.Property<int?>("SpecializationId")
                        .HasColumnType("int")
                        .HasColumnName("SpecializationID");

                    b.HasKey("Id");

                    b.HasIndex("OrgUnitId")
                        .HasDatabaseName("IX_Edu_Specializations_OrgUnits_OrgUnitID");

                    b.HasIndex("SpecializationId")
                        .HasDatabaseName("IX_Edu_Specializations_OrgUnits_SpecializationID");

                    b.ToTable("Edu_Specializations_OrgUnits", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Student", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("StudentID");

                    b.Property<int?>("AcademicStatusId")
                        .HasColumnType("int");

                    b.Property<int?>("AdvisorId")
                        .HasColumnType("int");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<double?>("EctsGPA")
                        .HasColumnType("float");

                    b.Property<int?>("EducationTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EntryDate")
                        .HasColumnType("datetime");

                    b.Property<double?>("GPA")
                        .HasColumnType("float");

                    b.Property<int?>("GrantTypeId")
                        .HasColumnType("int");

                    b.Property<bool?>("IsScholarship")
                        .HasColumnType("bit");

                    b.Property<bool>("NeedsDorm")
                        .HasColumnType("bit");

                    b.Property<int?>("SpecialityId")
                        .HasColumnType("int");

                    b.Property<int?>("StatusId")
                        .HasColumnType("int");

                    b.Property<int?>("StudyLanguageId")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("StatusId");

                    b.ToTable("Edu_Students", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.StudentStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Edu_StudentStatuses", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.University.User", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<DateTime?>("DOB")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("IIN")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool?>("Male")
                        .HasColumnType("bit");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("MobilePhone")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhotoFileName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.ToTable("Edu_Users", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("AWM.Service.Domain.Wf.Entities.State", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsFinal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("SystemName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("WorkTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WorkTypeId", "SystemName")
                        .IsUnique()
                        .HasDatabaseName("UQ_State_Type_Name")
                        .HasFilter("[IsDeleted] = 0");

                    b.ToTable("States", "Wf");
                });

            modelBuilder.Entity("AWM.Service.Domain.Wf.Entities.Transition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<int>("FromStateId")
                        .HasColumnType("int");

                    b.Property<bool>("IsAutomatic")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<int?>("RoleAccessId")
                        .HasColumnType("int")
                        .HasColumnName("RoleAccessId");

                    b.Property<int>("ToStateId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FromStateId")
                        .HasDatabaseName("IX_Transitions_From");

                    b.HasIndex("RoleAccessId");

                    b.HasIndex("ToStateId");

                    b.ToTable("Transitions", "Wf");
                });

            modelBuilder.Entity("AWM.Service.Domain.Wf.Entities.WorkType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("SpecialityLevelId")
                        .HasColumnType("int")
                        .HasColumnName("SpecialityLevelId");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("UQ_WorkType_Name")
                        .HasFilter("[IsDeleted] = 0");

                    b.HasIndex("SpecialityLevelId");

                    b.ToTable("WorkTypes", "Wf");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.LocalAccount", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.User", "User")
                        .WithOne()
                        .HasForeignKey("AWM.Service.Domain.Auth.Entities.LocalAccount", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleOperation", b =>
                {
                    b.HasOne("AWM.Service.Domain.Auth.Entities.RoleOperation", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleOperationAction", b =>
                {
                    b.HasOne("AWM.Service.Domain.Auth.Entities.RoleAccess", "RoleAccess")
                        .WithMany("OperationActions")
                        .HasForeignKey("RoleAccessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AWM.Service.Domain.Auth.Entities.RoleActionType", "RoleActionType")
                        .WithMany("OperationActions")
                        .HasForeignKey("RoleActionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AWM.Service.Domain.Auth.Entities.RoleOperation", "RoleOperation")
                        .WithMany("OperationActions")
                        .HasForeignKey("RoleOperationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RoleAccess");

                    b.Navigation("RoleActionType");

                    b.Navigation("RoleOperation");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.UserAccess", b =>
                {
                    b.HasOne("AWM.Service.Domain.Auth.Entities.RoleAccess", "RoleAccess")
                        .WithMany("UserAccesses")
                        .HasForeignKey("RoleAccessId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AWM.Service.Domain.University.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RoleAccess");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AWM.Service.Domain.CommonDomain.Entities.Notification", b =>
                {
                    b.HasOne("AWM.Service.Domain.CommonDomain.Entities.NotificationTemplate", null)
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Notif_Template");

                    b.HasOne("AWM.Service.Domain.University.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("AWM.Service.Domain.CommonDomain.Entities.StaffAssignment", b =>
                {
                    b.HasOne("AWM.Service.Domain.Defense.Entities.Commission", null)
                        .WithMany("Assignments")
                        .HasForeignKey("CommissionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AWM.Service.Domain.CommonDomain.Entities.Stage", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.OrgUnit", null)
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Stages_Dept");

                    b.HasOne("AWM.Service.Domain.University.Semester", null)
                        .WithMany()
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Stages_Semester");

                    b.HasOne("AWM.Service.Domain.University.Speciality", null)
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Stages_Speciality");

                    b.HasOne("AWM.Service.Domain.CommonDomain.Entities.WorkflowStage", null)
                        .WithMany()
                        .HasForeignKey("WorkflowStageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Stages_WfStage");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Commission", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.OrgUnit", null)
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Comm_Dept");

                    b.HasOne("AWM.Service.Domain.University.Semester", null)
                        .WithMany()
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Comm_Semester");

                    b.HasOne("AWM.Service.Domain.University.Speciality", null)
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Comm_Speciality");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.EvaluationCriteria", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.OrgUnit", null)
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Crit_Dept");

                    b.HasOne("AWM.Service.Domain.University.Speciality", null)
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Crit_Speciality");

                    b.HasOne("AWM.Service.Domain.Wf.Entities.WorkType", null)
                        .WithMany()
                        .HasForeignKey("WorkTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Crit_Type");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Grade", b =>
                {
                    b.HasOne("AWM.Service.Domain.CommonDomain.Entities.StaffAssignment", null)
                        .WithMany()
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Grades_Assignment");

                    b.HasOne("AWM.Service.Domain.Defense.Entities.EvaluationCriteria", null)
                        .WithMany()
                        .HasForeignKey("CriteriaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Grades_Crit");

                    b.HasOne("AWM.Service.Domain.Defense.Entities.Schedule", null)
                        .WithMany("Grades")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Grades_Sched");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.PreDefenseAttempt", b =>
                {
                    b.HasOne("AWM.Service.Domain.Defense.Entities.Schedule", null)
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_PreDef_Schedule");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.StudentWork", null)
                        .WithMany()
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_PreDef_Work");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Protocol", b =>
                {
                    b.HasOne("AWM.Service.Domain.Defense.Entities.Commission", null)
                        .WithMany()
                        .HasForeignKey("CommissionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Protocols_Commission");

                    b.HasOne("AWM.Service.Domain.University.User", null)
                        .WithMany()
                        .HasForeignKey("FinalizedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Protocols_Finalizer");

                    b.HasOne("AWM.Service.Domain.Defense.Entities.Schedule", null)
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Protocols_Schedule");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Schedule", b =>
                {
                    b.HasOne("AWM.Service.Domain.Defense.Entities.Commission", null)
                        .WithMany()
                        .HasForeignKey("CommissionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Sched_Comm");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.StudentWork", null)
                        .WithMany()
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Sched_Work");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Attachment", b =>
                {
                    b.HasOne("AWM.Service.Domain.Thesis.Entities.AttachmentType", null)
                        .WithMany()
                        .HasForeignKey("AttachmentTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Attach_Type");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.AttachmentType", "AttachmentType")
                        .WithMany()
                        .HasForeignKey("AttachmentTypeId1");

                    b.HasOne("AWM.Service.Domain.Wf.Entities.State", null)
                        .WithMany()
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Attach_State");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.StudentWork", null)
                        .WithMany("Attachments")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Attach_Work");

                    b.Navigation("AttachmentType");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Direction", b =>
                {
                    b.HasOne("AWM.Service.Domain.Wf.Entities.State", null)
                        .WithMany()
                        .HasForeignKey("CurrentStateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Directions_State");

                    b.HasOne("AWM.Service.Domain.University.OrgUnit", null)
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Directions_Dept");

                    b.HasOne("AWM.Service.Domain.University.Semester", null)
                        .WithMany()
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Directions_Semester");

                    b.HasOne("AWM.Service.Domain.Wf.Entities.WorkType", null)
                        .WithMany()
                        .HasForeignKey("WorkTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Directions_Type");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.QualityCheck", b =>
                {
                    b.HasOne("AWM.Service.Domain.Thesis.Entities.Attachment", "Attachment")
                        .WithMany()
                        .HasForeignKey("AttachmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_QualityCheck_Attachment");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.CheckType", "CheckType")
                        .WithMany()
                        .HasForeignKey("CheckTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Check_Type");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.StudentWork", null)
                        .WithMany("QualityChecks")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Check_Work");

                    b.Navigation("Attachment");

                    b.Navigation("CheckType");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.SpecialityCheckType", b =>
                {
                    b.HasOne("AWM.Service.Domain.Thesis.Entities.CheckType", "CheckType")
                        .WithMany()
                        .HasForeignKey("CheckTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_SpecChecks_Type");

                    b.HasOne("AWM.Service.Domain.University.OrgUnit", "OrgUnit")
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_SpecChecks_OrgUnit");

                    b.HasOne("AWM.Service.Domain.University.Speciality", "Speciality")
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_SpecChecks_Speciality");

                    b.Navigation("CheckType");

                    b.Navigation("OrgUnit");

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.StudentWork", b =>
                {
                    b.HasOne("AWM.Service.Domain.Wf.Entities.State", null)
                        .WithMany()
                        .HasForeignKey("CurrentStateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Works_State");

                    b.HasOne("AWM.Service.Domain.University.OrgUnit", null)
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Works_Dept");

                    b.HasOne("AWM.Service.Domain.University.Semester", null)
                        .WithMany()
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Works_Semester");

                    b.HasOne("AWM.Service.Domain.University.Speciality", null)
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Works_Speciality");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.Topic", null)
                        .WithMany()
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Works_Topic");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Topic", b =>
                {
                    b.HasOne("AWM.Service.Domain.Thesis.Entities.Direction", null)
                        .WithMany("Topics")
                        .HasForeignKey("DirectionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Topics_Direction");

                    b.HasOne("AWM.Service.Domain.University.OrgUnit", null)
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Topics_Dept");

                    b.HasOne("AWM.Service.Domain.University.Semester", null)
                        .WithMany()
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Topics_Semester");

                    b.HasOne("AWM.Service.Domain.University.Speciality", "Speciality")
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Topics_Spec");

                    b.HasOne("AWM.Service.Domain.Wf.Entities.WorkType", null)
                        .WithMany()
                        .HasForeignKey("WorkTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Topics_Type");

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.TopicApplication", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.User", null)
                        .WithMany()
                        .HasForeignKey("ReviewedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Applications_Reviewer");

                    b.HasOne("AWM.Service.Domain.University.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Applications_Student");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.Topic", null)
                        .WithMany("Applications")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Applications_Topic");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.WorkParticipant", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Participants_Student");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.StudentWork", null)
                        .WithMany("Participants")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Participants_Work");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.WorkReview", b =>
                {
                    b.HasOne("AWM.Service.Domain.Thesis.Entities.StudentWork", null)
                        .WithMany("WorkReviews")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.WorkflowHistory", b =>
                {
                    b.HasOne("AWM.Service.Domain.Wf.Entities.State", null)
                        .WithMany()
                        .HasForeignKey("FromStateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_WfHist_FromState");

                    b.HasOne("AWM.Service.Domain.Wf.Entities.State", null)
                        .WithMany()
                        .HasForeignKey("ToStateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_WfHist_ToState");

                    b.HasOne("AWM.Service.Domain.University.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_WfHist_User");

                    b.HasOne("AWM.Service.Domain.Thesis.Entities.StudentWork", null)
                        .WithMany("WorkflowHistory")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_WfHist_Work");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Employee", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.User", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Edu_Employees_User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.EmployeePosition", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.Employee", "Employee")
                        .WithMany("Positions")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Edu_EmployeePositions_Employee");

                    b.HasOne("AWM.Service.Domain.University.OrgUnit", "OrgUnit")
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Edu_EmployeePositions_OrgUnit");

                    b.HasOne("AWM.Service.Domain.University.Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Edu_EmployeePositions_Position");

                    b.Navigation("Employee");

                    b.Navigation("OrgUnit");

                    b.Navigation("Position");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.OrgUnit", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.OrgUnit", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Edu_OrgUnits_Parent");

                    b.HasOne("AWM.Service.Domain.University.OrgUnitType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Edu_OrgUnits_Type");

                    b.Navigation("Parent");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Semester", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.SemesterType", "SemesterType")
                        .WithMany()
                        .HasForeignKey("SemesterTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Edu_Semesters_Type");

                    b.Navigation("SemesterType");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Speciality", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.SpecialityLevel", "Level")
                        .WithMany()
                        .HasForeignKey("LevelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Edu_Specialities_Level");

                    b.Navigation("Level");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.SpecialitySpecialization", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.Speciality", "Speciality")
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Edu_SpecialitySpecializations_Speciality");

                    b.HasOne("AWM.Service.Domain.University.Specialization", "Specialization")
                        .WithMany()
                        .HasForeignKey("SpecializationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Edu_SpecialitySpecializations_Specialization");

                    b.Navigation("Speciality");

                    b.Navigation("Specialization");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.SpecializationsOrgUnit", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.OrgUnit", "OrgUnit")
                        .WithMany()
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Edu_SpecializationsOrgUnits_OrgUnit");

                    b.HasOne("AWM.Service.Domain.University.Specialization", "Specialization")
                        .WithMany()
                        .HasForeignKey("SpecializationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Edu_SpecializationsOrgUnits_Specialization");

                    b.Navigation("OrgUnit");

                    b.Navigation("Specialization");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Student", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.User", "User")
                        .WithOne()
                        .HasForeignKey("AWM.Service.Domain.University.Student", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Edu_Students_StudentID_Edu_Users");

                    b.HasOne("AWM.Service.Domain.University.Speciality", "Speciality")
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Edu_Students_Speciality");

                    b.HasOne("AWM.Service.Domain.University.StudentStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Edu_Students_Status");

                    b.Navigation("Speciality");

                    b.Navigation("Status");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AWM.Service.Domain.Wf.Entities.State", b =>
                {
                    b.HasOne("AWM.Service.Domain.Wf.Entities.WorkType", null)
                        .WithMany("States")
                        .HasForeignKey("WorkTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_States_WorkType");
                });

            modelBuilder.Entity("AWM.Service.Domain.Wf.Entities.Transition", b =>
                {
                    b.HasOne("AWM.Service.Domain.Wf.Entities.State", null)
                        .WithMany()
                        .HasForeignKey("FromStateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Trans_From");

                    b.HasOne("AWM.Service.Domain.Auth.Entities.RoleAccess", null)
                        .WithMany()
                        .HasForeignKey("RoleAccessId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Trans_Role");

                    b.HasOne("AWM.Service.Domain.Wf.Entities.State", null)
                        .WithMany()
                        .HasForeignKey("ToStateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Trans_To");
                });

            modelBuilder.Entity("AWM.Service.Domain.Wf.Entities.WorkType", b =>
                {
                    b.HasOne("AWM.Service.Domain.University.SpecialityLevel", null)
                        .WithMany()
                        .HasForeignKey("SpecialityLevelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_WorkTypes_Level");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleAccess", b =>
                {
                    b.Navigation("OperationActions");

                    b.Navigation("UserAccesses");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleActionType", b =>
                {
                    b.Navigation("OperationActions");
                });

            modelBuilder.Entity("AWM.Service.Domain.Auth.Entities.RoleOperation", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("OperationActions");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Commission", b =>
                {
                    b.Navigation("Assignments");
                });

            modelBuilder.Entity("AWM.Service.Domain.Defense.Entities.Schedule", b =>
                {
                    b.Navigation("Grades");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Direction", b =>
                {
                    b.Navigation("Topics");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.StudentWork", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Participants");

                    b.Navigation("QualityChecks");

                    b.Navigation("WorkReviews");

                    b.Navigation("WorkflowHistory");
                });

            modelBuilder.Entity("AWM.Service.Domain.Thesis.Entities.Topic", b =>
                {
                    b.Navigation("Applications");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.Employee", b =>
                {
                    b.Navigation("Positions");
                });

            modelBuilder.Entity("AWM.Service.Domain.University.OrgUnit", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("AWM.Service.Domain.Wf.Entities.WorkType", b =>
                {
                    b.Navigation("States");
                });
#pragma warning restore 612, 618
        }
    }
}
