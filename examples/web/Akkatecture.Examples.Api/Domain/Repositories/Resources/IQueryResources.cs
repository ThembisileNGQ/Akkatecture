using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Akkatecture.Examples.Api.Domain.Repositories.Resources
{
    public interface IQueryResources
    {
        Task<ResourcesReadModel> Find(Guid id);
        Task<IReadOnlyCollection<ResourcesReadModel>> FindAll();
    }
}