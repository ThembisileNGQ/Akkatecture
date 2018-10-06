using Akkatecture.Sagas;

namespace Akkatecture.Examples.Api.Domain.Sagas
{
    public class ResourceCreationSagaId : SagaId<ResourceCreationSagaId>
    {
        public ResourceCreationSagaId(string value) 
            : base(value)
        {
        }
    }
}