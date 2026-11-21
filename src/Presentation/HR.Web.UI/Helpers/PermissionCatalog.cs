using HR.Facade.Interface;

namespace HR.Web.UI.Helpers
{
    // Builds the permission list from the menu, so every screen automatically gets
    // View / Create / Edit / Delete permissions. Tree: Menu -> Screen -> Action.
    public static class PermissionCatalog
    {
        public static readonly string[] Actions = { "View", "Create", "Edit", "Delete" };

        public static List<PermissionDef> Build()
        {
            var list = new List<PermissionDef>();
            foreach (var g in ModuleRegistry.Groups)
            {
                var menu = "Menu." + g.Key;
                list.Add(new PermissionDef(menu, g.Title, null));
                foreach (var m in ModuleRegistry.All.Where(m => m.Group == g.Key))
                {
                    list.Add(new PermissionDef(m.Entity, m.Title, menu));
                    list.AddRange(Actions.Select(a => new PermissionDef($"{m.Entity}.{a}", $"{m.Title}: {a}", m.Entity)));
                }
            }
            list.Add(new PermissionDef("Menu.Print", "Reports and registers", null));
            list.Add(new PermissionDef("Reports.View", "All reports", "Menu.Print"));
            list.Add(new PermissionDef("Registers.View", "All registers and letters", "Menu.Print"));
            return list;
        }

        // Which permission a controller action needs. null = no permission check (login, dashboard, profile...).
        public static string? Required(string controller, string action)
        {
            if (controller.Equals("Reports", StringComparison.OrdinalIgnoreCase)) return "Reports.View";
            if (controller.Equals("Registers", StringComparison.OrdinalIgnoreCase)) return "Registers.View";
            var module = ModuleRegistry.All.FirstOrDefault(m => m.Controller.Equals(controller, StringComparison.OrdinalIgnoreCase));
            if (module == null) return null;
            var op = action switch
            {
                "Create" or "CheckIn" or "CheckOut" or "Generate" => "Create",
                "Edit" or "Decide" or "Approve" or "Reject" or "Status" or "Accept" => "Edit",
                "Delete" or "DeleteConfirmed" => "Delete",
                _ => "View"
            };
            return $"{module.Entity}.{op}";
        }
    }
}
