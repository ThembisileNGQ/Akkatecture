// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Akka.Actor;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;
using Akkatecture.Examples.Api.Domain.Repositories.Operations;
using Akkatecture.Examples.Api.Domain.Repositories.Resources;
using Akkatecture.Examples.Api.Domain.Sagas;
using Microsoft.AspNetCore.Builder;
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

            // Add Actors to DI as ActorRefProvider<T>
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