using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    // One page for any record: its fields, links to the parent records and lists of every child table.
    // Example: /records/Employee/5 shows the employee with address, education, skills, contracts, leave, payroll ...
    [Authorize(Roles = RoleNames.Staff)]
    public class RecordsController : AppController
    {
        private readonly IRecordFacade _records;

        public RecordsController(IRecordFacade records)
        {
            _records = records;
        }

        // Database overview: every table, row count and relations.
        [Route("records")]
        public async Task<IActionResult> Index() => View(await _records.GetTablesAsync());

        [Route("records/{entity}/{id:int}")]
        public async Task<IActionResult> Show(string entity, int id)
        {
            var details = await _records.GetDetailsAsync(entity, id);
            if (details == null) return NotFound();
            return View(details);
        }
    }
}
