using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Helpers;
using HR.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HR.Web.UI.Controllers
{
    [Authorize(Roles = RoleNames.HrAdminEmployee)]
    public class PayrollController : AppController
    {
        private readonly IPayrollFacade _facade;
        private readonly IEmployeeFacade _employeeFacade;

        public PayrollController(IPayrollFacade facade, IEmployeeFacade employeeFacade)
        {
            _facade = facade;
            _employeeFacade = employeeFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetPayrollsAsync();
            if (User.IsInRole(RoleNames.Employee))
            {
                var me = await _employeeFacade.GetEmployeeByUserIdAsync(User.GetUserId());
                items = items.Where(x => me != null && x.EmployeeId == me.EmployeeId).ToList();
            }
            return View(items);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new Payroll { Month = DateTime.Today.Month, Year = DateTime.Today.Year });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payroll model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddPayrollAsync(model);
                    TempData["Success"] = "Payroll record created.";
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _facade.GetPayrollByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Payroll model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdatePayrollAsync(model)) return NotFound();
                    TempData["Success"] = "Payroll record updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _facade.GetPayrollByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete payroll record", Message = e.Employee.User.FullName + " " + e.Month + "/" + e.Year });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeletePayrollAsync(id)) throw new BusinessException("Record not found.");
            }, "Payroll record deleted.");

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = RoleNames.HrAdmin)]
        public async Task<IActionResult> GenerateAll(int month, int year)
        {
            try
            {
                var count = await _facade.GenerateForAllAsync(month, year);
                TempData["Success"] = $"{count} payroll record(s) generated for {month}/{year}.";
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = RoleNames.HrAdmin)]
        public Task<IActionResult> MarkPaid(int id) =>
            TryAsync(() => _facade.MarkPaidAsync(id), "Payroll marked as paid.");

        private async Task LoadLookupsAsync()
        {
            ViewBag.Employees = (await _employeeFacade.GetEmployeesAsync()).Select(x => new SelectListItem(x.EmployeeCode + " - " + x.User.FullName, x.EmployeeId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
