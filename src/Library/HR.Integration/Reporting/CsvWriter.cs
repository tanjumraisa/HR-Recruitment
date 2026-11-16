using System.Text;

namespace HR.Integration.Reporting
{
    public interface ICsvWriter
    {
        byte[] Write(IEnumerable<string> header, IEnumerable<IEnumerable<string>> rows);
    }

    // Builds a UTF-8 CSV file (with BOM so Excel shows Bangla / special characters correctly).
    public class CsvWriter : ICsvWriter
    {
        public byte[] Write(IEnumerable<string> header, IEnumerable<IEnumerable<string>> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Line(header));
            foreach (var row in rows) sb.AppendLine(Line(row));
            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        private static string Line(IEnumerable<string> cells) =>
            string.Join(",", cells.Select(c => "\"" + (c ?? "").Replace("\"", "\"\"") + "\""));
    }
}
