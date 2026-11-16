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
    public class AttendanceController : AppController
    {
        private readonly IAttendanceFacade _facade;
        private readonly IEmployeeFacade _employeeFacade;

        public AttendanceController(IAttendanceFacade facade, IEmployeeFacade employeeFacade)
        {
            _facade = facade;
            _employeeFacade = employeeFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetAttendancesAsync();
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
            return View("Form", new Attendance { Date = DateTime.Today });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Attendance model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddAttendanceAsync(model);
                    TempData["Success"] = "Attendance record created.";
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
            var model = await _facade.GetAttendanceByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Attendance model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateAttendanceAsync(model)) return NotFound();
                    TempData["Success"] = "Attendance record updated.";
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
            var e = await _facade.GetAttendanceByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete attendance record", Message = e.Date.ToString("d") });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteAttendanceAsync(id)) throw new BusinessException("Record not found.");
            }, "Attendance record deleted.");

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = RoleNames.Employee)]
        public async Task<IActionResult> CheckIn()
        {
            var me = await _employeeFacade.GetEmployeeByUserIdAsync(User.GetUserId());
            if (me == null) { TempData["Error"] = "You do not have an employee profile."; return RedirectToAction(nameof(Index)); }
            return await TryAsync(() => _facade.CheckInAsync(me.EmployeeId), "Checked in.");
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = RoleNames.Employee)]
        public async Task<IActionResult> CheckOut()
        {
            var me = await _employeeFacade.GetEmployeeByUserIdAsync(User.GetUserId());
            if (me == null) { TempData["Error"] = "You do not have an employee profile."; return RedirectToAction(nameof(Index)); }
            return await TryAsync(() => _facade.CheckOutAsync(me.EmployeeId), "Checked out.");
        }

        private async Task LoadLookupsAsync()
        {
            ViewBag.Employees = (await _employeeFacade.GetEmployeesAsync()).Select(x => new SelectListItem(x.EmployeeCode + " - " + x.User.FullName, x.EmployeeId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
