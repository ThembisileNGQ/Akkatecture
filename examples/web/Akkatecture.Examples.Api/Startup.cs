using Akka.Actor;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Akkatecture.Examples.Api
{
    public class Startup
    {
        public Startup(ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("Akkatecture.Examples.Api application is starting.");
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureActorSystem(services);
            
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void ConfigureActorSystem(IServiceCollection services)
        {
            services
                .AddAkkatecture(ActorSystem.Create("api-system"))
                .AddAggregateManager<ResourceManager, Resource, ResourceId>();
        }

        
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime lifetime)
        {

            app.UseMvc();
            
            
            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("Akkatecture.Examples.Api application has initialized.");
        }
    }
}