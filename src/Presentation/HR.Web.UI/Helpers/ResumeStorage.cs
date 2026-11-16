using HR.Framework;

namespace HR.Web.UI.Helpers
{
    // Saves uploaded CVs OUTSIDE wwwroot (App_Data/resumes) so nobody can open them with a direct link.
    public static class ResumeStorage
    {
        public const long MaxBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] Allowed = { ".pdf", ".docx" };

        public static string Folder(IWebHostEnvironment env) =>
            Path.Combine(env.ContentRootPath, "App_Data", "resumes");

        public static async Task<(string StoredName, string OriginalName)> SaveAsync(IWebHostEnvironment env, IFormFile file)
        {
            if (file.Length == 0) throw new BusinessException("The file is empty.");
            if (file.Length > MaxBytes) throw new BusinessException("The file is larger than 5 MB.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Allowed.Contains(ext)) throw new BusinessException("Only PDF and DOCX files are allowed.");

            // The first bytes must match the extension (PDF = "%PDF", DOCX = zip "PK").
            var head = new byte[4];
            int read;
            await using (var s = file.OpenReadStream())
            {
                read = await s.ReadAsync(head, 0, 4);
            }
            var valid = read == 4 && (ext == ".pdf"
                ? head[0] == 0x25 && head[1] == 0x50 && head[2] == 0x44 && head[3] == 0x46
                : head[0] == 0x50 && head[1] == 0x4B);
            if (!valid) throw new BusinessException("The file content does not match a PDF or DOCX document.");

            Directory.CreateDirectory(Folder(env));
            var stored = Guid.NewGuid().ToString("N") + ext;
            await using (var fs = new FileStream(Path.Combine(Folder(env), stored), FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            var original = Path.GetFileName(file.FileName);
            if (original.Length > 150) original = original[^150..];
            return (stored, original);
        }

        public static string FullPath(IWebHostEnvironment env, string storedName) =>
            Path.Combine(Folder(env), Path.GetFileName(storedName));

        public static void Delete(IWebHostEnvironment env, string? storedName)
        {
            if (string.IsNullOrEmpty(storedName)) return;
            var path = FullPath(env, storedName);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
