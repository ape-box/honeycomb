using Honeycomb;
using Honeycomb.AspNetCore.Middleware;
using Honeycomb.MassTransit;
using Honeycomb.Models;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Handlers;
using Sample.HostedServices;

namespace Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHoneycomb(Configuration);

            services.AddMassTransit(svcConfig => {
                svcConfig.AddConsumer<ValuesHandler>();

                svcConfig.AddBus(provider => Bus.Factory.CreateUsingInMemory(busConfig => {
                    var honeycombService = provider.GetService<IHoneycombService>();
                    var settings = provider.GetService<IOptions<HoneycombApiSettings>>();

                    // Add HoneycombMiddleware to MassTransit's bus
                    busConfig.UseHoneycomb(honeycombService, settings);

                    busConfig.ReceiveEndpoint(reConfig => {
                        reConfig.ConfigureConsumer<ValuesHandler>(provider);
                    });
                }));
            });

            services.AddHostedService<MassTransitHostedService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHoneycomb();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
