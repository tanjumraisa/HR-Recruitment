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
    public class LeavesController : AppController
    {
        private readonly ILeaveRequestFacade _facade;
        private readonly IEmployeeFacade _employeeFacade;
        private readonly ILeaveTypeFacade _leaveTypeFacade;

        public LeavesController(ILeaveRequestFacade facade, IEmployeeFacade employeeFacade, ILeaveTypeFacade leaveTypeFacade)
        {
            _facade = facade;
            _employeeFacade = employeeFacade;
            _leaveTypeFacade = leaveTypeFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetLeaveRequestsAsync();
            if (User.IsInRole(RoleNames.Employee))
            {
                var me = await _employeeFacade.GetEmployeeByUserIdAsync(User.GetUserId());
                items = items.Where(x => me != null && x.EmployeeId == me.EmployeeId).ToList();
            }
            return View(items);
        }

        [Authorize(Roles = RoleNames.HrAdminEmployee)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new LeaveRequest { FromDate = DateTime.Today, ToDate = DateTime.Today });
        }

        [Authorize(Roles = RoleNames.HrAdminEmployee)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequest model)
        {
            if (User.IsInRole(RoleNames.Employee))
            {
                var me = await _employeeFacade.GetEmployeeByUserIdAsync(User.GetUserId());
                if (me == null)
                {
                    ModelState.AddModelError(string.Empty, "You do not have an employee profile yet.");
                    await LoadLookupsAsync();
                    return View("Form", model);
                }
                model.EmployeeId = me.EmployeeId;
                model.Status = LeaveStatus.Pending;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddLeaveRequestAsync(model);
                    TempData["Success"] = "Leave request created.";
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
            var model = await _facade.GetLeaveRequestByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LeaveRequest model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateLeaveRequestAsync(model)) return NotFound();
                    TempData["Success"] = "Leave request updated.";
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
            var e = await _facade.GetLeaveRequestByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete leave request", Message = e.Employee.User.FullName + " " + e.FromDate.ToString("d") });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteLeaveRequestAsync(id)) throw new BusinessException("Record not found.");
            }, "Leave request deleted.");

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = RoleNames.HrAdmin)]
        public async Task<IActionResult> Decide(int id, bool approve)
        {
            var me = await _employeeFacade.GetEmployeeByUserIdAsync(User.GetUserId());
            return await TryAsync(() => _facade.DecideAsync(id, approve, me?.EmployeeId),
                approve ? "Leave approved." : "Leave rejected.");
        }

        private async Task LoadLookupsAsync()
        {
            ViewBag.Employees = (await _employeeFacade.GetEmployeesAsync()).Select(x => new SelectListItem(x.EmployeeCode + " - " + x.User.FullName, x.EmployeeId.ToString())).ToList();
            ViewBag.LeaveTypes = (await _leaveTypeFacade.GetLeaveTypesAsync()).Select(x => new SelectListItem(x.Name + " (" + x.DaysPerYear + " days/yr)", x.LeaveTypeId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
