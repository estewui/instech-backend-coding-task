using System.Threading.Channels;

using Application.Common.Auditing;
using Application.Services;

namespace API.HostedServices
{
    public class AuditBackgroundService : BackgroundService
    {
        private readonly Channel<AuditEvent> _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuditBackgroundService> _logger;

        public AuditBackgroundService(
            Channel<AuditEvent> channel,
            IServiceScopeFactory scopeFactory,
            ILogger<AuditBackgroundService> logger)
        {
            _channel = channel;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var buffer = new List<AuditEvent>();

            try
            {
                await foreach (var evt in _channel.Reader.ReadAllAsync(stoppingToken))
                {
                    buffer.Add(evt);

                    // Drain any additional events already queued
                    while (_channel.Reader.TryRead(out var extra))
                        buffer.Add(extra);

                    await FlushBufferAsync(buffer);
                }
            }
            catch (OperationCanceledException)
            {
                // Shutdown requested — fall through to drain remaining events
            }

            // Graceful shutdown: drain all events still in the channel
            while (_channel.Reader.TryRead(out var remaining))
                buffer.Add(remaining);

            if (buffer.Count > 0)
            {
                _logger.LogInformation($"Draining {buffer.Count} remaining audit event(s) on shutdown.");
                await FlushBufferAsync(buffer);
            }
        }

        private async Task FlushBufferAsync(List<AuditEvent> buffer)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

                foreach (var auditEvent in buffer)
                {
                    switch (auditEvent.Type)
                    {
                        case AuditType.Claim:
                            await auditService.AuditClaim(auditEvent, CancellationToken.None);
                            break;
                        case AuditType.Cover:
                            await auditService.AuditCover(auditEvent, CancellationToken.None);
                            break;
                        default:
                            _logger.LogWarning($"Unknown audit type: {auditEvent.Type}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Audit background worker failure while flushing {Count} event(s).", buffer.Count);
            }
            finally
            {
                buffer.Clear();
            }
        }
    }

}
