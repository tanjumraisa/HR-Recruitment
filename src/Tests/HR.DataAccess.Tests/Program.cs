using Microsoft.EntityFrameworkCore.Metadata;
using HR.DataAccess;
using HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

// Database design checks. They build the EF Core model only, so no SQL Server is needed.
// Run:  dotnet run --project Tests/HR.DataAccess.Tests
var failed = 0;
void Check(bool ok, string name)
{
    Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
    if (!ok) failed++;
}

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=hrdb_model_check;Trusted_Connection=True;").Options;
using var db = new ApplicationDbContext(options);
var model = db.GetService<IDesignTimeModel>().Model;
var types = model.GetEntityTypes().ToList();

Check(types.Count >= 90, $"At least 90 tables (found {types.Count})");

var isolated = types.Where(t => !t.GetForeignKeys().Any() && !t.GetReferencingForeignKeys().Any()).Select(t => t.ClrType.Name).ToList();
Check(isolated.All(n => n is nameof(AuditLog) or nameof(TaxSlab)),
    "Every table is connected by a foreign key (except AuditLog / TaxSlab). Not connected: " + string.Join(", ", isolated));

var fks = types.SelectMany(t => t.GetForeignKeys()).ToList();
Check(fks.Count >= 80, $"Many relations ({fks.Count} foreign keys)");
Check(fks.All(f => f.DeleteBehavior == DeleteBehavior.Restrict), "All foreign keys are Restrict (no cascade delete)");

var soft = types.Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType)).ToList();
Check(soft.All(t => t.GetDeclaredQueryFilters().Any()), $"All {soft.Count} soft-delete tables have the IsDeleted query filter");

var uniqueWithFilter = types.SelectMany(t => t.GetIndexes()).Where(i => i.IsUnique && i.GetFilter() != null).Count();
Check(uniqueWithFilter >= 30, $"Unique rules ignore deleted rows ({uniqueWithFilter} filtered unique indexes)");

Check(model.FindEntityType(typeof(Branch))!.GetSeedData().Count() >= 1, "Seed data for branches");
Check(model.FindEntityType(typeof(Skill))!.GetSeedData().Count() >= 5, "Seed data for skills");
Check(model.FindEntityType(typeof(Employee))!.FindProperty(nameof(Employee.BranchId))!.IsNullable, "Employee.BranchId is optional (old rows stay valid)");

var migrations = db.GetService<IMigrationsAssembly>().Migrations.Keys.ToList();
Check(migrations.Any(m => m.EndsWith("_AddHrmsModules")), "Migration AddHrmsModules exists");
Check(migrations.Count == 5 && migrations.Last().EndsWith("_AddUserPermissions"), $"Migrations in order: {string.Join(", ", migrations)}");

Console.WriteLine(failed == 0 ? "All database design checks passed." : $"{failed} check(s) FAILED.");
return failed == 0 ? 0 : 1;
