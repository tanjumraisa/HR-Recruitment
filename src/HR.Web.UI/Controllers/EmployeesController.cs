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
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class EmployeesController : AppController
    {
        private readonly IEmployeeFacade _facade;
        private readonly IUserFacade _userFacade;
        private readonly IDepartmentFacade _departmentFacade;
        private readonly IDesignationFacade _designationFacade;
        private readonly IEmployeeFacade _employeeFacade;

        public EmployeesController(IEmployeeFacade facade, IUserFacade userFacade, IDepartmentFacade departmentFacade, IDesignationFacade designationFacade, IEmployeeFacade employeeFacade)
        {
            _facade = facade;
            _userFacade = userFacade;
            _departmentFacade = departmentFacade;
            _designationFacade = designationFacade;
            _employeeFacade = employeeFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetEmployeesAsync();
            return View(items);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new Employee { JoinDate = DateTime.Today });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddEmployeeAsync(model);
                    TempData["Success"] = "Employee created.";
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
            var model = await _facade.GetEmployeeByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateEmployeeAsync(model)) return NotFound();
                    TempData["Success"] = "Employee updated.";
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
            var e = await _facade.GetEmployeeByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete employee", Message = e.EmployeeCode + " - " + e.User.FullName });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteEmployeeAsync(id)) throw new BusinessException("Record not found.");
            }, "Employee deleted.");


        private async Task LoadLookupsAsync()
        {
            ViewBag.Users = (await _userFacade.GetUsersAsync()).Select(x => new SelectListItem(x.FullName + " (" + x.Email + ")", x.UserId.ToString())).ToList();
            ViewBag.Departments = (await _departmentFacade.GetDepartmentsAsync()).Select(x => new SelectListItem(x.Name, x.DepartmentId.ToString())).ToList();
            ViewBag.Designations = (await _designationFacade.GetDesignationsAsync()).Select(x => new SelectListItem(x.Title, x.DesignationId.ToString())).ToList();
            ViewBag.Managers = (await _employeeFacade.GetEmployeesAsync()).Select(x => new SelectListItem(x.EmployeeCode + " - " + x.User.FullName, x.EmployeeId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
