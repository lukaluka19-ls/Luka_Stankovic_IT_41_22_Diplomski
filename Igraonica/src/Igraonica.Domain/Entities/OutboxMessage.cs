using Igraonica.Domain.Common;
using Igraonica.Domain.Enums;

namespace Igraonica.Domain.Entities;

public class OutboxMessage : BaseEntity
{
    public OutboxMessageType Type { get; private set; }
    public string Recipient { get; private set; } = null!;
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? LastError { get; private set; }

    private OutboxMessage() { }

    public static OutboxMessage Create(OutboxMessageType type, string recipient, string subject,
        string body, DateTimeOffset now)
    {
        return new OutboxMessage
        {
            Type = type,
            Recipient = recipient,
            Subject = subject,
            Body = body,
            CreatedAt = now,
            RetryCount = 0,
        };
    }

    public bool IsSent => SentAt is not null;

    public void MarkSent(DateTimeOffset now) => SentAt = now;

    public void RegisterFailure(string error)
    {
        RetryCount++;
        LastError = error;
    }
}
