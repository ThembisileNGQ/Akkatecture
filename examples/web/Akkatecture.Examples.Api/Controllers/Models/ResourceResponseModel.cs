using System;

namespace Akkatecture.Examples.Api.Controllers.Models
{
    public class ResourceResponseModel
    {
        public Guid Id { get; }
        public Guid OperationId { get; }

        public ResourceResponseModel(
            Guid id,
            Guid operationId)
        {
            Id = id;
            OperationId = operationId;
        }
    }
}