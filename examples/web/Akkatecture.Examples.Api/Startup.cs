using Akka.Actor;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;
using Akkatecture.Examples.Api.Domain.Repositories.Operations;
using Akkatecture.Examples.Api.Domain.Repositories.Resources;
using Akkatecture.Examples.Api.Domain.Sagas;
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
            var actorSystem = ActorSystem.Create("api-system");
            var aggregateManager = actorSystem.ActorOf(Props.Create(() => new ResourceManager()),"resource-manager");
            var sagaManager = actorSystem.ActorOf(Props.Create(() => new ResourceCreationSagaManager(() => new ResourceCreationSaga())),"resourcecreation-sagamanager");
            var resourceStorage = actorSystem.ActorOf(Props.Create(() => new ResourcesStorageHandler()), "resource-storagehandler");
            var operationStorage = actorSystem.ActorOf(Props.Create(() => new OperationsStorageHandler()), "operation-storagehandler");

            // Add Actors to DI as IActorRef<T>
            services
                .AddAkkatecture(actorSystem)
                .AddActorReference<ResourceManager>(aggregateManager)
                .AddActorReference<ResourceCreationSagaManager>(sagaManager)
                .AddActorReference<ResourcesStorageHandler>(resourceStorage)
                .AddActorReference<OperationsStorageHandler>(operationStorage);
            
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add Read Side 
            services
                .AddTransient<IQueryResources, ResourcesQueryHandler>()
                .AddTransient<IQueryOperations, OperationsQueryHandler>();
        }

        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory)
        {

            app.Map("/api", api =>
            {
                api.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action}");
                });
            });
            
            
            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("Akkatecture.Examples.Api application has initialized.");
        }
    }
}