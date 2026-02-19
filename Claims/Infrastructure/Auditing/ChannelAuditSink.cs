using Application.Common.Auditing;
using System.Threading.Channels;

namespace Infrastructure.Auditing
{
    public class ChannelAuditSink : IAuditSink
    {
        private readonly Channel<AuditEvent> _channel;

        public ChannelAuditSink(Channel<AuditEvent> channel)
        {
            _channel = channel;
        }

        public ValueTask EnqueueAsync(AuditEvent evt, CancellationToken ct = default)
        {
            if (_channel.Writer.TryWrite(evt))
                return ValueTask.CompletedTask;

            return _channel.Writer.WriteAsync(evt, ct);
        }
    }

}
