using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Akkatecture.Examples.Api.Domain.Repositories.Operations
{
    public interface IQueryOperations
    {
        Task<OperationsReadModel> Find(Guid operationId);
        Task<IReadOnlyCollection<OperationsReadModel>> FindAll();
    }
}