using System.Threading.Tasks;
using Honeycomb.MassTransit;
using MassTransit;

namespace Sample.Handlers
{
    public interface IAddValues
    {
        string Value { get; }
    }

    public class ValuesHandler : IConsumer<IAddValues>
    {
        public Task Consume(ConsumeContext<IAddValues> context)
        {
            context.EnrichLogEvent("value_added", context.Message.Value);

            return Task.CompletedTask;
        }
    }
}
