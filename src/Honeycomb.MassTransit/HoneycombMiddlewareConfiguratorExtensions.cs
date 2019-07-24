using GreenPipes;
using Honeycomb.Models;
using Microsoft.Extensions.Options;

namespace Honeycomb.MassTransit
{
    public static class HoneycombMiddlewareConfiguratorExtensions
    {
        public static void UseHoneycomb<T>(this IPipeConfigurator<T> configurator, IHoneycombService honeycombService, IOptions<HoneycombApiSettings> settings)
            where T : class, PipeContext
            => configurator.AddPipeSpecification(new HoneycombSpecification<T>(honeycombService, settings));
    }
}
