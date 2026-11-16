# HR and Recruitment

ASP.NET Core MVC (.NET 10) + EF Core + SQL Server. Layered like the CRM-Education project:
`HR.Entities`, `HR.DataAccess`, `HR.Facade`, `HR.Framework`, `HR.Web.UI`.

Modules: job openings, careers portal, candidates, applications, interviews, offers (auto-hire), employees, departments,
designations, leave, attendance, payroll, performance reviews, users and roles.

Documents (follow the project framework):

1. [Requirement analysis](docs/01-Requirement-Analysis.md)
2. [ER diagram](docs/02-ER-Diagram.md)
3. [Use case](docs/03-Use-Case.md)
4. [Database design](docs/04-Database-Design.md)
5. [Model, project setup, CRUD, authentication, business logic + how to run](docs/05-Implementation-Guide.md)

Quick start: see the "Run it" section of the implementation guide (needs `dotnet ef migrations add InitialCreate` once).
Default admin: `admin@hr.local` / `Admin@12345` (set in `src/HR.Web.UI/appsettings.json`).
