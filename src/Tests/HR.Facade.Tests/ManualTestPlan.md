Business rule checks to do by hand in the web app (automatic ones: dotnet run --project Tests/HR.Facade.Tests):

- Employee > View: address, education, skill, contract, bank account and dependents can be added from the same page.
- Loan: add a loan, then repayments; the loan closes when the full amount is paid.
- Attendance correction: approving it changes the check-in / check-out of that attendance record.
- Work from home / shift swap / overtime: changing the status sends a notification (bell icon) to the employee.
- Training enrolment: more people than the capacity is refused; enrolled employee gets a notification.
- Contract: salary outside the grade band and a second active contract are refused.
- Reports: every report opens, prints on A4 and exports to CSV.
- API: GET /api/branches without X-Api-Key returns 401; with the key returns JSON.
