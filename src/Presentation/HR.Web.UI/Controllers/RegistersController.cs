using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HR.Web.UI.Controllers
{
    // Printable registers and letters, filled from the database (use the browser Print button -> A4 / Save as PDF).
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class RegistersController : Controller
    {
        private readonly IEmployeeFacade _employees;
        private readonly ILeaveRequestFacade _leaves;
        private readonly IAttendanceFacade _attendances;
        private readonly IPayrollFacade _payrolls;
        private readonly IDepartmentFacade _departments;
        private readonly IConfiguration _config;

        public RegistersController(IEmployeeFacade employees, ILeaveRequestFacade leaves, IAttendanceFacade attendances,
            IPayrollFacade payrolls, IDepartmentFacade departments, IConfiguration config)
        {
            _employees = employees;
            _leaves = leaves;
            _attendances = attendances;
            _payrolls = payrolls;
            _departments = departments;
            _config = config;
        }

        private void Company()
        {
            ViewBag.Company = _config["Company:Name"] ?? "Company Name";
            ViewBag.Address = _config["Company:Address"] ?? "Company address";
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> EmployeeRegister(int? departmentId)
        {
            Company();
            ViewBag.DepartmentId = departmentId;
            ViewBag.Departments = (await _departments.GetDepartmentsAsync())
                .Select(d => new SelectListItem(d.Name, d.DepartmentId.ToString())).ToList();

            var list = await _employees.GetEmployeesAsync();
            if (departmentId.HasValue) list = list.Where(e => e.DepartmentId == departmentId.Value).ToList();
            return View(list.OrderBy(e => e.EmployeeCode).ToList());
        }

        public async Task<IActionResult> LeaveRegister(DateTime? from, DateTime? to)
        {
            Company();
            var f = (from ?? new DateTime(DateTime.Today.Year, 1, 1)).Date;
            var t = (to ?? DateTime.Today).Date;
            ViewBag.From = f; ViewBag.To = t;

            var list = (await _leaves.GetLeaveRequestsAsync())
                .Where(l => l.FromDate <= t && l.ToDate >= f)
                .OrderBy(l => l.Employee.EmployeeCode).ThenBy(l => l.FromDate).ToList();
            return View(list);
        }

        public async Task<IActionResult> AttendanceRegister(DateTime? from, DateTime? to, int? employeeId)
        {
            Company();
            var f = (from ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).Date;
            var t = (to ?? DateTime.Today).Date;
            ViewBag.From = f; ViewBag.To = t; ViewBag.EmployeeId = employeeId;
            ViewBag.Employees = (await _employees.GetEmployeesAsync())
                .Select(e => new SelectListItem(e.EmployeeCode + " - " + e.User.FullName, e.EmployeeId.ToString())).ToList();

            var list = (await _attendances.GetAttendancesAsync())
                .Where(a => a.Date >= f && a.Date <= t && (!employeeId.HasValue || a.EmployeeId == employeeId.Value))
                .OrderBy(a => a.Employee.EmployeeCode).ThenBy(a => a.Date).ToList();
            return View(list);
        }

        public async Task<IActionResult> WagesRegister(int? month, int? year)
        {
            Company();
            var m = month ?? DateTime.Today.Month;
            var y = year ?? DateTime.Today.Year;
            ViewBag.Month = m; ViewBag.Year = y;

            var list = (await _payrolls.GetPayrollsAsync())
                .Where(p => p.Month == m && p.Year == y)
                .OrderBy(p => p.Employee.EmployeeCode).ToList();
            return View(list);
        }

        public async Task<IActionResult> AppointmentLetter(int? employeeId)
        {
            Company();
            ViewBag.Employees = (await _employees.GetEmployeesAsync())
                .Select(e => new SelectListItem(e.EmployeeCode + " - " + e.User.FullName, e.EmployeeId.ToString())).ToList();
            ViewBag.EmployeeId = employeeId;

            Employee? model = employeeId.HasValue ? await _employees.GetEmployeeByIdAsync(employeeId.Value) : null;
            // GetEmployeeByIdAsync includes User, Department, Designation and Manager.
            return View(model);
        }
    }
}
