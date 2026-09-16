using Igraonica.Domain.Common;

namespace Igraonica.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string EntityName { get; private set; } = null!;
    public string Details { get; private set; } = null!; // jsonb
    public DateTimeOffset Timestamp { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(Guid userId, string action, string entityName,
        string detailsJson, DateTimeOffset now)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            Details = detailsJson,
            Timestamp = now,
        };
    }
}
