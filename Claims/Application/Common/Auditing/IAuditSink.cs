namespace Application.Common.Auditing
{

    public interface IAuditSink
    {
        ValueTask EnqueueAsync(AuditEvent evt, CancellationToken ct = default);
    }
}
