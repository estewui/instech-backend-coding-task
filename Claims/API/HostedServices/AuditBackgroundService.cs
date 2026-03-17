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

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var first = await _channel.Reader.ReadAsync(stoppingToken);
                    buffer.Add(first);

                    while (_channel.Reader.TryRead(out var evt))
                        buffer.Add(evt);

                    using var scope = _scopeFactory.CreateScope();
                    var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

                    foreach (var auditEvent in buffer)
                    {
                        switch (auditEvent.Type)
                        {
                            case AuditType.Claim:
                                await auditService.AuditClaim(auditEvent, stoppingToken);
                                break;
                            case AuditType.Cover:
                                await auditService.AuditCover(auditEvent, stoppingToken);
                                break;
                            default:
                                throw new Exception($"Unknown audit type: {auditEvent.Type}");
                        }
                    }

                    buffer.Clear();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Audit background worker failure.");
                    buffer.Clear();
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }

}
