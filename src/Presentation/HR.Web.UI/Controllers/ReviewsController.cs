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
    public class ReviewsController : AppController
    {
        private readonly IPerformanceReviewFacade _facade;
        private readonly IEmployeeFacade _employeeFacade;

        public ReviewsController(IPerformanceReviewFacade facade, IEmployeeFacade employeeFacade)
        {
            _facade = facade;
            _employeeFacade = employeeFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetPerformanceReviewsAsync();
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
            return View("Form", new PerformanceReview());
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PerformanceReview model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddPerformanceReviewAsync(model);
                    TempData["Success"] = "Review created.";
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
            var model = await _facade.GetPerformanceReviewByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PerformanceReview model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdatePerformanceReviewAsync(model)) return NotFound();
                    TempData["Success"] = "Review updated.";
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
            var e = await _facade.GetPerformanceReviewByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete review", Message = e.Employee.User.FullName + " " + e.Period });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeletePerformanceReviewAsync(id)) throw new BusinessException("Record not found.");
            }, "Review deleted.");


        private async Task LoadLookupsAsync()
        {
            ViewBag.Employees = (await _employeeFacade.GetEmployeesAsync()).Select(x => new SelectListItem(x.EmployeeCode + " - " + x.User.FullName, x.EmployeeId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
