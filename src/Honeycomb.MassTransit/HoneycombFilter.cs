using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using Honeycomb.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Honeycomb.MassTransit
{
    public class HoneycombFilter<T> : IFilter<T> where T : class, PipeContext
    {
        private const string ContextItemName = "Honeycomb_event";
        private readonly IHoneycombService _service;
        private readonly IOptions<HoneycombApiSettings> _settings;

        public HoneycombFilter(IHoneycombService service, IOptions<HoneycombApiSettings> settings)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task Send(T context, IPipe<T> next)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var ev = context.GetOrAddPayload(() => new HoneycombEvent
            {
                DataSetName = _settings.Value.DefaultDataSet
            });

            try
            {
                await next.Send(context).ConfigureAwait(false);

                ev.Data.Add("meta.local_hostname", Environment.MachineName);

                if (context.TryGetPayload<ConsumeContext>(out var messageContext))
                {
                    ev.Data.Add("trace.trace_id", messageContext.CorrelationId);
                    ev.Data.Add("message.correlation_id", messageContext.CorrelationId);
                    ev.Data.Add("message.conversation_id", messageContext.ConversationId);
                    ev.Data.Add("message.types", messageContext.SupportedMessageTypes);
                    ev.Data.Add("message.input_address", messageContext.ReceiveContext.InputAddress);
                    ev.Data.Add("message.destination_address", messageContext.DestinationAddress);
                    ev.Data.Add("message.elapsed_time", messageContext.ReceiveContext.ElapsedTime);
                    ev.Data.Add("message.expiration_time", messageContext.ExpirationTime);
            	    ev.Data.TryAdd("duration_ms", stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception exception)
            {
                ev.Data.TryAdd("message.error", exception.Source);
                ev.Data.TryAdd("message.error_detail", exception.Message);
                throw ;
            }
            finally
            {
                _service.QueueEvent(ev);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope(ContextItemName);
        }
    }
}
