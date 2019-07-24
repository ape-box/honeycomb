using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;

namespace Sample.HostedServices
{
    public class MassTransitHostedService : IHostedService
    {
        private readonly IBusControl _busControl;

        public MassTransitHostedService(IBusControl busControl)
        {
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }

        public Task StartAsync(CancellationToken cancellationToken)
            => _busControl.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken)
            => _busControl.StopAsync(cancellationToken);
    }
}
