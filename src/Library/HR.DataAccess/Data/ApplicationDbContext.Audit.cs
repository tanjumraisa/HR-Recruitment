using System.Globalization;
using HR.DataAccess.Interface;
using HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HR.DataAccess
{
    // Automatic records written on every save:
    //  1. AuditLog row for each insert / update / delete (with the changed columns),
    //  2. ApplicationStatusHistory row when a job application changes status,
    //  3. Notification to the candidate when his application status changes.
    public partial class ApplicationDbContext
    {
        private bool _writingAudit;

        // Set to true for bulk inserts (demo data) so no audit rows are written.
        public bool SkipAudit { get; set; }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            if (_writingAudit || SkipAudit)
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            var audits = new List<(EntityEntry Entry, string Action, string Changes)>();
            var statusChanges = new List<(JobApplication Application, string? FromStatus)>();

            var entries = ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .Where(e => e.Entity is not AuditLog && e.Entity is not Notification && e.Entity is not ApplicationStatusHistory)
                .ToList();

            foreach (var entry in entries)
            {
                var action = entry.State.ToString();
                var changes = string.Empty;

                if (entry.State == EntityState.Modified)
                {
                    var dbValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                    if (dbValues != null)
                    {
                        var diffs = entry.Properties
                            .Where(p => p.Metadata.Name != nameof(User.PasswordHash))
                            .Where(p => !Equals(dbValues[p.Metadata.Name], p.CurrentValue))
                            .Select(p => $"{p.Metadata.Name}: {Format(dbValues[p.Metadata.Name])} -> {Format(p.CurrentValue)}")
                            .ToList();
                        if (diffs.Count == 0) continue;
                        changes = string.Join("; ", diffs);

                        if (entry.Entity is ISoftDelete soft && soft.IsDeleted && dbValues[nameof(ISoftDelete.IsDeleted)] is false)
                            action = "Deleted";

                        if (entry.Entity is JobApplication app && dbValues[nameof(JobApplication.Status)] is string oldStatus && oldStatus != app.Status)
                            statusChanges.Add((app, oldStatus));
                    }
                }
                else if (entry.State == EntityState.Added && entry.Entity is JobApplication newApp)
                {
                    statusChanges.Add((newApp, null));
                }

                audits.Add((entry, action, changes));
            }

            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            if (audits.Count == 0 && statusChanges.Count == 0) return result;

            _writingAudit = true;
            try
            {
                var userName = _currentUser?.UserName ?? "system";
                var userId = _currentUser?.UserId is > 0 ? _currentUser.UserId : null;
                var added = new List<object>();

                foreach (var (entry, action, changes) in audits)
                {
                    var key = entry.Metadata.FindPrimaryKey()?.Properties.Select(p => entry.Property(p.Name).CurrentValue?.ToString());
                    var log = new AuditLog
                    {
                        EntityName = entry.Metadata.ClrType.Name,
                        RecordId = key == null ? null : string.Join(",", key),
                        Action = action,
                        Changes = changes.Length > 2000 ? changes[..2000] : changes,
                        UserName = userName,
                        CreatedAt = DateTime.Now
                    };
                    AuditLogs.Add(log);
                    added.Add(log);
                }

                foreach (var (app, fromStatus) in statusChanges)
                {
                    var history = new ApplicationStatusHistory
                    {
                        ApplicationId = app.ApplicationId,
                        FromStatus = fromStatus,
                        ToStatus = app.Status,
                        ChangedById = userId,
                        ChangedAt = DateTime.Now,
                        Remarks = fromStatus == null ? "Application submitted" : null
                    };
                    ApplicationStatusHistories.Add(history);
                    added.Add(history);

                    var candidateUserId = await Candidates.IgnoreQueryFilters()
                        .Where(c => c.CandidateId == app.CandidateId)
                        .Select(c => c.UserId)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (candidateUserId.HasValue && fromStatus != null)
                    {
                        var note = new Notification
                        {
                            UserId = candidateUserId.Value,
                            Title = "Application status: " + app.Status,
                            Message = $"Your application #{app.ApplicationId} moved from {fromStatus} to {app.Status}.",
                            Link = "/Careers/MyApplications",
                            CreatedAt = DateTime.Now
                        };
                        Notifications.Add(note);
                        added.Add(note);
                    }
                }

                await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                foreach (var item in added) Entry(item).State = EntityState.Detached;
            }
            finally
            {
                _writingAudit = false;
            }
            return result;
        }

        private static string Format(object? value) => value switch
        {
            null => "null",
            DateTime d => d.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            decimal m => m.ToString("0.##", CultureInfo.InvariantCulture),
            _ => value.ToString() ?? ""
        };
    }
}
