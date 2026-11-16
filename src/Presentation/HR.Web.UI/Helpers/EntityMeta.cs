using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using HR.Entities;
using HR.Framework;

namespace HR.Web.UI.Helpers
{
    public enum FieldKind { Text, TextArea, Int, Decimal, Date, DateTime, Time, Bool, Choice, ForeignKey }

    public class FieldMeta
    {
        public PropertyInfo Prop { get; init; } = null!;
        public string Name => Prop.Name;
        public string Label { get; init; } = string.Empty;
        public FieldKind Kind { get; init; }
        public Type? Target { get; init; }
        public bool Nullable { get; init; }
        public bool Required { get; init; }
        public string[] Choices { get; init; } = Array.Empty<string>();
        public int? MaxLength { get; init; }
        public string? Min { get; init; }
        public string? Max { get; init; }
        public bool ShowInList { get; set; }
    }

    // Describes an entity class for the generic list / form pages (built once by reflection).
    public class EntityMeta
    {
        private static readonly ConcurrentDictionary<Type, EntityMeta> Cache = new();
        private static readonly string[] Skip = { nameof(ISoftDelete.IsDeleted), nameof(ISoftDelete.DeletedAt) };

        public Type Type { get; private init; } = null!;
        public PropertyInfo Key { get; private init; } = null!;
        public List<FieldMeta> Fields { get; private init; } = new();
        public string Title { get; private init; } = string.Empty;

        public static EntityMeta For(Type type) => Cache.GetOrAdd(type, Build);

        private static EntityMeta Build(Type type)
        {
            var key = type.GetProperty(type.Name + "Id") ?? type.GetProperties().First(p => p.GetCustomAttribute<KeyAttribute>() != null);
            var fkTargets = type.GetProperties()
                .Select(p => new { Nav = p, Fk = p.GetCustomAttribute<ForeignKeyAttribute>() })
                .Where(x => x.Fk != null)
                .ToDictionary(x => x.Fk!.Name, x => x.Nav.PropertyType);

            var fields = new List<FieldMeta>();
            foreach (var p in type.GetProperties())
            {
                if (p == key || !p.CanWrite || Skip.Contains(p.Name)) continue;
                var t = System.Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (t != typeof(string) && !t.IsValueType) continue;   // navigation / collection

                var nullable = System.Nullable.GetUnderlyingType(p.PropertyType) != null
                               || (t == typeof(string) && new NullabilityInfoContext().Create(p).WriteState == NullabilityState.Nullable);
                var range = p.GetCustomAttribute<RangeAttribute>();
                var choices = p.GetCustomAttribute<ChoicesAttribute>();
                var dataType = p.GetCustomAttribute<DataTypeAttribute>()?.DataType;
                var length = p.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength;

                FieldKind kind;
                if (fkTargets.ContainsKey(p.Name)) kind = FieldKind.ForeignKey;
                else if (choices != null) kind = FieldKind.Choice;
                else if (t == typeof(string)) kind = dataType == DataType.MultilineText || length > 300 ? FieldKind.TextArea : FieldKind.Text;
                else if (t == typeof(int)) kind = FieldKind.Int;
                else if (t == typeof(decimal)) kind = FieldKind.Decimal;
                else if (t == typeof(DateTime)) kind = dataType == DataType.Date ? FieldKind.Date : FieldKind.DateTime;
                else if (t == typeof(TimeSpan)) kind = FieldKind.Time;
                else if (t == typeof(bool)) kind = FieldKind.Bool;
                else continue;

                fields.Add(new FieldMeta
                {
                    Prop = p,
                    Label = LabelOf(p),
                    Kind = kind,
                    Target = fkTargets.GetValueOrDefault(p.Name),
                    Nullable = nullable,
                    Required = p.GetCustomAttribute<RequiredAttribute>() != null || (kind == FieldKind.ForeignKey && !nullable),
                    Choices = choices?.Values ?? Array.Empty<string>(),
                    MaxLength = length,
                    Min = kind == FieldKind.ForeignKey ? null : range?.Minimum?.ToString(),
                    Max = kind == FieldKind.ForeignKey ? null : range?.Maximum?.ToString()
                });
            }
            foreach (var f in fields.Where(f => f.Kind != FieldKind.TextArea).Take(8)) f.ShowInList = true;

            return new EntityMeta { Type = type, Key = key, Fields = fields, Title = Words(Plural(type.Name)) };
        }

        public int KeyOf(object entity) => (int)Key.GetValue(entity)!;

        // Text shown in the list / details for one field.
        public static string Show(FieldMeta f, object entity, IReadOnlyDictionary<string, Dictionary<int, string>> lookups)
        {
            var v = f.Prop.GetValue(entity);
            if (f.Kind == FieldKind.ForeignKey)
                return v is int id && lookups.TryGetValue(f.Name, out var texts) ? (texts.TryGetValue(id, out var t) ? t : $"#{id}") : "-";
            return v switch
            {
                null => "-",
                DateTime d when f.Kind == FieldKind.Date => d.ToString("dd MMM yyyy", CultureInfo.InvariantCulture),
                DateTime d => d.ToString("dd MMM yyyy HH:mm", CultureInfo.InvariantCulture),
                TimeSpan ts => ts.ToString(@"hh\:mm"),
                decimal m => m.ToString("N2", CultureInfo.InvariantCulture),
                bool b => b ? "Yes" : "No",
                _ => v.ToString() ?? "-"
            };
        }

        // Value written into an <input>.
        public static string InputValue(FieldMeta f, object entity)
        {
            var v = f.Prop.GetValue(entity);
            return v switch
            {
                null => "",
                DateTime d when f.Kind == FieldKind.Date => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTime d => d.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture),
                TimeSpan ts => ts.ToString(@"hh\:mm"),
                decimal m => m.ToString("0.##", CultureInfo.InvariantCulture),
                int i when f.Kind == FieldKind.ForeignKey && i == 0 => "",
                _ => v.ToString() ?? ""
            };
        }

        public static string LabelOf(PropertyInfo p)
        {
            var name = p.GetCustomAttribute<DisplayAttribute>()?.GetName();
            if (!string.IsNullOrEmpty(name)) return name;
            return Words(p.Name.EndsWith("Id") && p.Name.Length > 2 ? p.Name[..^2] : p.Name);
        }

        public static string Words(string name)
        {
            var s = Regex.Replace(name, "(?<=[a-z0-9])(?=[A-Z])", " ");
            return s.Length > 1 ? s[0] + s[1..].ToLowerInvariant() : s;
        }

        public static string Plural(string name) =>
            name.EndsWith("y") && !name.EndsWith("ay") ? name[..^1] + "ies"
            : name.EndsWith("s") || name.EndsWith("x") || name.EndsWith("ch") ? name + "es"
            : name + "s";
    }
}
