using Honeycomb.Models;
using MassTransit;

namespace Honeycomb.MassTransit
{
    public static class HoneycombConsumeContextExtensions
    {
        public static void EnrichLogEvent<T>(this ConsumeContext<T> context, string key, object value)
            where T : class
        {
            if (context.TryGetPayload<HoneycombEvent>(out var ev))
            {
                ev.Data.Add(key, value);
            }
        }
    }
}
