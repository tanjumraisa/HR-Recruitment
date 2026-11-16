# 5-7. Model, Project Setup, CRUD, Authentication and Business Logic

## Step map

| Framework step | Where it is in the code |
|---|---|
| 5. Model create | `src/HR.Entities/*.cs` (16 classes, data annotations) |
| 6a. DbContext | `src/HR.DataAccess/ApplicationDbContext.cs` (DbSets, indexes, relationships, seed data) |
| 6b. Migration | created with the `dotnet ef` commands below -> `src/HR.DataAccess/Migrations` |
| 6c. CRUD | `IRepository<T>` + `Repository<T>` in DataAccess, `I<Entity>Facade` + `<Entity>Facade` in Facade, one MVC controller + views per module in `HR.Web.UI` |
| 7a. Authentication | `AccountController`, cookie auth in `Program.cs`, claims (UserId, Name, Email, Role), password hashing in `UserFacade` |
| 7b. Authorization | `[Authorize(Roles = RoleNames.X)]` on controllers/actions + global `AuthorizeFilter` + role-aware menu |
| 7c. Business logic | `src/HR.Facade/Implementation/*Facade.cs` (`ValidateAsync` + workflow methods) and `HR.Framework/Statuses.cs` |

## Solution structure (same pattern as CRM-Education)

```text
HR-Recruitment/
|-- docs/                       requirement, ER, use case, DB design
`-- src/
    |-- HR.slnx
    |-- HR.Framework/           constants, roles, statuses, BusinessException, rules, extensions
    |-- HR.Entities/            models (one class per table)
    |-- HR.DataAccess/          ApplicationDbContext, IRepository<T>, Repository<T>, per-entity repositories, Migrations
    |-- HR.Facade/              business logic per entity + DependencyInjection.cs (AddHrFacade)
    `-- HR.Web.UI/              MVC controllers, Razor views, Program.cs, appsettings.json
```

Dependency direction: `Web.UI -> Facade -> DataAccess -> Entities -> Framework`. Controllers never talk to the DbContext directly.

## Run it

1. Install **.NET 10 SDK**, **SQL Server / SQL Express**, and the EF tool once: `dotnet tool install --global dotnet-ef`
2. Check the connection string in `src/HR.Web.UI/appsettings.json` (default `Server=.\SQLEXPRESS;Database=hrdb;...`).
3. From the repository root:

```bash
dotnet restore src/HR.slnx
dotnet build src/HR.slnx

# create the first migration (step 6b)
dotnet ef migrations add InitialCreate --project src/HR.DataAccess --startup-project src/HR.Web.UI

# optional: create the database now (the app also does this at start-up)
dotnet ef database update --project src/HR.DataAccess --startup-project src/HR.Web.UI

dotnet run --project src/HR.Web.UI
```

In Visual Studio you can run the same in **Package Manager Console**: select *HR.DataAccess* as the Default project, then `Add-Migration InitialCreate` and `Update-Database`.

4. Open the shown URL. Log in as **admin@hr.local / Admin@12345** (change it in `SeedAdmin` before real use).

## Quick test scenario

1. Admin: create Department *IT*, Designation *Software Engineer*, then a user with role *HRManager* (Users -> Add).
2. HR Manager: Job openings -> create job -> **Publish**.
3. Log out, **Register** as a candidate, open Careers -> job -> **Apply**.
4. HR Manager: Applications -> Details -> **Mark Shortlisted** -> **Schedule interview** -> edit the interview to *Completed* with a score -> **Issue offer** -> **Accepted**.
5. The candidate is now an Employee (log in again to get the new role). Check in, apply leave; HR approves; HR generates payroll.

## Design decisions

- **Facade = business logic.** Repositories only read/write; every rule in the Use Case document is a check in a facade `ValidateAsync` or workflow method, and violations throw `BusinessException`, which controllers show as alerts.
- **Passwords are hashed** with `PasswordHasher<User>` (the CRM-Education sample compares plain text; this project does not).
- **Edit saves only the entity** (`Entry(entity).State = Modified`), and fields the form does not carry (CreatedAt, PostedById, EmployeeCode ...) are restored from the stored record in `before_update`, so editing never wipes them.
- **Restrict deletes** for every foreign key (see DB design).
- **Known limits:** the offer-accept workflow performs several saves without a single database transaction; resume is a link/path, not an upload; no e-mail notification. These are listed as future work.
