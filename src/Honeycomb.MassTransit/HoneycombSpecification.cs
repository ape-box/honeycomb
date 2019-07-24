using System;
using System.Collections.Generic;
using System.Linq;
using GreenPipes;
using Honeycomb.Models;
using Microsoft.Extensions.Options;

namespace Honeycomb.MassTransit
{
    public class HoneycombSpecification<T> : IPipeSpecification<T> where T : class, PipeContext
    {
        private readonly IHoneycombService _honeycombService;
        private readonly IOptions<HoneycombApiSettings> _settings;

        public HoneycombSpecification(IHoneycombService honeycombService, IOptions<HoneycombApiSettings> settings)
        {
            _honeycombService = honeycombService ?? throw new ArgumentNullException(nameof(honeycombService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void Apply(IPipeBuilder<T> builder)
            => builder.AddFilter(new HoneycombFilter<T>(_honeycombService, _settings));

        public IEnumerable<ValidationResult> Validate()
            => Enumerable.Empty<ValidationResult>();
    }
}
