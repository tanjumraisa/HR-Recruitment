using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HR.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHrmsPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CanteenItems",
                columns: table => new
                {
                    CanteenItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubsidyPercent = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanteenItems", x => x.CanteenItemId);
                });

            migrationBuilder.CreateTable(
                name: "DisciplinaryActions",
                columns: table => new
                {
                    DisciplinaryActionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    IssuedById = table.Column<int>(type: "int", nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "date", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplinaryActions", x => x.DisciplinaryActionId);
                    table.ForeignKey(
                        name: "FK_DisciplinaryActions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisciplinaryActions_Employees_IssuedById",
                        column: x => x.IssuedById,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePromotions",
                columns: table => new
                {
                    EmployeePromotionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FromDesignationId = table.Column<int>(type: "int", nullable: true),
                    ToDesignationId = table.Column<int>(type: "int", nullable: false),
                    OldSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "date", nullable: false),
                    PromotionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePromotions", x => x.EmployeePromotionId);
                    table.ForeignKey(
                        name: "FK_EmployeePromotions_Designations_FromDesignationId",
                        column: x => x.FromDesignationId,
                        principalTable: "Designations",
                        principalColumn: "DesignationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePromotions_Designations_ToDesignationId",
                        column: x => x.ToDesignationId,
                        principalTable: "Designations",
                        principalColumn: "DesignationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePromotions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTransfers",
                columns: table => new
                {
                    EmployeeTransferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FromDepartmentId = table.Column<int>(type: "int", nullable: true),
                    ToDepartmentId = table.Column<int>(type: "int", nullable: false),
                    FromBranchId = table.Column<int>(type: "int", nullable: true),
                    ToBranchId = table.Column<int>(type: "int", nullable: true),
                    TransferDate = table.Column<DateTime>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTransfers", x => x.EmployeeTransferId);
                    table.ForeignKey(
                        name: "FK_EmployeeTransfers_Branches_FromBranchId",
                        column: x => x.FromBranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTransfers_Branches_ToBranchId",
                        column: x => x.ToBranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTransfers_Departments_FromDepartmentId",
                        column: x => x.FromDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTransfers_Departments_ToDepartmentId",
                        column: x => x.ToDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTransfers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FestivalBonusSetups",
                columns: table => new
                {
                    FestivalBonusSetupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Festival = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmploymentTypeId = table.Column<int>(type: "int", nullable: true),
                    CalculationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinServiceMonths = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FestivalBonusSetups", x => x.FestivalBonusSetupId);
                    table.ForeignKey(
                        name: "FK_FestivalBonusSetups_EmploymentTypes_EmploymentTypeId",
                        column: x => x.EmploymentTypeId,
                        principalTable: "EmploymentTypes",
                        principalColumn: "EmploymentTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialYears",
                columns: table => new
                {
                    FinancialYearId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialYears", x => x.FinancialYearId);
                });

            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    FundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FundType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeContributionPercent = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    EmployerContributionPercent = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.FundId);
                });

            migrationBuilder.CreateTable(
                name: "Grievances",
                columns: table => new
                {
                    GrievanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "date", nullable: false),
                    AssignedToId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grievances", x => x.GrievanceId);
                    table.ForeignKey(
                        name: "FK_Grievances_Employees_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grievances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HrAudits",
                columns: table => new
                {
                    HrAuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    AuditDate = table.Column<DateTime>(type: "date", nullable: false),
                    Auditor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrAudits", x => x.HrAuditId);
                    table.ForeignKey(
                        name: "FK_HrAudits_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HseIncidents",
                columns: table => new
                {
                    HseIncidentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentDate = table.Column<DateTime>(type: "date", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    IncidentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ActionTaken = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HseIncidents", x => x.HseIncidentId);
                    table.ForeignKey(
                        name: "FK_HseIncidents_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HseIncidents_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabourCases",
                columns: table => new
                {
                    LabourCaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    Court = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FiledOn = table.Column<DateTime>(type: "date", nullable: false),
                    NextHearing = table.Column<DateTime>(type: "date", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabourCases", x => x.LabourCaseId);
                    table.ForeignKey(
                        name: "FK_LabourCases_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveWithdrawals",
                columns: table => new
                {
                    LeaveWithdrawalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveRequestId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveWithdrawals", x => x.LeaveWithdrawalId);
                    table.ForeignKey(
                        name: "FK_LeaveWithdrawals_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveWithdrawals_LeaveRequests_LeaveRequestId",
                        column: x => x.LeaveRequestId,
                        principalTable: "LeaveRequests",
                        principalColumn: "LeaveRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutsourcingAgencies",
                columns: table => new
                {
                    OutsourcingAgencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsourcingAgencies", x => x.OutsourcingAgencyId);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    SurveyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.SurveyId);
                });

            migrationBuilder.CreateTable(
                name: "TravelExpenses",
                columns: table => new
                {
                    TravelExpenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    TravelDate = table.Column<DateTime>(type: "date", nullable: false),
                    FromPlace = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToPlace = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TransportCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FoodCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HotelCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelExpenses", x => x.TravelExpenseId);
                    table.ForeignKey(
                        name: "FK_TravelExpenses_Employees_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelExpenses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CanteenMeals",
                columns: table => new
                {
                    CanteenMealId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CanteenItemId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanteenMeals", x => x.CanteenMealId);
                    table.ForeignKey(
                        name: "FK_CanteenMeals_CanteenItems_CanteenItemId",
                        column: x => x.CanteenItemId,
                        principalTable: "CanteenItems",
                        principalColumn: "CanteenItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanteenMeals_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HrBudgets",
                columns: table => new
                {
                    HrBudgetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialYearId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BudgetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrBudgets", x => x.HrBudgetId);
                    table.ForeignKey(
                        name: "FK_HrBudgets_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrBudgets_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "FinancialYearId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundMemberships",
                columns: table => new
                {
                    FundMembershipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "date", nullable: false),
                    NomineeId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundMemberships", x => x.FundMembershipId);
                    table.ForeignKey(
                        name: "FK_FundMemberships_Dependents_NomineeId",
                        column: x => x.NomineeId,
                        principalTable: "Dependents",
                        principalColumn: "DependentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundMemberships_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundMemberships_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "FundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutsourcedEmployees",
                columns: table => new
                {
                    OutsourcedEmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OutsourcingAgencyId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    JoinDate = table.Column<DateTime>(type: "date", nullable: false),
                    DailyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsourcedEmployees", x => x.OutsourcedEmployeeId);
                    table.ForeignKey(
                        name: "FK_OutsourcedEmployees_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutsourcedEmployees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutsourcedEmployees_OutsourcingAgencies_OutsourcingAgencyId",
                        column: x => x.OutsourcingAgencyId,
                        principalTable: "OutsourcingAgencies",
                        principalColumn: "OutsourcingAgencyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyQuestions",
                columns: table => new
                {
                    SurveyQuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    QuestionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyQuestions", x => x.SurveyQuestionId);
                    table.ForeignKey(
                        name: "FK_SurveyQuestions_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundContributions",
                columns: table => new
                {
                    FundContributionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundMembershipId = table.Column<int>(type: "int", nullable: false),
                    PayrollId = table.Column<int>(type: "int", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    EmployeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployerAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundContributions", x => x.FundContributionId);
                    table.ForeignKey(
                        name: "FK_FundContributions_FundMemberships_FundMembershipId",
                        column: x => x.FundMembershipId,
                        principalTable: "FundMemberships",
                        principalColumn: "FundMembershipId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundContributions_Payrolls_PayrollId",
                        column: x => x.PayrollId,
                        principalTable: "Payrolls",
                        principalColumn: "PayrollId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundWithdrawals",
                columns: table => new
                {
                    FundWithdrawalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundMembershipId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundWithdrawals", x => x.FundWithdrawalId);
                    table.ForeignKey(
                        name: "FK_FundWithdrawals_FundMemberships_FundMembershipId",
                        column: x => x.FundMembershipId,
                        principalTable: "FundMemberships",
                        principalColumn: "FundMembershipId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutsourcedAttendances",
                columns: table => new
                {
                    OutsourcedAttendanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutsourcedEmployeeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    CheckIn = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckOut = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsourcedAttendances", x => x.OutsourcedAttendanceId);
                    table.ForeignKey(
                        name: "FK_OutsourcedAttendances_OutsourcedEmployees_OutsourcedEmployeeId",
                        column: x => x.OutsourcedEmployeeId,
                        principalTable: "OutsourcedEmployees",
                        principalColumn: "OutsourcedEmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutsourcedSalaries",
                columns: table => new
                {
                    OutsourcedSalaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutsourcedEmployeeId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    WorkingDays = table.Column<int>(type: "int", nullable: false),
                    DailyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsourcedSalaries", x => x.OutsourcedSalaryId);
                    table.ForeignKey(
                        name: "FK_OutsourcedSalaries_OutsourcedEmployees_OutsourcedEmployeeId",
                        column: x => x.OutsourcedEmployeeId,
                        principalTable: "OutsourcedEmployees",
                        principalColumn: "OutsourcedEmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyResponses",
                columns: table => new
                {
                    SurveyResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyQuestionId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyResponses", x => x.SurveyResponseId);
                    table.ForeignKey(
                        name: "FK_SurveyResponses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyResponses_SurveyQuestions_SurveyQuestionId",
                        column: x => x.SurveyQuestionId,
                        principalTable: "SurveyQuestions",
                        principalColumn: "SurveyQuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CanteenItems",
                columns: new[] { "CanteenItemId", "DeletedAt", "IsDeleted", "MealType", "Name", "SubsidyPercent", "UnitPrice" },
                values: new object[,]
                {
                    { 1, null, false, "Breakfast", "Breakfast", 50m, 60m },
                    { 2, null, false, "Lunch", "Lunch", 50m, 120m },
                    { 3, null, false, "Snacks", "Evening snacks", 0m, 40m }
                });

            migrationBuilder.InsertData(
                table: "FestivalBonusSetups",
                columns: new[] { "FestivalBonusSetupId", "CalculationType", "DeletedAt", "EmploymentTypeId", "Festival", "IsDeleted", "MinServiceMonths", "Value" },
                values: new object[,]
                {
                    { 1, "PercentOfBasic", null, null, "EidUlFitr", false, 6, 100m },
                    { 2, "PercentOfBasic", null, null, "EidUlAdha", false, 6, 100m }
                });

            migrationBuilder.InsertData(
                table: "FinancialYears",
                columns: new[] { "FinancialYearId", "DeletedAt", "EndDate", "IsCurrent", "IsDeleted", "Name", "StartDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2025-26", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(2027, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2026-27", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "DeletedAt", "Description", "EmployeeContributionPercent", "EmployerContributionPercent", "FundType", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, null, null, 10m, 10m, "ProvidentFund", false, "Provident Fund" },
                    { 2, null, null, 0m, 8.33m, "Gratuity", false, "Gratuity Fund" },
                    { 3, null, null, 0m, 0.5m, "WorkersWelfare", false, "Workers' Welfare Fund" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanteenItems_Name",
                table: "CanteenItems",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CanteenMeals_CanteenItemId",
                table: "CanteenMeals",
                column: "CanteenItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CanteenMeals_EmployeeId",
                table: "CanteenMeals",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinaryActions_EmployeeId",
                table: "DisciplinaryActions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinaryActions_IssuedById",
                table: "DisciplinaryActions",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePromotions_EmployeeId",
                table: "EmployeePromotions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePromotions_FromDesignationId",
                table: "EmployeePromotions",
                column: "FromDesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePromotions_ToDesignationId",
                table: "EmployeePromotions",
                column: "ToDesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTransfers_EmployeeId",
                table: "EmployeeTransfers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTransfers_FromBranchId",
                table: "EmployeeTransfers",
                column: "FromBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTransfers_FromDepartmentId",
                table: "EmployeeTransfers",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTransfers_ToBranchId",
                table: "EmployeeTransfers",
                column: "ToBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTransfers_ToDepartmentId",
                table: "EmployeeTransfers",
                column: "ToDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FestivalBonusSetups_EmploymentTypeId",
                table: "FestivalBonusSetups",
                column: "EmploymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialYears_Name",
                table: "FinancialYears",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FundContributions_FundMembershipId_Month_Year",
                table: "FundContributions",
                columns: new[] { "FundMembershipId", "Month", "Year" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FundContributions_PayrollId",
                table: "FundContributions",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_FundMemberships_EmployeeId",
                table: "FundMemberships",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FundMemberships_FundId_EmployeeId",
                table: "FundMemberships",
                columns: new[] { "FundId", "EmployeeId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FundMemberships_NomineeId",
                table: "FundMemberships",
                column: "NomineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_Name",
                table: "Funds",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FundWithdrawals_FundMembershipId",
                table: "FundWithdrawals",
                column: "FundMembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Grievances_AssignedToId",
                table: "Grievances",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Grievances_EmployeeId",
                table: "Grievances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrAudits_DepartmentId",
                table: "HrAudits",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HrBudgets_DepartmentId",
                table: "HrBudgets",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HrBudgets_FinancialYearId_DepartmentId_Category",
                table: "HrBudgets",
                columns: new[] { "FinancialYearId", "DepartmentId", "Category" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_HseIncidents_BranchId",
                table: "HseIncidents",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HseIncidents_EmployeeId",
                table: "HseIncidents",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabourCases_CaseNo",
                table: "LabourCases",
                column: "CaseNo",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_LabourCases_EmployeeId",
                table: "LabourCases",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveWithdrawals_EmployeeId",
                table: "LeaveWithdrawals",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveWithdrawals_LeaveRequestId",
                table: "LeaveWithdrawals",
                column: "LeaveRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedAttendances_OutsourcedEmployeeId_Date",
                table: "OutsourcedAttendances",
                columns: new[] { "OutsourcedEmployeeId", "Date" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedEmployees_BranchId",
                table: "OutsourcedEmployees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedEmployees_DepartmentId",
                table: "OutsourcedEmployees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedEmployees_OutsourcingAgencyId",
                table: "OutsourcedEmployees",
                column: "OutsourcingAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedSalaries_OutsourcedEmployeeId_Month_Year",
                table: "OutsourcedSalaries",
                columns: new[] { "OutsourcedEmployeeId", "Month", "Year" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcingAgencies_Name",
                table: "OutsourcingAgencies",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestions_SurveyId",
                table: "SurveyQuestions",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponses_EmployeeId",
                table: "SurveyResponses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponses_SurveyQuestionId",
                table: "SurveyResponses",
                column: "SurveyQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelExpenses_ApprovedById",
                table: "TravelExpenses",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_TravelExpenses_EmployeeId",
                table: "TravelExpenses",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanteenMeals");

            migrationBuilder.DropTable(
                name: "DisciplinaryActions");

            migrationBuilder.DropTable(
                name: "EmployeePromotions");

            migrationBuilder.DropTable(
                name: "EmployeeTransfers");

            migrationBuilder.DropTable(
                name: "FestivalBonusSetups");

            migrationBuilder.DropTable(
                name: "FundContributions");

            migrationBuilder.DropTable(
                name: "FundWithdrawals");

            migrationBuilder.DropTable(
                name: "Grievances");

            migrationBuilder.DropTable(
                name: "HrAudits");

            migrationBuilder.DropTable(
                name: "HrBudgets");

            migrationBuilder.DropTable(
                name: "HseIncidents");

            migrationBuilder.DropTable(
                name: "LabourCases");

            migrationBuilder.DropTable(
                name: "LeaveWithdrawals");

            migrationBuilder.DropTable(
                name: "OutsourcedAttendances");

            migrationBuilder.DropTable(
                name: "OutsourcedSalaries");

            migrationBuilder.DropTable(
                name: "SurveyResponses");

            migrationBuilder.DropTable(
                name: "TravelExpenses");

            migrationBuilder.DropTable(
                name: "CanteenItems");

            migrationBuilder.DropTable(
                name: "FundMemberships");

            migrationBuilder.DropTable(
                name: "FinancialYears");

            migrationBuilder.DropTable(
                name: "OutsourcedEmployees");

            migrationBuilder.DropTable(
                name: "SurveyQuestions");

            migrationBuilder.DropTable(
                name: "Funds");

            migrationBuilder.DropTable(
                name: "OutsourcingAgencies");

            migrationBuilder.DropTable(
                name: "Surveys");
        }
    }
}
