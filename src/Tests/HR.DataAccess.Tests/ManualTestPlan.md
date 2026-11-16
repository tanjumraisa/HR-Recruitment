Database checks to do by hand on SQL Server (LocalDB, database hrdb):

- Update-Database creates all tables; SQL Server Object Explorer > hrdb > Tables shows 65 tables + __EFMigrationsHistory.
- Database Diagram (or /records page) shows every new table linked by a foreign key.
- Deleting a Branch that is used by an employee is refused with a friendly message.
- A deleted record (IsDeleted = 1) no longer appears in lists, but the row is still in the table.
- A deleted name (e.g. a skill) can be added again.
- Every insert / update / delete creates a row in AuditLogs with the user name.
- Changing an application status adds a row to ApplicationStatusHistories and a notification for the candidate.
- Existing data (employees, jobs, applications) is unchanged after the AddHrmsModules migration.
