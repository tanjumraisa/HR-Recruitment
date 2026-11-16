using System.Reflection;
using HR.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Endpoints
{
    // CRUD endpoints for every HRMS module that has an ICrudFacade<T>, e.g.
    //   GET    /api/branches          all active rows
    //   GET    /api/branches/5        one row
    //   POST   /api/branches          insert (JSON body)
    //   PUT    /api/branches/5        update (JSON body, id must match)
    //   DELETE /api/branches/5        soft delete
    // The facade does all validation; a broken rule returns 400 with the message.
    public static class ModuleEndpoints
    {
        public static readonly List<(string Route, string Entity)> Modules = new();

        public static RouteGroupBuilder MapHrModules(this RouteGroupBuilder api)
        {
            var facadeTypes = typeof(ICrudFacade<>).Assembly.GetTypes()
                .Where(t => t.IsInterface && t.Namespace?.StartsWith("HR.Facade.Interface") == true)
                .Select(t => new
                {
                    Facade = t,
                    Crud = t.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICrudFacade<>))
                })
                .Where(x => x.Crud != null)
                .OrderBy(x => x.Facade.Name);

            var map = typeof(ModuleEndpoints).GetMethod(nameof(MapCrud), BindingFlags.NonPublic | BindingFlags.Static)!;
            foreach (var x in facadeTypes)
            {
                var entity = x.Crud!.GetGenericArguments()[0];
                var route = Plural(entity.Name).ToLowerInvariant();
                map.MakeGenericMethod(entity, x.Facade).Invoke(null, new object[] { api, route });
                Modules.Add((route, entity.Name));
            }
            return api;
        }

        private static void MapCrud<TEntity, TFacade>(RouteGroupBuilder api, string route)
            where TEntity : class
            where TFacade : ICrudFacade<TEntity>
        {
            var key = typeof(TEntity).GetProperty(typeof(TEntity).Name + "Id")!;
            var g = api.MapGroup("/" + route).WithTags(typeof(TEntity).Name);

            g.MapGet("/", async ([FromServices] TFacade f) => Results.Ok(await f.GetAllAsync()));

            g.MapGet("/{id:int}", async (int id, [FromServices] TFacade f) =>
                await f.GetByIdAsync(id) is { } e ? Results.Ok(e) : Results.NotFound());

            g.MapPost("/", async ([FromBody] TEntity e, [FromServices] TFacade f) =>
            {
                key.SetValue(e, 0);
                await f.AddAsync(e);
                return Results.Created($"/api/{route}/{key.GetValue(e)}", e);
            });

            g.MapPut("/{id:int}", async (int id, [FromBody] TEntity e, [FromServices] TFacade f) =>
            {
                if ((int)key.GetValue(e)! != id) return Results.BadRequest(new { error = "Id in the URL and in the body must be the same." });
                return await f.UpdateAsync(e) ? Results.Ok(e) : Results.NotFound();
            });

            g.MapDelete("/{id:int}", async (int id, [FromServices] TFacade f) =>
                await f.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());
        }

        public static string Plural(string name) =>
            name.EndsWith("y") && !name.EndsWith("ay") ? name[..^1] + "ies"
            : name.EndsWith("s") || name.EndsWith("x") || name.EndsWith("ch") ? name + "es"
            : name + "s";
    }
}
