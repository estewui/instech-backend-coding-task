namespace Application.Common.Auditing
{
    public enum AuditType
    {
        Claim,
        Cover
    }
    
    public class AuditEvent
    {
        public AuditType Type { get; set; }
        public string? EntityId { get; set; }
        public DateTime Timestamp { get; set; }
        public required string HttpRequestType { get; set; }
    }
}
