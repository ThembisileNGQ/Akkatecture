using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Akka;

namespace Akkatecture.Examples.Api.Domain.Repositories.Operations
{
    public class OperationsQueryHandler : IQueryOperations
    {
        private readonly IActorRef<OperationsStorageHandler> _operationStorageHandler;
        public OperationsQueryHandler(
                IActorRef<OperationsStorageHandler> operationStorageHandler)
        {
            _operationStorageHandler = operationStorageHandler;
        }
        public async Task<OperationsReadModel> Find(Guid operationId)
        {
            var query = new GetOperationsQuery();
            
            var result = await _operationStorageHandler.Ask<List<OperationsReadModel>>(query);

            var readModel = result.SingleOrDefault(x => x.Id == operationId);
            
            return readModel;
        }

        public async Task<IReadOnlyCollection<OperationsReadModel>> FindAll()
        {
            var query = new GetOperationsQuery();
            
            var result = await _operationStorageHandler.Ask<List<OperationsReadModel>>(query);
            
            return result;
        }
    }

    public class GetOperationsQuery
    {
        
    }
}