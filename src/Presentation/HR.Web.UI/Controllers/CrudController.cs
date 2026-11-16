using HR.Integration.Reporting;
using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Helpers;
using HR.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    // Generic admin screens (list, search, filter, export, add, edit, soft delete) for one HRMS table.
    // Each module controller only passes its facade, e.g. BranchesController : CrudController<Branch>.
    // The business rules stay in the facade layer.
    public abstract class CrudController<TEntity> : AppController where TEntity : class, new()
    {
        private const int PageSize = 20;
        private const string IndexView = "~/Views/Shared/Crud/Index.cshtml";
        private const string FormView = "~/Views/Shared/Crud/Form.cshtml";

        protected readonly ICrudFacade<TEntity> Facade;
        protected readonly IRecordFacade Records;
        protected static readonly EntityMeta Meta = EntityMeta.For(typeof(TEntity));

        protected CrudController(ICrudFacade<TEntity> facade, IRecordFacade records)
        {
            Facade = facade;
            Records = records;
        }

        private ModuleInfo? Module => ModuleRegistry.ForEntity(typeof(TEntity).Name);

        // GET: list of active records, ?q= search, ?EmployeeId=5 filter, ?export=csv
        public virtual async Task<IActionResult> Index(string? q, int page = 1, string? export = null)
        {
            var items = await Facade.GetAllAsync();
            var lookups = await LoadLookupsAsync();
            var filters = new Dictionary<string, int>();

            foreach (var f in Meta.Fields.Where(f => f.Kind == FieldKind.ForeignKey))
            {
                if (int.TryParse(Request.Query[f.Name], out var value) && value > 0)
                {
                    filters[f.Name] = value;
                    items = items.Where(x => f.Prop.GetValue(x) is int v && v == value).ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(q))
            {
                items = items.Where(x => Meta.Fields.Any(f =>
                    EntityMeta.Show(f, x, lookups).Contains(q.Trim(), StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (export == "csv") return Csv(items, lookups);

            var totalPages = Math.Max(1, (int)Math.Ceiling(items.Count / (double)PageSize));
            page = Math.Clamp(page, 1, totalPages);
            return View(IndexView, new CrudIndexViewModel
            {
                Meta = Meta,
                Module = Module,
                Items = items.Skip((page - 1) * PageSize).Take(PageSize).Cast<object>().ToList(),
                Lookups = lookups,
                Filters = filters,
                Q = q,
                Page = page,
                TotalPages = totalPages,
                Total = items.Count
            });
        }

        // GET: details page with every connected record
        public IActionResult Details(int id) => Redirect($"/records/{typeof(TEntity).Name}/{id}");

        [HttpGet]
        public virtual async Task<IActionResult> Create(string? returnUrl)
        {
            var model = new TEntity();
            // Values from the query string, e.g. /EmployeeSkills/Create?EmployeeId=5
            foreach (var f in Meta.Fields.Where(f => f.Kind == FieldKind.ForeignKey))
                if (int.TryParse(Request.Query[f.Name], out var v) && v > 0) f.Prop.SetValue(model, v);
            return await FormAsync(model, true, returnUrl);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create([FromForm] TEntity model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await Facade.AddAsync(model);
                    TempData["Success"] = $"{EntityMeta.Words(typeof(TEntity).Name)} saved.";
                    return Back(returnUrl);
                }
                catch (BusinessException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return await FormAsync(model, true, returnUrl);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Edit(int id, string? returnUrl)
        {
            var model = await Facade.GetByIdAsync(id);
            if (model == null) return NotFound();
            return await FormAsync(model, false, returnUrl);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, [FromForm] TEntity model, string? returnUrl)
        {
            if (Meta.KeyOf(model) != id) return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await Facade.UpdateAsync(model)) return NotFound();
                    TempData["Success"] = $"{EntityMeta.Words(typeof(TEntity).Name)} updated.";
                    return Back(returnUrl);
                }
                catch (BusinessException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return await FormAsync(model, false, returnUrl);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var e = await Facade.GetByIdAsync(id);
            if (e == null) return NotFound();
            var texts = await Records.GetLookupAsync(typeof(TEntity));
            return View("~/Views/Shared/DeleteConfirm.cshtml", new DeleteViewModel
            {
                Id = id,
                Title = "Delete " + EntityMeta.Words(typeof(TEntity).Name).ToLower(),
                Message = texts.TryGetValue(id, out var t) ? t : "#" + id
            });
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public virtual Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await Facade.DeleteAsync(id)) throw new BusinessException("Record not found.");
            }, $"{EntityMeta.Words(typeof(TEntity).Name)} deleted.");

        // ---------- helpers ----------

        private async Task<IActionResult> FormAsync(TEntity model, bool isNew, string? returnUrl) =>
            View(FormView, new CrudFormViewModel
            {
                Meta = Meta,
                Module = Module,
                Entity = model,
                IsNew = isNew,
                Lookups = await LoadLookupsAsync(),
                ReturnUrl = returnUrl
            });

        private async Task<Dictionary<string, Dictionary<int, string>>> LoadLookupsAsync()
        {
            var result = new Dictionary<string, Dictionary<int, string>>();
            foreach (var f in Meta.Fields.Where(f => f.Kind == FieldKind.ForeignKey))
                result[f.Name] = await Records.GetLookupAsync(f.Target!);
            return result;
        }

        private IActionResult Back(string? returnUrl) =>
            !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction(nameof(Index));

        private FileContentResult Csv(List<TEntity> items, Dictionary<string, Dictionary<int, string>> lookups)
        {
            var csv = HttpContext.RequestServices.GetRequiredService<ICsvWriter>();
            var bytes = csv.Write(Meta.Fields.Select(f => f.Label), items.Select(x => Meta.Fields.Select(f => EntityMeta.Show(f, x, lookups))));
            return File(bytes, "text/csv", Meta.Title.Replace(' ', '_') + ".csv");
        }
    }
}
