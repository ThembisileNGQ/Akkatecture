using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Akka;

namespace Akkatecture.Examples.Api.Domain.Repositories.Resources
{
    public class ResourcesQueryHandler : IQueryResources
    {
        private readonly IActorRef<ResourcesStorageHandler> _resourceStorageHandler;
        
        public ResourcesQueryHandler(
            IActorRef<ResourcesStorageHandler> resourceStorageHandler)
        {
            _resourceStorageHandler = resourceStorageHandler;
        }
        public async Task<ResourcesReadModel> Find(Guid id)
        {
            var query = new GetResourcesQuery();
            
            var result = await _resourceStorageHandler.Ask<List<ResourcesReadModel>>(query);

            var readModel = result.SingleOrDefault(x => x.Id == id);
            
            return readModel;
        }

        public async Task<IReadOnlyCollection<ResourcesReadModel>> FindAll()
        {
            var query =new GetResourcesQuery();
            
            var result = await _resourceStorageHandler.Ask<List<ResourcesReadModel>>(query);

            return result;
        }
    }

    public class GetResourcesQuery
    {
        
    }
}